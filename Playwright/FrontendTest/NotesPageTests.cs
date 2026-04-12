using Microsoft.Playwright;
using NUnit.Framework;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FrontendTest
{
    [TestFixture]
    [NonParallelizable]
    public class NotesPageTests
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

        //Test course podaci
        string courseTitle = "Test course";
        string courseSemester = "1";
        int courseProfIndex = 1;
        string courseDescription = "Ovo je test kurs za notes";

        [SetUp]
        public async Task Setup()
        {
            playwright = await Playwright.CreateAsync();
            browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false,
                SlowMo = 500
            });

            context = await browser.NewContextAsync();
            page = await context.NewPageAsync();

            await page.GotoAsync("http://127.0.0.1:5500/index.html");

            // Login uvek pre testova
            await page.Locator(".login-email").FillAsync("ristovski311@gmail.com");
            await page.Locator(".login-pass").FillAsync("banana");
            await page.Locator(".toggle-password").ClickAsync();
            await page.Locator(".auth-button").ClickAsync();

            // Potreban nam je jedan test profesor za kurseve koje cemo koristiti u notes
            await page.Locator(".main-nav-item-professors").ClickAsync();
            await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Add Professor.*") }).ClickAsync();
            await page.GetByPlaceholder(new Regex("First name")).FillAsync(profFirstName);
            await page.GetByPlaceholder(new Regex("Last name")).FillAsync(profLastName);
            await page.GetByPlaceholder(new Regex(".*email@.*")).FillAsync(profMail);
            await page.Locator("[name='phone']").FillAsync(profPhone);
            await page.Locator("[name='office']").FillAsync(profOffice);

            await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Add professor.*") }).ClickAsync();
            await page.Locator(".modal-overlay").WaitForAsync(new() { State = WaitForSelectorState.Hidden });

            //Potreban nam je i jedan kurs za notes
            await page.Locator(".main-nav-item-courses").ClickAsync();
            await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Add Course.*") }).ClickAsync();
            await page.Locator("[name='title']").FillAsync(courseTitle);
            await page.Locator("[name='semester']").FillAsync(courseSemester.ToString());
            await page!.WaitForSelectorAsync("select[name='professorId'] option:nth-child(2)",
                                             new PageWaitForSelectorOptions { State = WaitForSelectorState.Attached });
            await page.Locator("[name='professorId']").SelectOptionAsync(new SelectOptionValue { Index = courseProfIndex });
            await page.Locator(".form-textarea").FillAsync(courseDescription);

            await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Create course.*") }).ClickAsync();
        }

        [TearDown]
        public async Task Cleanup()
        {
            var modal = page!.Locator(".modal-overlay");
            if (await modal.IsVisibleAsync())
                await page.Locator(".modal-close").ClickAsync();

            //Dovoljno je obrisati samo profesora, i time ce svaki kurs kaskadno da se obrise
            await page.Locator(".main-nav-item-professors").ClickAsync();
            var card = page.Locator(".course-card").Filter(new() { HasTextString = profFirstName });
            await card.HoverAsync();
            await card.Locator(".course-action-btn").Nth(1).ClickAsync(new LocatorClickOptions { Force = true });
            await page.Locator(".btn-submit").ClickAsync();

            if (page != null) await page.CloseAsync();
            if (context != null) await context.CloseAsync();
            if (browser != null) await browser.CloseAsync();
            if (playwright != null) playwright.Dispose();
        }

        //--- 1

        //[Ignore("")]
        [Test]
        public async Task NavigateToNotesPage()
        {
            await page.Locator(".main-nav-item-notes").ClickAsync();
            await Assertions.Expect(page.Locator(".page-title")).ToHaveTextAsync("My Notes");
        }

        //--- 2

        //[Ignore("")]
        [Test]
        public async Task AddNote_OpenModal()
        {
            await page.Locator(".main-nav-item-notes").ClickAsync();

            await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*New Note.*") }).ClickAsync();
            await Assertions.Expect(page.Locator(".modal-overlay")).ToBeVisibleAsync();
        }

        //--- 3

        //Pomocna fja za popunjavanje forme modal-a note-a
        public async Task FillNoteModal(string title, string desc, string content, int index)
        {
            await page.Locator("[name='title']").FillAsync(title);
            await page.Locator("[name='description']").FillAsync(desc);
            await page.Locator("[name='content']").FillAsync(content);

            await page!.WaitForSelectorAsync("select[name='courseId'] option:nth-child(2)",
                                             new PageWaitForSelectorOptions { State = WaitForSelectorState.Attached }); //Sacekamo da se ucitaju kursevi
            await page.Locator("[name='courseId']").SelectOptionAsync(new SelectOptionValue { Index = index });
        }

        //Pomocna fja za brisanje note-a
        public async Task DeleteNote(string title)
        {
            var modal = page!.Locator(".modal-overlay");
            if (await modal.IsVisibleAsync())
                await page.Locator(".modal-close").ClickAsync();

            var card = page.Locator(".note-card").Filter(new() { HasTextString = title });
            await card.HoverAsync();
            await card.Locator(".note-action-btn").Nth(1).WaitForAsync(new() { State = WaitForSelectorState.Visible });
            await card.Locator(".note-action-btn").Nth(1).ClickAsync(new LocatorClickOptions { Force = true });
            await page.Locator(".modal-overlay .btn-submit").ClickAsync();
        }

        [Ignore("")]
        [Test]
        public async Task CreateNote_Success()
        {
            string noteTitle = "Test Note 1";
            string noteDescription = "Ovo je opis test note-a 1";
            string noteContent = "Ovo je sadrzaj test note-a 1";
            int noteIndex = 1;

            try
            {
                await page.Locator(".main-nav-item-notes").ClickAsync();

                await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*New Note.*") }).ClickAsync();
                await FillNoteModal(noteTitle, noteDescription, noteContent, noteIndex);
                await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Create Note.*") }).ClickAsync();

                await Task.Delay(1000);

                var card = page.Locator(".note-card").Filter(new() { HasTextString = noteTitle });
                await Assertions.Expect(card).ToBeVisibleAsync();
            }
            finally
            {
                await DeleteNote(noteTitle);
            }
        }

        ///--- 3

        //[Ignore("")]
        [Test]
        public async Task CreateNote_EmptyTitle()
        {
            string noteTitle = "";
            string noteDescription = "Ovo je opis test note-a 1";
            string noteContent = "Ovo je sadrzaj test note-a 1";
            int noteIndex = 1;

            await page.Locator(".main-nav-item-notes").ClickAsync();

            await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*New Note.*") }).ClickAsync();
            await FillNoteModal(noteTitle, noteDescription, noteContent, noteIndex);
            await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Create Note.*") }).ClickAsync();

            //Overlay idalje treba da je tu ako ne unesemo title
            await Assertions.Expect(page.Locator(".modal-overlay")).ToBeVisibleAsync();
        }


    }
}