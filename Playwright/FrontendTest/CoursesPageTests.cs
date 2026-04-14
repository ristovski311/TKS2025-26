using Microsoft.Playwright;
using NUnit.Framework;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FrontendTest
{
    [TestFixture]
    [NonParallelizable]
    public class CoursesPageTests
    {
        IBrowser? browser;
        IPlaywright? playwright;
        IPage? page;
        IBrowserContext? context;

        //Test professor podaci
        string profFirstName = "Profa";
        string profLastName = "Profic";
        string profMail = "profa@example.com";
        string profPhone = "837489";
        string profOffice = "1234";

        [SetUp]
        public async Task Setup()
        {
            playwright = await Playwright.CreateAsync();
            browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false,
                SlowMo = 100
            });

            context = await browser.NewContextAsync();
            page = await context.NewPageAsync();

            await page.GotoAsync("http://127.0.0.1:5500/index.html");

            //User podaci za registraciju
            DateTime now = DateTime.Now;

            string userMail = $"test_{now:yyyyMMddHHmmss}@example.com";
            string username = $"test_{now:yyyyMMddHHmmss}";
            string userFirstname = $"test_{now:yyyyMMddHHmmss}_FN";
            string userLastname = $"test_{now:yyyyMMddHHmmss}_LN";
            string userPass = $"test_{now:yyyyMMddHHmmss}_pass";
            int userSemester = 1;
            string userPhone = "1234";

            //Registracija test korisnika
            await page.Locator(".auth-link").ClickAsync();
            await page.GetByPlaceholder("Username").FillAsync(username);
            await page.GetByPlaceholder("First name").FillAsync(userFirstname);
            await page.GetByPlaceholder("Last name").FillAsync(userLastname);
            await page.GetByPlaceholder("Email").FillAsync(userMail);
            await page.GetByPlaceholder("Password").FillAsync(userPass);
            await page.GetByPlaceholder("Semester").FillAsync(userSemester.ToString());
            await page.GetByPlaceholder("Phone").FillAsync(userPhone);

            await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex("Register") }).ClickAsync();

            await Assertions.Expect(page.Locator(".auth-title")).ToHaveTextAsync(new Regex(".*Login.*"));

            // Login uvek pre testova
            await page.Locator(".login-email").FillAsync(userMail);
            await page.Locator(".login-pass").FillAsync(userPass);
            await page.Locator(".toggle-password").ClickAsync();
            await page.Locator(".auth-button").ClickAsync();

            // Potreban nam je jedan test profesor za kurseve koga cemo kreirati u setup a obrisati u teardown
            await page.Locator(".main-nav-item-professors").ClickAsync();
            await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Add Professor.*") }).ClickAsync();
            await page.GetByPlaceholder(new Regex("First name")).FillAsync(profFirstName);
            await page.GetByPlaceholder(new Regex("Last name")).FillAsync(profLastName);
            await page.GetByPlaceholder(new Regex(".*email@.*")).FillAsync(profMail);
            await page.Locator("[name='phone']").FillAsync(profPhone);
            await page.Locator("[name='office']").FillAsync(profOffice);

            await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Add professor.*") }).ClickAsync();
            await Assertions.Expect(page.Locator(".loading-overlay")).ToBeHiddenAsync();
        }
        
        [TearDown]
        public async Task Cleanup()
        {
            var modal = page!.Locator(".modal-overlay");
            if (await modal.IsVisibleAsync())
                await page.Locator(".modal-close").ClickAsync();

            await page.Locator(".delete-button").ClickAsync();
            await page.Locator(".modal-overlay .btn-submit").ClickAsync();
            //await Assertions.Expect(page.Locator(".loading-overlay")).ToBeHiddenAsync();

            if (page != null) await page.CloseAsync();
            if (context != null) await context.CloseAsync();
            if (browser != null) await browser.CloseAsync();
            if (playwright != null) playwright.Dispose();
        }

        //--- 1

        //[Ignore("")]
        [Test]
        public async Task NavigateToCoursesPage()
        {
            await page.Locator(".main-nav-item-courses").ClickAsync();
            await Assertions.Expect(page.Locator(".page-title")).ToHaveTextAsync("My Courses");
        }

        //--- 2

        //[Ignore("")]
        [Test]
        public async Task AddCourse_OpenModal()
        {
            await page.Locator(".main-nav-item-courses").ClickAsync();
            await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Add Course.*")}).ClickAsync();
            await Assertions.Expect(page.Locator(".modal-overlay")).ToBeVisibleAsync();
        }

        //Pomocna fja za popunjavanje modala za kreiranje kuesa kad je modal otvoren

        public async Task FillCourseModal(string title, int semester, int indexToSelect, string description)
        {
            await page.Locator("[name='title']").FillAsync(title);
            await page.Locator("[name='semester']").FillAsync(semester.ToString());
            await page!.WaitForSelectorAsync("select[name='professorId'] option:nth-child(2)",
                                             new PageWaitForSelectorOptions { State = WaitForSelectorState.Attached }); //Sacekamo da se ucitaju profesori
            await page.Locator("[name='professorId']").SelectOptionAsync(new SelectOptionValue { Index = indexToSelect });
            await page.Locator(".form-textarea").FillAsync(description);
        }

        //Pomocna fja za brisanje test kurseva
        public async Task DeleteCourse(string title)
        {
            var modal = page!.Locator(".modal-overlay");
            if (await modal.IsVisibleAsync())
                await page.Locator(".modal-close").ClickAsync();

            var card = page!.Locator(".course-card:not(.skeleton-card)").Filter(new() { HasTextString = title });
            await card.HoverAsync();
            await card.Locator(".course-action-btn").Nth(1).WaitForAsync(new() { State = WaitForSelectorState.Visible });
            await card.Locator(".course-action-btn").Nth(1).ClickAsync(); //X dugme je drugo po redu zato nth(1)
            await page.Locator(".btn-submit").ClickAsync();

            await Assertions.Expect(page.Locator(".loading-overlay")).ToBeHiddenAsync();
            await Assertions.Expect(page.Locator(".modal-overlay")).ToBeHiddenAsync();
            //await card.Locator(".modal-overlay").WaitForAsync(new() { State = WaitForSelectorState.Hidden });
        }

        //--- 3

        //[Ignore("")]
        [Test]
        public async Task AddCourse_Success()
        {
            string title = "TestCourse1";
            int semester = 1;
            int index = 1;
            string desc = "Opis test kursa 1";

            try
            {
                await page.Locator(".main-nav-item-courses").ClickAsync();

                await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Add Course.*") }).ClickAsync();
                await FillCourseModal(title, semester, index, desc);
                await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Create course.*") }).ClickAsync();

                await Assertions.Expect(page.Locator(".loading-overlay")).ToBeHiddenAsync();
                await page.Locator(".course-card:not(.skeleton-card)").WaitForAsync(new() { State = WaitForSelectorState.Visible });

                var card = page.Locator(".course-card:not(.skeleton-card)").Filter(new() { HasTextString = title });
                await Assertions.Expect(card).ToBeVisibleAsync();
            }
            finally
            {
                await DeleteCourse(title);
            }
        }

        //--- 4

        //[Ignore("")]
        [Test]
        public async Task AddCourse_EmptyTitle() //Svako polje moze pojedinacno da se testira kad je prazno, ali da ne bi bili repetitivni testovi nisam ih stavio ovde
        {
            string title = "";
            int semester = 1;
            int index = 1;
            string desc = "Opis test kursa 1";

            await page.Locator(".main-nav-item-courses").ClickAsync();

            await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Add Course.*") }).ClickAsync();
            await FillCourseModal(title, semester, index, desc);
            await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Create course.*") }).ClickAsync();

            //Treba da overlay modal idalje bude prisutan
            await Assertions.Expect(page.Locator(".modal-overlay")).ToBeVisibleAsync();
        }

        //--- 5

        //[Ignore("")]
        [Test]
        public async Task AddCourse_CancelBtn()
        {
            string title = "Cancel Course";
            int semester = 1;
            int index = 1;
            string desc = "Ovaj kurs se nece kreirati nakon klika na cancel";

            await page.Locator(".main-nav-item-courses").ClickAsync();

            await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Add Course.*") }).ClickAsync();
            await FillCourseModal(title, semester, index, desc);
            await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Cancel.*") }).ClickAsync();

            var card = page!.Locator(".course-card:not(.skeleton-card)").Filter(new() { HasTextString = title });
            await Assertions.Expect(card).ToHaveCountAsync(0);
        }

        //--- 6

        //[Ignore("")]
        [Test]
        public async Task AddCourse_CloseModal()
        {
            string title = "Close modal Course";
            int semester = 1;
            int index = 1;
            string desc = "Ovaj kurs se nece kreirati nakon klika na close modal tj X";

            await page.Locator(".main-nav-item-courses").ClickAsync();

            await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Add Course.*") }).ClickAsync();
            await FillCourseModal(title, semester, index, desc);

            await page.Locator(".modal-close").ClickAsync();

            var card = page!.Locator(".course-card:not(.skeleton-card)").Filter(new() { HasTextString = title });
            await Assertions.Expect(card).ToHaveCountAsync(0);
        }

        //--- 7

        //[Ignore("")]
        [Test]
        public async Task EditCourse_TitleChanged()
        {
            string title = "EditCourse1";
            string title_edited = "New Title";
            int semester = 1;
            int index = 1;
            string desc = "Opis test kursa za edit 1";

            try
            {
                await page.Locator(".main-nav-item-courses").ClickAsync();

                await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Add Course.*") }).ClickAsync();
                await FillCourseModal(title, semester, index, desc);
                await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Create course.*") }).ClickAsync();

                await page.Locator(".course-card:not(.skeleton-card)").WaitForAsync(new() { State = WaitForSelectorState.Visible});
                var card = page.Locator(".course-card:not(.skeleton-card)").Filter(new() { HasTextString = title });
                
                await card.HoverAsync();
                await card.Locator(".course-action-btn").Nth(0).WaitForAsync(new() { State = WaitForSelectorState.Visible });
                await card.Locator(".course-action-btn").Nth(0).ClickAsync(); //edit dugme je prvo, pa nth(0)

                await FillCourseModal(title_edited, semester, index, desc);
                await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Save changes.*") }).ClickAsync();
                await Assertions.Expect(page.Locator(".loading-overlay")).ToBeHiddenAsync();

                await page.Locator(".course-card:not(.skeleton-card)").WaitForAsync(new() { State = WaitForSelectorState.Visible});
                card = page.Locator(".course-card").Filter(new() { HasTextString = title_edited });
                await Assertions.Expect(card).ToBeVisibleAsync();
            }
            finally
            {
                await DeleteCourse(title_edited);
            }
        }

        //--- 8

        //[Ignore("")]
        [Test]
        public async Task EditCourse_Cancel()
        {
            string title = "EditCourse1";
            string title_edited = "New Title";
            int semester = 1;
            int index = 1;
            string desc = "Opis test kursa za edit 1";

            try
            {
                await page.Locator(".main-nav-item-courses").ClickAsync();

                await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Add Course.*") }).ClickAsync();
                await FillCourseModal(title, semester, index, desc);
                await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Create course.*") }).ClickAsync();

                await Assertions.Expect(page.Locator(".loading-overlay")).ToBeHiddenAsync();
                await page.Locator(".course-card:not(.skeleton-card)").WaitForAsync(new() { State = WaitForSelectorState.Visible });
                var card = page.Locator(".course-card:not(.skeleton-card)").Filter(new() { HasTextString = title });

                await card.HoverAsync();
                await card.Locator(".course-action-btn").Nth(0).WaitForAsync(new() { State = WaitForSelectorState.Visible });
                await card.Locator(".course-action-btn").Nth(0).ClickAsync(); //edit dugme je prvo, pa nth(0)

                await FillCourseModal(title_edited, semester, index, desc);

                await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Cancel.*") }).ClickAsync();

                card = page.Locator(".course-card:not(.skeleton-card)").Filter(new() { HasTextString = title_edited });
                await Assertions.Expect(card).ToHaveCountAsync(0);
            }
            finally
            {
                await DeleteCourse(title);
            }
        }

        //--- 9

        //[Ignore("")]
        [Test]
        public async Task DeleteCourse_Success()
        {
            string title = "Test Delete Course1";
            int semester = 1;
            int index = 1;
            string desc = "Opis test kursa za brisanje 1";
            await page.Locator(".main-nav-item-courses").ClickAsync();

            await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Add Course.*") }).ClickAsync();
            await FillCourseModal(title, semester, index, desc);
            await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Create course.*") }).ClickAsync();

            await Assertions.Expect(page.Locator(".loading-overlay")).ToBeHiddenAsync();
            await page.Locator(".course-card:not(.skeleton-card)").WaitForAsync(new() { State = WaitForSelectorState.Visible });

            await DeleteCourse(title); //Brisemo kurs

            await Assertions.Expect(page.Locator(".course-card:not(.skeleton-card)").Filter(new() { HasTextString = title })).ToHaveCountAsync(0);
        }

        //--- 10

        //[Ignore("")]
        [Test]
        public async Task DeleteCourse_Cancel()
        {
            string title = "Test Delete Course1";
            int semester = 1;
            int index = 1;
            string desc = "Opis test kursa za brisanje 1";
            await page.Locator(".main-nav-item-courses").ClickAsync();

            try
            {
                await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Add Course.*") }).ClickAsync();
                await FillCourseModal(title, semester, index, desc);
                await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Create course.*") }).ClickAsync();

                await Assertions.Expect(page.Locator(".loading-overlay")).ToBeHiddenAsync();
                await page.Locator(".course-card:not(.skeleton-card)").WaitForAsync(new() { State = WaitForSelectorState.Visible });

                var card = page!.Locator(".course-card:not(.skeleton-card)").Filter(new() { HasTextString = title });

                await card.HoverAsync();
                await card.Locator(".course-action-btn").Nth(0).WaitForAsync(new() { State = WaitForSelectorState.Visible });
                await card.Locator(".course-action-btn").Nth(0).ClickAsync();
                await page.Locator(".btn-cancel").ClickAsync();

                await Assertions.Expect(card).ToBeVisibleAsync();
            }
            finally
            {
                await DeleteCourse(title);
            }
        }

        //--- 11

        //[Ignore("")]
        [Test]
        public async Task PassCourse_OpenModal_DisplayRightTitle()
        {
            string title = "Test Pass Course1";
            int semester = 1;
            int index = 1;
            string desc = "Opis test kursa za prolazak 1";
            await page.Locator(".main-nav-item-courses").ClickAsync();

            try
            {
                await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Add Course.*") }).ClickAsync();
                await FillCourseModal(title, semester, index, desc);
                await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Create course.*") }).ClickAsync();

                await Assertions.Expect(page.Locator(".loading-overlay")).ToBeHiddenAsync();
                await page.Locator(".course-card:not(.skeleton-card)").WaitForAsync(new() { State = WaitForSelectorState.Visible });

                var card = page!.Locator(".course-card:not(.skeleton-card)").Filter(new() { HasTextString = title });
                await card.Locator(".btn-pass").ClickAsync();

                var modal = page.Locator(".modal-overlay");
                await Assertions.Expect(modal).ToHaveTextAsync(new Regex($".*{title}.*"));

                await modal.Locator(".modal-close").ClickAsync();
                await page.Locator(".modal-overlay").WaitForAsync(new() { State = WaitForSelectorState.Hidden });
            }
            finally
            {
                await DeleteCourse(title);
            }
        }

        //--- 12

        //[Ignore("")]
        [Test]
        public async Task PassCourse_Cancel()
        {
            string title = "Test Pass Course1";
            int semester = 1;
            int index = 1;
            string desc = "Opis test kursa za prolazak 1";
            int grade = 10;
            await page.Locator(".main-nav-item-courses").ClickAsync();

            try
            {
                await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Add Course.*") }).ClickAsync();
                await FillCourseModal(title, semester, index, desc);
                await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Create course.*") }).ClickAsync();

                await Assertions.Expect(page.Locator(".loading-overlay")).ToBeHiddenAsync();
                await page.Locator(".course-card:not(.skeleton-card)").WaitForAsync(new() { State = WaitForSelectorState.Visible });

                var card = page!.Locator(".course-card:not(.skeleton-card)").Filter(new() { HasTextString = title });
                await card.Locator(".btn-pass").ClickAsync();

                var modal = page.Locator(".modal-overlay");
                await modal.Locator(".form-input").FillAsync(grade.ToString());

                await modal.Locator(".btn-cancel").ClickAsync();
                await page.Locator(".modal-overlay").WaitForAsync(new() { State = WaitForSelectorState.Hidden });
                await Assertions.Expect(card.Locator(".btn-pass")).ToBeVisibleAsync() ;
            }
            finally
            {
                await DeleteCourse(title);
            }
        }

        //--- 13

        //[Ignore("")]
        [Test]
        public async Task PassCourse_Success()
        {
            string title = "Test Pass Course1";
            int semester = 1;
            int index = 1;
            string desc = "Opis test kursa za prolazak 1";
            int grade = 10;
            await page.Locator(".main-nav-item-courses").ClickAsync();

            try
            {
                await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Add Course.*") }).ClickAsync();
                await FillCourseModal(title, semester, index, desc);
                await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Create course.*") }).ClickAsync();
                
                await Assertions.Expect(page.Locator(".loading-overlay")).ToBeHiddenAsync();
                await page.Locator(".course-card:not(.skeleton-card)").WaitForAsync(new() { State = WaitForSelectorState.Visible });

                var card = page!.Locator(".course-card:not(.skeleton-card)").Filter(new() { HasTextString = title });

                await card.Locator(".btn-pass").ClickAsync();
                var modal = page.Locator(".modal-overlay");

                await modal.Locator(".form-input").FillAsync(grade.ToString());

                await modal.Locator(".btn-submit").ClickAsync();
                await page.Locator(".modal-overlay").WaitForAsync(new() { State = WaitForSelectorState.Hidden });

                await Assertions.Expect(page.Locator(".loading-overlay")).ToBeHiddenAsync();
                await page.Locator(".course-card:not(.skeleton-card)").WaitForAsync(new() { State = WaitForSelectorState.Visible });

                await Assertions.Expect(card).ToHaveTextAsync(new Regex($".*Grade: {grade}.*"));
            }
            finally
            {
                await DeleteCourse(title);
            }
        }

        //--- 14

        //[Ignore("")]
        [Test]
        public async Task Sort_BySemester_Descending()
        {
            string title1 = "B - Test sorting course 1";
            string title2 = "A - Test sorting course 2";
            int semester1 = 1;
            int semester2 = 2;
            int index = 1;
            string desc1 = "Test kurs za sortiranje 1";
            string desc2 = "Test kurs za sortiranje 2";

            await page.Locator(".main-nav-item-courses").ClickAsync();

            try
            {
                await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Add Course.*") }).ClickAsync();
                await FillCourseModal(title1, semester1, index, desc1);
                await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Create course.*") }).ClickAsync();

                await Assertions.Expect(page.Locator(".loading-overlay")).ToBeHiddenAsync();
                await page.Locator(".course-card:not(.skeleton-card)").WaitForAsync(new() { State = WaitForSelectorState.Visible });

                await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Add Course.*") }).ClickAsync();
                await FillCourseModal(title2, semester2, index, desc2);
                await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Create course.*") }).ClickAsync();

                await Assertions.Expect(page.Locator(".loading-overlay")).ToBeHiddenAsync();
                await page.Locator(".course-card:not(.skeleton-card)").First.WaitForAsync(new() { State = WaitForSelectorState.Visible });

                await page.Locator(".sort-row .form-select").Nth(0).SelectOptionAsync(new SelectOptionValue { Label = "Semester" });
                await page.Locator(".course-card:not(.skeleton-card)").First.WaitForAsync(new() { State = WaitForSelectorState.Visible });

                await page.Locator(".sort-row .form-select").Nth(1).SelectOptionAsync(new SelectOptionValue { Label = "Descending" });
                await page.Locator(".course-card:not(.skeleton-card)").First.WaitForAsync(new() { State = WaitForSelectorState.Visible });

                var allCards = page.Locator(".course-card:not(.skeleton-card)");

                var firstTitle = await allCards.Nth(0).Locator(".course-title").InnerTextAsync();
                var secondTitle = await allCards.Nth(1).Locator(".course-title").InnerTextAsync();

                Assert.That(firstTitle.Trim(), Is.EqualTo(title2), $"Ocekivano da '{title2}' bude prvi nakon sortiranja descending");
                Assert.That(secondTitle.Trim(), Is.EqualTo(title1), $"Ocekivano da '{title1}' bude drugi nakon sortiranja descending");
            }
            finally
            {
                await DeleteCourse(title1);
                await DeleteCourse(title2);
            }
        }

        //--- 15

        //[Ignore("")]
        [Test]
        public async Task Sort_BySemester_Ascending()
        {
            string title1 = "B - Test sorting course 1";
            string title2 = "A - Test sorting course 2";
            int semester1 = 1;
            int semester2 = 2;
            int index = 1;
            string desc1 = "Test kurs za sortiranje 1";
            string desc2 = "Test kurs za sortiranje 2";

            await page.Locator(".main-nav-item-courses").ClickAsync();

            try
            {
                await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Add Course.*") }).ClickAsync();
                await FillCourseModal(title1, semester1, index, desc1);
                await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Create course.*") }).ClickAsync();

                await Assertions.Expect(page.Locator(".loading-overlay")).ToBeHiddenAsync();
                await page.Locator(".course-card:not(.skeleton-card)").WaitForAsync(new() { State = WaitForSelectorState.Visible });

                await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Add Course.*") }).ClickAsync();
                await FillCourseModal(title2, semester2, index, desc2);
                await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Create course.*") }).ClickAsync();

                await Assertions.Expect(page.Locator(".loading-overlay")).ToBeHiddenAsync();
                await page.Locator(".course-card:not(.skeleton-card)").First.WaitForAsync(new() { State = WaitForSelectorState.Visible });

                await page.Locator(".sort-row .form-select").Nth(0).SelectOptionAsync(new SelectOptionValue { Label = "Semester" });
                await page.Locator(".course-card:not(.skeleton-card)").First.WaitForAsync(new() { State = WaitForSelectorState.Visible });

                await page.Locator(".sort-row .form-select").Nth(1).SelectOptionAsync(new SelectOptionValue { Label = "Ascending" });
                await page.Locator(".course-card:not(.skeleton-card)").First.WaitForAsync(new() { State = WaitForSelectorState.Visible });

                var allCards = page.Locator(".course-card:not(.skeleton-card)");

                var firstTitle = await allCards.Nth(0).Locator(".course-title").InnerTextAsync();
                var secondTitle = await allCards.Nth(1).Locator(".course-title").InnerTextAsync();

                Assert.That(firstTitle.Trim(), Is.EqualTo(title1), $"Ocekivano da '{title1}' bude prvi nakon sortiranja ascending");
                Assert.That(secondTitle.Trim(), Is.EqualTo(title2), $"Ocekivano da '{title2}' bude drugi nakon sortiranja ascending");
            }
            finally
            { 
                await DeleteCourse(title1);
                await DeleteCourse(title2);
            }
        }
    }
}