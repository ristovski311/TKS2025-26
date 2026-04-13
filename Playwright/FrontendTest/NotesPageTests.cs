using Microsoft.Playwright;
using NUnit.Framework;
using System;
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
            await page.GetByText(new Regex("Register here")).ClickAsync();
            await page.GetByPlaceholder("Username").FillAsync(username);
            await page.GetByPlaceholder("First name").FillAsync(userFirstname);
            await page.GetByPlaceholder("Last name").FillAsync(userLastname);
            await page.GetByPlaceholder("Email").FillAsync(userMail);
            await page.GetByPlaceholder("Password").FillAsync(userPass);
            await page.GetByPlaceholder("Semester").FillAsync(userSemester.ToString());
            await page.GetByPlaceholder("Phone").FillAsync(userPhone);

            await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex("Register") }).ClickAsync();

            // Login uvek pre testova
            await page.Locator(".login-email").FillAsync(userMail);
            await page.Locator(".login-pass").FillAsync(userPass);
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
            await Assertions.Expect(page.Locator(".loading-overlay")).ToBeHiddenAsync();
        }

        [TearDown]
        public async Task Cleanup()
        {
            var modal = page!.Locator(".modal-overlay");
            if (await modal.IsVisibleAsync())
                await page.Locator(".modal-close").ClickAsync();

            ////Dovoljno je obrisati samo profesora, i time ce svaki kurs kaskadno da se obrise
            //await page.Locator(".main-nav-item-professors").ClickAsync();
            //var card = page.Locator(".course-card").Filter(new() { HasTextString = profFirstName });
            //await card.HoverAsync();
            //await card.Locator(".course-action-btn").Nth(1).ClickAsync(new LocatorClickOptions { Force = true });
            //await page.Locator(".btn-submit").ClickAsync();

            await page.Locator(".delete-button").ClickAsync();
            await page.Locator(".modal-overlay .btn-submit").ClickAsync();
            await Task.Delay(1000);

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

            await Task.Delay(500);

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

        //[Ignore("")]
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
                await Assertions.Expect(page.Locator(".modal-overlay")).ToBeVisibleAsync();
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

        ///--- 4

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
            await Assertions.Expect(page.Locator(".modal-overlay")).ToBeVisibleAsync();
            await FillNoteModal(noteTitle, noteDescription, noteContent, noteIndex);
            await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Create Note.*") }).ClickAsync();

            //Overlay idalje treba da je tu ako ne unesemo title
            await Assertions.Expect(page.Locator(".modal-overlay")).ToBeVisibleAsync();
        }

        ///--- 5

        //[Ignore("")]
        [Test]
        public async Task CreateNote_CancelBtn()
        {
            string noteTitle = "Test note";
            string noteDescription = "Ovo je opis test note-a 1";
            string noteContent = "Ovo je sadrzaj test note-a 1";
            int noteIndex = 1;

            await page.Locator(".main-nav-item-notes").ClickAsync();

            await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*New Note.*") }).ClickAsync();
            await Assertions.Expect(page.Locator(".modal-overlay")).ToBeVisibleAsync();
            await FillNoteModal(noteTitle, noteDescription, noteContent, noteIndex);
            await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Cancel.*") }).ClickAsync();

            var card = page!.Locator(".note-card").Filter(new() { HasTextString = noteTitle });
            await Assertions.Expect(card).ToHaveCountAsync(0);
        }

        ///--- 6

        //[Ignore("")]
        [Test]
        public async Task CreateNote_CloseModal()
        {
            string noteTitle = "Test note";
            string noteDescription = "Ovo je opis test note-a 1";
            string noteContent = "Ovo je sadrzaj test note-a 1";
            int noteIndex = 1;

            await page.Locator(".main-nav-item-notes").ClickAsync();

            await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*New Note.*") }).ClickAsync();
            await Assertions.Expect(page.Locator(".modal-overlay")).ToBeVisibleAsync();
            await FillNoteModal(noteTitle, noteDescription, noteContent, noteIndex);
            await page.Locator(".modal-close").ClickAsync();

            var card = page!.Locator(".note-card").Filter(new() { HasTextString = noteTitle });
            await Assertions.Expect(card).ToHaveCountAsync(0);
        }

        ///--- 7

        //[Ignore("")]
        [Test]
        public async Task EditNote_Title()
        {
            string noteTitle = "Test note";
            string noteTitleEdited = "Test note edited";
            string noteDescription = "Ovo je opis test note-a 1";
            string noteContent = "Ovo je sadrzaj test note-a 1";
            int noteIndex = 1;

            try
            {
                await page.Locator(".main-nav-item-notes").ClickAsync();

                await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*New Note.*") }).ClickAsync();
                await Assertions.Expect(page.Locator(".modal-overlay")).ToBeVisibleAsync();
                await FillNoteModal(noteTitle, noteDescription, noteContent, noteIndex);
                await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Create Note.*") }).ClickAsync();

                await Task.Delay(1000);

                var card = page.Locator(".note-card").Filter(new() { HasTextString = noteTitle });

                await card.HoverAsync();
                await card.Locator(".note-action-btn").Nth(0).WaitForAsync(new() { State = WaitForSelectorState.Visible });
                await card.Locator(".note-action-btn").Nth(0).ClickAsync();

                await FillNoteModal(noteTitleEdited, noteDescription, noteContent, noteIndex);

                await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Save changes.*") }).ClickAsync();
                await Task.Delay(1000);

                card = page.Locator(".note-card").Filter(new() { HasTextString = noteTitleEdited });

                await Assertions.Expect(card).ToBeVisibleAsync();
            }
            finally
            {
                await DeleteNote(noteTitle);
            }
        }

        ///--- 8

        //[Ignore("")]
        [Test]
        public async Task EditNote_Cancel()
        {
            string noteTitle = "Test note";
            string noteTitleEdited = "Test note edited";
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

                await card.HoverAsync();
                await card.Locator(".note-action-btn").Nth(0).WaitForAsync(new() { State = WaitForSelectorState.Visible });
                await card.Locator(".note-action-btn").Nth(0).ClickAsync();

                await FillNoteModal(noteTitleEdited, noteDescription, noteContent, noteIndex);

                await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Cancel.*") }).ClickAsync();
                await Task.Delay(500);
                card = page.Locator(".note-card").Filter(new() { HasTextString = noteTitleEdited });

                await Assertions.Expect(card).ToHaveCountAsync(0);
            }
            finally
            {
                await DeleteNote(noteTitle);
            }
        }

        //--- 9

        //[Ignore("")]
        [Test]
        public async Task DeleteNote_Success()
        {
            string noteTitle = "Test note";
            string noteDescription = "Ovo je opis test note-a 1";
            string noteContent = "Ovo je sadrzaj test note-a 1";
            int noteIndex = 1;

            await page.Locator(".main-nav-item-notes").ClickAsync();

            await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*New Note.*") }).ClickAsync();
            await Assertions.Expect(page.Locator(".modal-overlay")).ToBeVisibleAsync();
            await FillNoteModal(noteTitle, noteDescription, noteContent, noteIndex);
            await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Create Note.*") }).ClickAsync();

            await Task.Delay(500);//Sacekamo da se kreira note

            await DeleteNote(noteTitle); //Brisemo note

            var card = page.Locator(".note-card").Filter(new() { HasTextString = noteTitle });
            await Assertions.Expect(card).ToHaveCountAsync(0);
        }

        //--- 10

        //[Ignore("")]
        [Test]
        public async Task DeleteNote_Cancel()
        {
            string noteTitle = "Test note";
            string noteDescription = "Ovo je opis test note-a 1";
            string noteContent = "Ovo je sadrzaj test note-a 1";
            int noteIndex = 1;

            await page.Locator(".main-nav-item-notes").ClickAsync();

            try
            {
                await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*New Note.*") }).ClickAsync();
                await Assertions.Expect(page.Locator(".modal-overlay")).ToBeVisibleAsync();
                await FillNoteModal(noteTitle, noteDescription, noteContent, noteIndex);
                await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Create Note.*") }).ClickAsync();

                await Task.Delay(500);//Sacekamo da se kreira note

                var card = page.Locator(".note-card").Filter(new() { HasTextString = noteTitle });
                await card.HoverAsync();
                await card.Locator(".note-action-btn").Nth(1).WaitForAsync(new() { State = WaitForSelectorState.Visible });
                await card.Locator(".note-action-btn").Nth(1).ClickAsync(new LocatorClickOptions { Force = true });
                await page.Locator(".modal-overlay .btn-cancel").ClickAsync();

                card = page.Locator(".note-card").Filter(new() { HasTextString = noteTitle });
                await Assertions.Expect(card).ToBeVisibleAsync();
            }
            finally
            {
                await DeleteNote(noteTitle);
            }
            
        }

        //--- 11

        //Pomocna fja za brisanje foldera

        public async Task DeleteFolder(string title)
        {
            var modal = page!.Locator(".modal-overlay");
            if (await modal.IsVisibleAsync())
                await page.Locator(".modal-close").ClickAsync();
            
            var folderList = page.Locator(".folder-list");
            var folderCard = folderList.Locator(".folder-item").Filter(new() { HasTextString = title });
            await folderCard.HoverAsync();
            await folderCard.Locator(".folder-action-btn").Nth(1).WaitForAsync(new() { State = WaitForSelectorState.Visible });
            await folderCard.Locator(".folder-action-btn").Nth(1).ClickAsync(new LocatorClickOptions { Force = true });
            await page.Locator(".modal-overlay .btn-submit").ClickAsync();
        }

        //[Ignore("")]
        [Test]
        public async Task CreateFolder_Success()
        {
            string folderTitle = "Test folder";

            await page.Locator(".main-nav-item-notes").ClickAsync();

            try{
                await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*New Folder.*") }).ClickAsync();
                var modal = page.Locator(".modal-overlay");
                await modal.Locator(".form-input").FillAsync(folderTitle);
                await modal.Locator(".btn-submit").ClickAsync();

                await Task.Delay(1000);

                var folderList = page.Locator(".folder-list");

                await Assertions.Expect(folderList.Locator(".folder-item").Filter(new() { HasTextString = folderTitle })).ToHaveCountAsync(1);
            }
            finally
            {
                await DeleteFolder(folderTitle);
            }
        }

        //--- 12

        //[Ignore("")]
        [Test]
        public async Task CreateFolder_EmptyTitle()
        {
            string folderTitle = "";

            await page.Locator(".main-nav-item-notes").ClickAsync();

            await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*New Folder.*") }).ClickAsync();
            var modal = page.Locator(".modal-overlay");
            await modal.Locator(".form-input").FillAsync(folderTitle);
            await modal.Locator(".btn-submit").ClickAsync();

            await Task.Delay(1000);

            var folderList = page.Locator(".folder-list");

            await Assertions.Expect(modal).ToBeVisibleAsync();
        }

        //--- 13

        //[Ignore("")]
        [Test]
        public async Task EditFolder_Success()
        {
            string folderTitle = "Test folder 1";
            string folderTitleEdited = "New folder title";

            await page.Locator(".main-nav-item-notes").ClickAsync();

            try
            {
                await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*New Folder.*") }).ClickAsync();
                var modal = page.Locator(".modal-overlay");
                await modal.Locator(".form-input").FillAsync(folderTitle);
                await modal.Locator(".btn-submit").ClickAsync();

                await Task.Delay(1000);

                var folderList = page.Locator(".folder-list");
                var folderCard = folderList.Locator(".folder-item").Filter(new() { HasTextString = folderTitle });

                await folderCard.HoverAsync();
                await folderCard.Locator(".folder-action-btn").Nth(0).WaitForAsync(new() { State = WaitForSelectorState.Visible });
                await folderCard.Locator(".folder-action-btn").Nth(0).ClickAsync(new LocatorClickOptions { Force = true });

                modal = page.Locator(".modal-overlay");
                await modal.Locator(".form-input").FillAsync(folderTitleEdited);
                await modal.Locator(".btn-submit").ClickAsync();

                await Assertions.Expect(folderList.Locator(".folder-item").Filter(new() { HasTextString = folderTitleEdited })).ToHaveCountAsync(1);
            }
            finally
            {
                await DeleteFolder(folderTitleEdited);
            }
        }

        //--- 14

        //[Ignore("")]
        [Test]
        public async Task EditFolder_EmptyTitle()
        {
            string folderTitle = "Test folder 1";
            string folderTitleEdited = "";

            await page.Locator(".main-nav-item-notes").ClickAsync();

            try
            {
                await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*New Folder.*") }).ClickAsync();
                var modal = page.Locator(".modal-overlay");
                await modal.Locator(".form-input").FillAsync(folderTitle);
                await modal.Locator(".btn-submit").ClickAsync();

                await Task.Delay(1000);

                var folderList = page.Locator(".folder-list");
                var folderCard = folderList.Locator(".folder-item").Filter(new() { HasTextString = folderTitle });

                await folderCard.HoverAsync();
                await folderCard.Locator(".folder-action-btn").Nth(0).WaitForAsync(new() { State = WaitForSelectorState.Visible });
                await folderCard.Locator(".folder-action-btn").Nth(0).ClickAsync(new LocatorClickOptions { Force = true });

                modal = page.Locator(".modal-overlay");
                await modal.Locator(".form-input").FillAsync(folderTitleEdited);
                await modal.Locator(".btn-submit").ClickAsync();

                await Assertions.Expect(modal).ToBeVisibleAsync();
            }
            finally
            {
                await DeleteFolder(folderTitle);
            }
        }

        //--- 15

        //[Ignore("")]
        [Test]
        public async Task DeleteFolder_Success()
        {
            string folderTitle = "Test folder delete";

            await page.Locator(".main-nav-item-notes").ClickAsync();

            await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*New Folder.*") }).ClickAsync();
            var modal = page.Locator(".modal-overlay");
            await modal.Locator(".form-input").FillAsync(folderTitle);
            await modal.Locator(".btn-submit").ClickAsync();

            await Task.Delay(1000);

            await DeleteFolder(folderTitle);

            var folderList = page.Locator(".folder-list");
            await Assertions.Expect(folderList.Locator(".folder-item").Filter(new() { HasTextString = folderTitle })).ToHaveCountAsync(0);
        }

        //--- 16

        //[Ignore("")]
        [Test]
        public async Task DeleteFolder_Cancel()
        {
            string folderTitle = "Test folder cancel delete";

            await page.Locator(".main-nav-item-notes").ClickAsync();

            try
            {
                await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*New Folder.*") }).ClickAsync();
                var modal = page.Locator(".modal-overlay");
                await modal.Locator(".form-input").FillAsync(folderTitle);
                await modal.Locator(".btn-submit").ClickAsync();

                await Task.Delay(1000);

                var folderList = page.Locator(".folder-list");
                var folderCard = folderList.Locator(".folder-item").Filter(new() { HasTextString = folderTitle });

                await folderCard.HoverAsync();
                await folderCard.Locator(".folder-action-btn").Nth(1).WaitForAsync(new() { State = WaitForSelectorState.Visible });
                await folderCard.Locator(".folder-action-btn").Nth(1).ClickAsync(new LocatorClickOptions { Force = true });

                await page.Locator(".modal-overlay .btn-cancel").ClickAsync();

                await Assertions.Expect(folderList.Locator(".folder-item").Filter(new() { HasTextString = folderTitle })).ToHaveCountAsync(1);
            }
            finally
            {
                await DeleteFolder(folderTitle);
            }
        }

        //--- 17

        //[Ignore("")]
        [Test]
        public async Task CreateNoteInFolder_Success()
        {
            string folderTitle = "Test folder for note";
            string noteTitle = "Note inside folder";
            string noteDescription = "Opis note u folderu";
            string noteContent = "Sadrzaj note u folderu";
            int noteIndex = 1;

            await page.Locator(".main-nav-item-notes").ClickAsync();

            try
            {
                await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*New Folder.*") }).ClickAsync();
                var modal = page.Locator(".modal-overlay");
                await modal.Locator(".form-input").FillAsync(folderTitle);
                await modal.Locator(".btn-submit").ClickAsync();
                await Task.Delay(1000);

                var folderList = page.Locator(".folder-list");
                var folderItem = folderList.Locator(".folder-item").Filter(new() { HasTextString = folderTitle });
                await folderItem.ClickAsync();

                await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*New Note.*") }).ClickAsync();
                await Assertions.Expect(page.Locator(".modal-overlay")).ToBeVisibleAsync();
                await FillNoteModal(noteTitle, noteDescription, noteContent, noteIndex);
                await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Create Note.*") }).ClickAsync();
                await Task.Delay(1000);

                var card = page.Locator(".note-card").Filter(new() { HasTextString = noteTitle });
                await Assertions.Expect(card).ToBeVisibleAsync();
            }
            finally
            {
                await DeleteFolder(folderTitle);
            }
        }

        //--- 18

        //[Ignore("")]
        [Test]
        public async Task NoteInFolder_NotVisibleInOtherFolder()
        {
            string folder1Title = "Folder one";
            string folder2Title = "Folder two";
            string noteTitle = "Note only in folder one";
            string noteDescription = "Opis";
            string noteContent = "Sadrzaj";
            int noteIndex = 1;

            await page.Locator(".main-nav-item-notes").ClickAsync();

            try
            {
                await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*New Folder.*") }).ClickAsync();
                var modal = page.Locator(".modal-overlay");
                await modal.Locator(".form-input").FillAsync(folder1Title);
                await modal.Locator(".btn-submit").ClickAsync();
                await Task.Delay(1000);

                await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*New Folder.*") }).ClickAsync();
                modal = page.Locator(".modal-overlay");
                await modal.Locator(".form-input").FillAsync(folder2Title);
                await modal.Locator(".btn-submit").ClickAsync();
                await Task.Delay(1000);

                var folderList = page.Locator(".folder-list");
                await folderList.Locator(".folder-item").Filter(new() { HasTextString = folder1Title }).ClickAsync();

                await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*New Note.*") }).ClickAsync();
                await Assertions.Expect(page.Locator(".modal-overlay")).ToBeVisibleAsync();
                await FillNoteModal(noteTitle, noteDescription, noteContent, noteIndex);
                await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Create Note.*") }).ClickAsync();
                await Task.Delay(1000);

                await folderList.Locator(".folder-item").Filter(new() { HasTextString = folder2Title }).ClickAsync();

                var card = page.Locator(".note-card").Filter(new() { HasTextString = noteTitle });
                await Assertions.Expect(card).ToHaveCountAsync(0);
            }
            finally
            {
                await DeleteFolder(folder1Title);
                await DeleteFolder(folder2Title);
            }
        }

        //--- 19

        //[Ignore("")]
        [Test]
        public async Task NoteInFolder_VisibleInAllNotes()
        {
            string folderTitle = "Folder for visibility test";
            string noteTitle = "Note that should appear in All Notes";
            string noteDescription = "Opis";
            string noteContent = "Sadrzaj";
            int noteIndex = 1;

            await page.Locator(".main-nav-item-notes").ClickAsync();

            try
            {
                await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*New Folder.*") }).ClickAsync();
                var modal = page.Locator(".modal-overlay");
                await modal.Locator(".form-input").FillAsync(folderTitle);
                await modal.Locator(".btn-submit").ClickAsync();
                await Task.Delay(1000);

                var folderList = page.Locator(".folder-list");
                await folderList.Locator(".folder-item").Filter(new() { HasTextString = folderTitle }).ClickAsync();

                await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*New Note.*") }).ClickAsync();
                await Assertions.Expect(page.Locator(".modal-overlay")).ToBeVisibleAsync();
                await FillNoteModal(noteTitle, noteDescription, noteContent, noteIndex);
                await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Create Note.*") }).ClickAsync();
                await Task.Delay(1000);

                await folderList.Locator(".folder-item").First.ClickAsync();

                var card = page.Locator(".note-card").Filter(new() { HasTextString = noteTitle });
                await Assertions.Expect(card).ToBeVisibleAsync();
            }
            finally
            {
                await DeleteFolder(folderTitle);
            }
        }

        //--- 20

        //[Ignore("")]
        [Test]
        public async Task DeleteFolder_AlsoDeletesNoteInside()
        {
            string folderTitle = "Folder with note to cascade delete";
            string noteTitle = "Note that should be cascade deleted";
            string noteDescription = "Opis";
            string noteContent = "Sadrzaj";
            int noteIndex = 1;

            await page.Locator(".main-nav-item-notes").ClickAsync();

            await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*New Folder.*") }).ClickAsync();
            var modal = page.Locator(".modal-overlay");
            await modal.Locator(".form-input").FillAsync(folderTitle);
            await modal.Locator(".btn-submit").ClickAsync();
            await Task.Delay(1000);

            var folderList = page.Locator(".folder-list");
            await folderList.Locator(".folder-item").Filter(new() { HasTextString = folderTitle }).ClickAsync();

            await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*New Note.*") }).ClickAsync();
            await Assertions.Expect(page.Locator(".modal-overlay")).ToBeVisibleAsync();
            await FillNoteModal(noteTitle, noteDescription, noteContent, noteIndex);
            await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Create Note.*") }).ClickAsync();
            await Task.Delay(1000);

            await DeleteFolder(folderTitle);
            await Task.Delay(500);

            await page.Locator(".folder-list").Locator(".folder-item").First.ClickAsync();

            var card = page.Locator(".note-card").Filter(new() { HasTextString = noteTitle });
            await Assertions.Expect(card).ToHaveCountAsync(0);
        }

    }
}