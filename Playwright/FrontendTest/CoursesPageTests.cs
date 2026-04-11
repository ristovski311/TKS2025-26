using Microsoft.Playwright;
using NUnit.Framework;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FrontendTest
{
    [TestFixture]
    public class CoursesPageTests
    {
        IBrowser? browser;
        IPlaywright? playwright;
        IPage? page;
        IBrowserContext? context;

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

            // Login uvek pre testova

            await page.Locator(".login-email").FillAsync("ristovski311@gmail.com");
            await page.Locator(".login-pass").FillAsync("banana");
            await page.Locator(".toggle-password").ClickAsync();
            await page.Locator(".auth-button").ClickAsync();
        }

        [TearDown]
        public async Task Cleanup()
        {
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
            var card = page!.Locator(".course-card").Filter(new() { HasTextString = title });
            await card.HoverAsync();
            await card.Locator(".course-action-btn").Nth(1).WaitForAsync(new() { State = WaitForSelectorState.Visible });
            await card.Locator(".course-action-btn").Nth(1).ClickAsync(); //X dugme je drugo po redu zato nth(1)
            await page.Locator(".btn-submit").ClickAsync();
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

                await Task.Delay(500);//Sacekamo da se kreira kurs

                var card = page.Locator(".course-card").Filter(new() { HasTextString = title });
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

            var card = page!.Locator(".course-card").Filter(new() { HasTextString = title });
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

            var card = page!.Locator(".course-card").Filter(new() { HasTextString = title });
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

                await Task.Delay(500);

                var card = page.Locator(".course-card").Filter(new() { HasTextString = title });
                
                await card.HoverAsync();
                await card.Locator(".course-action-btn").Nth(0).WaitForAsync(new() { State = WaitForSelectorState.Visible });
                await card.Locator(".course-action-btn").Nth(0).ClickAsync(); //edit dugme je prvo, pa nth(0)

                await FillCourseModal(title_edited, semester, index, desc);

                await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Save changes.*") }).ClickAsync();
                await Task.Delay(500);

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

                await Task.Delay(500);

                var card = page.Locator(".course-card").Filter(new() { HasTextString = title });

                await card.HoverAsync();
                await card.Locator(".course-action-btn").Nth(0).WaitForAsync(new() { State = WaitForSelectorState.Visible });
                await card.Locator(".course-action-btn").Nth(0).ClickAsync(); //edit dugme je prvo, pa nth(0)

                await FillCourseModal(title_edited, semester, index, desc);

                await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Cancel.*") }).ClickAsync();
                await Task.Delay(500);

                card = page.Locator(".course-card").Filter(new() { HasTextString = title_edited });
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

            await Task.Delay(500);//Sacekamo da se kreira kurs

            await DeleteCourse(title); //Brisemo kurs

            var card = page.Locator(".course-card").Filter(new() { HasTextString = title });
            await Assertions.Expect(card).ToHaveCountAsync(0);
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

                await Task.Delay(1000);//Sacekamo da se kreira kurs

                var card = page!.Locator(".course-card").Filter(new() { HasTextString = title });

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

                await Task.Delay(1000);//Sacekamo da se kreira kurs

                var card = page!.Locator(".course-card").Filter(new() { HasTextString = title });

                await card.Locator(".btn-pass").ClickAsync();

                var modal = page.Locator(".modal-overlay");

                await Assertions.Expect(modal).ToHaveTextAsync(new Regex($".*{title}.*"));

                await modal.Locator(".modal-close").ClickAsync();
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

                await Task.Delay(1000);//Sacekamo da se kreira kurs

                var card = page!.Locator(".course-card").Filter(new() { HasTextString = title });

                await card.Locator(".btn-pass").ClickAsync();
                
                var modal = page.Locator(".modal-overlay");

                await modal.Locator(".form-input").FillAsync(grade.ToString());

                await modal.Locator(".btn-cancel").ClickAsync();

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

                await Task.Delay(1000);//Sacekamo da se kreira kurs

                var card = page!.Locator(".course-card").Filter(new() { HasTextString = title });

                await card.Locator(".btn-pass").ClickAsync();
                var modal = page.Locator(".modal-overlay");

                await modal.Locator(".form-input").FillAsync(grade.ToString());

                await modal.Locator(".btn-submit").ClickAsync();

                await Task.Delay(500); //Cekamo da se ucita ponovo kurs

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
            string title1 = "Test sorting course 1";
            string title2 = "Test sorting course 2";
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
                await Task.Delay(1000);
                
                await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Add Course.*") }).ClickAsync();
                await FillCourseModal(title2, semester2, index, desc2);
                await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Create course.*") }).ClickAsync();
                await Task.Delay(1000);

                await page.Locator(".sort-row .form-select").Nth(0).SelectOptionAsync(new SelectOptionValue { Label = "Semester" });
                await page.Locator(".sort-row .form-select").Nth(1).SelectOptionAsync(new SelectOptionValue { Label = "Descending" });

                await Task.Delay(1000);

                var allCards = page!.Locator(".course-card");
                int count = await allCards.CountAsync();

                int index1 = -1, index2 = -1;

                for (int i = 0; i < count; i++)
                {
                    var cardTitle = await allCards.Nth(i).Locator(".course-title").InnerTextAsync();
                    if (cardTitle == title1) index1 = i;
                    if (cardTitle == title2) index2 = i;
                }

                // Proveravamo da li je title1 (semester 1) posle title2 (semester 2) u listi
                Assert.That(index1, Is.GreaterThan(index2), $"Ocekivano da '{title2}' bude pre '{title1}' nakon sortiranja po semestru descending");
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
            string title1 = "Test sorting course 1";
            string title2 = "Test sorting course 2";
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
                await Task.Delay(1000);

                await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Add Course.*") }).ClickAsync();
                await FillCourseModal(title2, semester2, index, desc2);
                await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Create course.*") }).ClickAsync();
                await Task.Delay(1000);

                await page.Locator(".sort-row .form-select").Nth(0).SelectOptionAsync(new SelectOptionValue { Label = "Semester" });
                await page.Locator(".sort-row .form-select").Nth(1).SelectOptionAsync(new SelectOptionValue { Label = "Ascending" });

                await Task.Delay(1000);

                var allCards = page!.Locator(".course-card");
                int count = await allCards.CountAsync();

                int index1 = -1, index2 = -1;

                for (int i = 0; i < count; i++)
                {
                    var cardTitle = await allCards.Nth(i).Locator(".course-title").InnerTextAsync();
                    if (cardTitle == title1) index1 = i;
                    if (cardTitle == title2) index2 = i;
                }

                // Proveravamo da li je title1 (semester 1) pre title2 (semester 2) u listi
                Assert.That(index1, Is.LessThan(index2), $"Ocekivano da '{title1}' bude pre '{title2}' nakon sortiranja po semestru ascending");
            }
            finally
            {
                await DeleteCourse(title1);
                await DeleteCourse(title2);
            }
        }

    }
}