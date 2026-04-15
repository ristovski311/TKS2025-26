using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;
using NUnit.Framework;

namespace FrontendTest
{
    [TestFixture]
    [NonParallelizable]
    public class ProfessorsPageTests
    {
        IBrowser? browser;
        IPlaywright? playwright;
        IPage? page;
        IBrowserContext? context;

        string uniqueId = "";
        string testUsername = "";
        string testEmail = "";
        string testPassword = "TestPass123!";
        string testFirstName = "Test";
        string testLastName = "User";
        string testSemester = "3";
        string testPhone = "0600000000";

        string profFirstName = "John";
        string profLastName = "Doe";
        string profEmail = "john.doe@example.com";
        string profPhone = "0611234567";
        string profOffice = "101";

        static readonly LocatorWaitForOptions WaitVisible = new() { State = WaitForSelectorState.Visible, Timeout = 10_000 };
        static readonly LocatorWaitForOptions WaitHidden = new() { State = WaitForSelectorState.Hidden, Timeout = 10_000 };

        [SetUp]
        public async Task Setup()
        {
            uniqueId = Guid.NewGuid().ToString("N").Substring(0, 8);
            testUsername = $"profuser_{uniqueId}";
            testEmail = $"profuser_{uniqueId}@example.com";

            playwright = await Playwright.CreateAsync();
            browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false,
                SlowMo = 100
            });

            context = await browser.NewContextAsync();
            page = await context.NewPageAsync();

            await page.GotoAsync("http://127.0.0.1:5500/index.html");

            await page.Locator(".auth-link").ClickAsync();
            await page.Locator(".register-username").FillAsync(testUsername);
            await page.Locator(".register-email").FillAsync(testEmail);
            await page.Locator(".register-pass").FillAsync(testPassword);
            await page.Locator(".register-first-name").FillAsync(testFirstName);
            await page.Locator(".register-last-name").FillAsync(testLastName);
            await page.Locator(".register-semester").FillAsync(testSemester);
            await page.Locator(".register-phone").FillAsync(testPhone);
            await page.Locator(".auth-button").ClickAsync();

            await Assertions.Expect(page.GetByRole(AriaRole.Heading, new() { NameString = "Login" }))
                            .ToBeVisibleAsync(new() { Timeout = 15_000 });

            await page.Locator(".login-email").FillAsync(testEmail);
            await page.Locator(".login-pass").FillAsync(testPassword);
            await page.Locator(".auth-button").ClickAsync();
            await Assertions.Expect(page).ToHaveTitleAsync(new Regex(".*NoteIT!.*"), new() { Timeout = 15_000 });

            await page.Locator(".main-nav-item-professors").ClickAsync();
            await Assertions.Expect(page.Locator(".page-title")).ToHaveTextAsync("My Professors", new() { Timeout = 10_000 });
            await page.Locator(".courses-grid").WaitForAsync(WaitVisible);
        }

        [TearDown]
        public async Task Cleanup()
        {
            await page!.GotoAsync("http://127.0.0.1:5500/index.html");

            await page.Locator(".main-nav-item-professors").WaitForAsync(WaitVisible);

            var deleteBtn = page.Locator(".delete-button");
            try
            {
                await deleteBtn.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 5_000 });
                await deleteBtn.ClickAsync();
                await page.Locator(".modal-overlay").WaitForAsync(WaitVisible);
                await page.Locator(".btn-submit").ClickAsync();
                await page.Locator(".modal-overlay").WaitForAsync(WaitHidden);

                await page.Locator(".loading-overlay").WaitForAsync(WaitHidden);
            }
            catch (TimeoutException) {  }

            if (page != null) await page.CloseAsync();
            if (context != null) await context.CloseAsync();
            if (browser != null) await browser.CloseAsync();
            if (playwright != null) playwright.Dispose();
        }

        public async Task FillProfessorModal(string firstName, string lastName,
                                              string email, string phone, string office)
        {
            var firstNameInput = page!.GetByPlaceholder(new Regex("First name"));
            await Assertions.Expect(firstNameInput).ToBeEditableAsync(new() { Timeout = 5_000 });

            await firstNameInput.FillAsync(firstName);
            await page.GetByPlaceholder(new Regex("Last name")).FillAsync(lastName);
            await page.GetByPlaceholder(new Regex(".*email@.*")).FillAsync(email);
            await page.Locator("[name='phone']").FillAsync(phone);
            await page.Locator("[name='office']").FillAsync(office);
        }

        public async Task OpenAddProfessorModal()
        {
            await page!.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Add Professor.*") }).ClickAsync();
            await page.Locator(".modal-overlay").WaitForAsync(WaitVisible);
        }
        public async Task DeleteProfessor(string firstName, string lastName)
        {
            await page!.Locator(".courses-grid").WaitForAsync(WaitVisible);

            while (await page.Locator(".modal-overlay").CountAsync() > 0)
            {
                var overlay = page.Locator(".modal-overlay").Last;
                var closeBtn = overlay.Locator(".modal-close");

                bool closeVisible = false;
                try
                {
                    await closeBtn.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 2_000 });
                    closeVisible = true;
                }
                catch (TimeoutException) { }

                if (closeVisible)
                {
                    await closeBtn.ClickAsync();
                    await overlay.WaitForAsync(new() { State = WaitForSelectorState.Hidden, Timeout = 5_000 });
                }
                else
                {
                    break;
                }
            }

            string fullName = $"Prof. {firstName} {lastName}";
            var card = page.Locator(".course-card").Filter(new() { HasTextString = fullName });

            await card.WaitForAsync(WaitVisible);
            await card.HoverAsync();

            var deleteActionBtn = card.Locator(".course-action-btn").Nth(1);
            await deleteActionBtn.WaitForAsync(WaitVisible);
            await deleteActionBtn.ClickAsync();

            var confirmModal = page.Locator(".modal-overlay");
            await confirmModal.WaitForAsync(WaitVisible);
            await confirmModal.Locator(".btn-submit").ClickAsync();

            await page.Locator(".modal-overlay").WaitForAsync(WaitHidden);
            await card.WaitForAsync(new() { State = WaitForSelectorState.Hidden, Timeout = 10_000 });
        }

        //--- 1

        [Test]
        public async Task NavigateToProfessorsPage()
        {
            await Assertions.Expect(page!.Locator(".page-title")).ToHaveTextAsync("My Professors");
        }

        //--- 2

        [Test]
        public async Task ProfessorsPage_GridIsVisible()
        {
            await Assertions.Expect(page!.Locator(".courses-grid")).ToBeVisibleAsync();
        }

        //--- 3

        [Test]
        public async Task ProfessorsPage_EmptyStateMessageDisplayed()
        {
            await Assertions.Expect(page!.Locator(".empty-message")).ToBeVisibleAsync();
        }

        //--- 4

        [Test]
        public async Task AddProfessor_OpenModal()
        {
            await OpenAddProfessorModal();
            await Assertions.Expect(page!.Locator(".modal-overlay")).ToBeVisibleAsync();
        }

        //--- 5

        [Test]
        public async Task AddProfessor_ModalHasCorrectTitle()
        {
            await OpenAddProfessorModal();
            await Assertions.Expect(page!.Locator(".modal-title")).ToHaveTextAsync("Add New Professor");
        }

        //--- 6

        [Test]
        public async Task AddProfessor_CloseModal()
        {
            await OpenAddProfessorModal();
            await page!.Locator(".modal-close").ClickAsync();
            await page.Locator(".modal-overlay").WaitForAsync(WaitHidden);
            await Assertions.Expect(page.Locator(".modal-overlay")).ToBeHiddenAsync();
        }

        //--- 7

        [Test]
        public async Task AddProfessor_Success()
        {
            try
            {
                await OpenAddProfessorModal();
                await FillProfessorModal(profFirstName, profLastName, profEmail, profPhone, profOffice);
                await page!.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Add professor.*") }).ClickAsync();

                await page.Locator(".modal-overlay").WaitForAsync(WaitHidden);

                var newCard = page.Locator(".course-card")
                                  .Filter(new() { HasTextString = $"Prof. {profFirstName} {profLastName}" });
                await newCard.WaitForAsync(WaitVisible);
                await Assertions.Expect(newCard).ToBeVisibleAsync();
            }
            finally
            {
                await DeleteProfessor(profFirstName, profLastName);
            }
        }

        //--- 8

        [Test]
        public async Task AddProfessor_EmptyFirstName()
        {
            await OpenAddProfessorModal();
            await FillProfessorModal("", profLastName, profEmail, profPhone, profOffice);
            await page!.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Add professor.*") }).ClickAsync();

            await Assertions.Expect(page.Locator(".modal-overlay")).ToBeVisibleAsync();

            bool isInvalid = !await page.GetByPlaceholder(new Regex("First name"))
                                        .EvaluateAsync<bool>("el => el.validity.valid");
            Assert.That(isInvalid, Is.True);
        }

        //--- 9

        [Test]
        public async Task AddProfessor_EmptyLastName()
        {
            await OpenAddProfessorModal();
            await FillProfessorModal(profFirstName, "", profEmail, profPhone, profOffice);
            await page!.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Add professor.*") }).ClickAsync();

            await Assertions.Expect(page.Locator(".modal-overlay")).ToBeVisibleAsync();

            bool isInvalid = !await page.GetByPlaceholder(new Regex("Last name"))
                                        .EvaluateAsync<bool>("el => el.validity.valid");
            Assert.That(isInvalid, Is.True);
        }

        //--- 10

        [Test]
        public async Task AddProfessor_EmptyEmail()
        {
            await OpenAddProfessorModal();
            await FillProfessorModal(profFirstName, profLastName, "", profPhone, profOffice);
            await page!.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Add professor.*") }).ClickAsync();

            await Assertions.Expect(page.Locator(".modal-overlay")).ToBeVisibleAsync();

            bool isInvalid = !await page.GetByPlaceholder(new Regex(".*email@.*"))
                                        .EvaluateAsync<bool>("el => el.validity.valid");
            Assert.That(isInvalid, Is.True);
        }

        //--- 11

        [Test]
        public async Task AddProfessor_Cancel()
        {
            await OpenAddProfessorModal();
            await FillProfessorModal(profFirstName, profLastName, profEmail, profPhone, profOffice);

            await page!.Locator(".modal-overlay").Locator(".btn-cancel").ClickAsync();
            await page.Locator(".modal-overlay").WaitForAsync(WaitHidden);

            await Assertions.Expect(page.Locator(".modal-overlay")).ToBeHiddenAsync();

            await Assertions.Expect(
                page.Locator(".course-card")
                    .Filter(new() { HasTextString = $"Prof. {profFirstName} {profLastName}" }))
                .ToHaveCountAsync(0, new() { Timeout = 5_000 });
        }

        //--- 12

        [Test]
        public async Task AddProfessor_CardShowsCorrectInfo()
        {
            try
            {
                await OpenAddProfessorModal();
                await FillProfessorModal(profFirstName, profLastName, profEmail, profPhone, profOffice);
                await page!.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Add professor.*") }).ClickAsync();
                await page.Locator(".modal-overlay").WaitForAsync(WaitHidden);

                var card = page.Locator(".course-card")
                               .Filter(new() { HasTextString = $"Prof. {profFirstName} {profLastName}" });
                await card.WaitForAsync(WaitVisible);

                await Assertions.Expect(card.Locator(".course-professor")).ToContainTextAsync(profEmail);
                await Assertions.Expect(card.Locator(".course-semester")).ToContainTextAsync(profPhone);
                await Assertions.Expect(card.Locator(".course-description")).ToContainTextAsync(profOffice);
            }
            finally
            {
                await DeleteProfessor(profFirstName, profLastName);
            }
        }

        //--- 13

        [Test]
        public async Task EditProfessor_OpenModal()
        {
            try
            {
                await OpenAddProfessorModal();
                await FillProfessorModal(profFirstName, profLastName, profEmail, profPhone, profOffice);
                await page!.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Add professor.*") }).ClickAsync();
                await page.Locator(".modal-overlay").WaitForAsync(WaitHidden);

                var card = page.Locator(".course-card")
                               .Filter(new() { HasTextString = $"Prof. {profFirstName} {profLastName}" });
                await card.WaitForAsync(WaitVisible);
                await card.ClickAsync();

                await page.Locator(".modal-overlay").WaitForAsync(WaitVisible);
                await Assertions.Expect(page.Locator(".modal-title")).ToHaveTextAsync("Edit Professor Data");
            }
            finally
            {
                await DeleteProfessor(profFirstName, profLastName);
            }
        }

        //--- 14

        [Test]
        public async Task EditProfessor_NameChanged()
        {
            string editedFirstName = "Jane";
            string editedLastName = "Smith";
            try
            {
                await OpenAddProfessorModal();
                await FillProfessorModal(profFirstName, profLastName, profEmail, profPhone, profOffice);
                await page!.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Add professor.*") }).ClickAsync();
                await page.Locator(".modal-overlay").WaitForAsync(WaitHidden);

                var card = page.Locator(".course-card")
                               .Filter(new() { HasTextString = $"Prof. {profFirstName} {profLastName}" });
                await card.WaitForAsync(WaitVisible);
                await card.ClickAsync();

                await page.Locator(".modal-overlay").WaitForAsync(WaitVisible);

                var firstNameInput = page.GetByPlaceholder(new Regex("First name"));
                await Assertions.Expect(firstNameInput).ToBeEditableAsync(new() { Timeout = 5_000 });
                await firstNameInput.FillAsync(editedFirstName);
                await page.GetByPlaceholder(new Regex("Last name")).FillAsync(editedLastName);

                await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Save changes.*") }).ClickAsync();
                await page.Locator(".modal-overlay").WaitForAsync(WaitHidden);

                var updatedCard = page.Locator(".course-card")
                                      .Filter(new() { HasTextString = $"Prof. {editedFirstName} {editedLastName}" });
                await updatedCard.WaitForAsync(WaitVisible);
                await Assertions.Expect(updatedCard).ToBeVisibleAsync();
            }
            finally
            {
                await DeleteProfessor(editedFirstName, editedLastName);
            }
        }

        //--- 15

        [Test]
        public async Task DeleteProfessor_Success()
        {
            await OpenAddProfessorModal();
            await FillProfessorModal(profFirstName, profLastName, profEmail, profPhone, profOffice);
            await page!.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Add professor.*") }).ClickAsync();
            await page.Locator(".modal-overlay").WaitForAsync(WaitHidden);

            await page.Locator(".course-card")
                      .Filter(new() { HasTextString = $"Prof. {profFirstName} {profLastName}" })
                      .WaitForAsync(WaitVisible);

            await DeleteProfessor(profFirstName, profLastName);

            await Assertions.Expect(
                page.Locator(".course-card")
                    .Filter(new() { HasTextString = $"Prof. {profFirstName} {profLastName}" }))
                .ToHaveCountAsync(0);
        }

        //--- 16

        [Test]
        public async Task DeleteProfessor_CancelConfirmation()
        {
            try
            {
                await OpenAddProfessorModal();
                await FillProfessorModal(profFirstName, profLastName, profEmail, profPhone, profOffice);
                await page!.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Add professor.*") }).ClickAsync();
                await page.Locator(".modal-overlay").WaitForAsync(WaitHidden);

                var card = page.Locator(".course-card")
                               .Filter(new() { HasTextString = $"Prof. {profFirstName} {profLastName}" });
                await card.WaitForAsync(WaitVisible);
                await card.HoverAsync();

                var deleteActionBtn = card.Locator(".course-action-btn").Nth(1);
                await deleteActionBtn.WaitForAsync(WaitVisible);
                await deleteActionBtn.ClickAsync();

                var confirmModal = page.Locator(".modal-overlay");
                await confirmModal.WaitForAsync(WaitVisible);
                await confirmModal.Locator(".btn-cancel").ClickAsync();
                await page.Locator(".modal-overlay").WaitForAsync(WaitHidden);

                await card.WaitForAsync(WaitVisible);
                await Assertions.Expect(card).ToBeVisibleAsync();
            }
            finally
            {
                await DeleteProfessor(profFirstName, profLastName);
            }
        }

        //--- 17

        [Test]
        public async Task AddMultipleProfessors_AllCardsVisible()
        {
            string secondFirstName = "Alice";
            string secondLastName = "Brown";
            string secondEmail = "alice.brown@example.com";

            try
            {
                await OpenAddProfessorModal();
                await FillProfessorModal(profFirstName, profLastName, profEmail, profPhone, profOffice);
                await page!.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Add professor.*") }).ClickAsync();
                await page.Locator(".modal-overlay").WaitForAsync(WaitHidden);
                await page.Locator(".course-card")
                          .Filter(new() { HasTextString = $"Prof. {profFirstName} {profLastName}" })
                          .WaitForAsync(WaitVisible);

                await OpenAddProfessorModal();
                await FillProfessorModal(secondFirstName, secondLastName, secondEmail, "0621234567", "202");
                await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Add professor.*") }).ClickAsync();
                await page.Locator(".modal-overlay").WaitForAsync(WaitHidden);
                await page.Locator(".course-card")
                          .Filter(new() { HasTextString = $"Prof. {secondFirstName} {secondLastName}" })
                          .WaitForAsync(WaitVisible);

                await Assertions.Expect(page.Locator(".course-card")).ToHaveCountAsync(2, new() { Timeout = 10_000 });

                await Assertions.Expect(
                    page.Locator(".course-card")
                        .Filter(new() { HasTextString = $"Prof. {profFirstName} {profLastName}" }))
                    .ToBeVisibleAsync();

                await Assertions.Expect(
                    page.Locator(".course-card")
                        .Filter(new() { HasTextString = $"Prof. {secondFirstName} {secondLastName}" }))
                    .ToBeVisibleAsync();
            }
            finally
            {
                await DeleteProfessor(profFirstName, profLastName);
                await DeleteProfessor(secondFirstName, secondLastName);
            }
        }
    }
}