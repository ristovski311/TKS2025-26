using Microsoft.Playwright;
using NUnit.Framework;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FrontendTest
{
    [TestFixture]
    public class RegistrationPageTests
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

        public void GenerateTestUser()
        {
            uniqueId = Guid.NewGuid().ToString("N").Substring(0, 8);
            testUsername = $"testuser_{uniqueId}";
            testEmail = $"testuser_{uniqueId}@example.com";
        }

        [SetUp]
        public async Task Setup()
        {
            uniqueId = "";
            testUsername = "";
            testEmail = "";

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
            await Assertions.Expect(page.GetByRole(AriaRole.Heading, new() { NameString = "Register" })).ToBeVisibleAsync();
        }

        [TearDown]
        public async Task Cleanup()
        {
            if (!string.IsNullOrEmpty(testEmail))
            {
                await page!.GotoAsync("http://127.0.0.1:5500/index.html");

                await page.Locator(".login-email").FillAsync(testEmail);
                await page.Locator(".login-pass").FillAsync(testPassword);
                await page.Locator(".auth-button").ClickAsync();

                var deleteBtn = page.Locator(".delete-button");
                try
                {
                    await deleteBtn.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 5000 });
                    await deleteBtn.ClickAsync();
                    await page.Locator(".btn-submit").ClickAsync();
                    await page.Locator(".modal-overlay").WaitForAsync(new() { State = WaitForSelectorState.Hidden });
                }
                catch (TimeoutException)
                {
                    
                }
            }

            if (page != null) await page.CloseAsync();
            if (context != null) await context.CloseAsync();
            if (browser != null) await browser.CloseAsync();
            if (playwright != null) playwright.Dispose();
        }

        public async Task FillRegisterForm(string username, string email, string password,
                                           string firstName, string lastName,
                                           string semester, string phone)
        {
            await page!.Locator(".register-username").FillAsync(username);
            await page.Locator(".register-email").FillAsync(email);
            await page.Locator(".register-pass").FillAsync(password);
            await page.Locator(".register-first-name").FillAsync(firstName);
            await page.Locator(".register-last-name").FillAsync(lastName);
            await page.Locator(".register-semester").FillAsync(semester);
            await page.Locator(".register-phone").FillAsync(phone);
        }

        //--- 1

        [Test]
        public async Task RegisterPage_IsLoaded()
        {
            await Assertions.Expect(page!.GetByRole(AriaRole.Heading, new() { NameString = "Register" })).ToBeVisibleAsync();
        }

        //--- 2

        [Test]
        public async Task RegisterPage_AllFieldsPresent()
        {
            await Assertions.Expect(page!.Locator(".register-username")).ToBeVisibleAsync();
            await Assertions.Expect(page!.Locator(".register-email")).ToBeVisibleAsync();
            await Assertions.Expect(page!.Locator(".register-pass")).ToBeVisibleAsync();
            await Assertions.Expect(page!.Locator(".register-first-name")).ToBeVisibleAsync();
            await Assertions.Expect(page!.Locator(".register-last-name")).ToBeVisibleAsync();
            await Assertions.Expect(page!.Locator(".register-semester")).ToBeVisibleAsync();
            await Assertions.Expect(page!.Locator(".register-phone")).ToBeVisibleAsync();
            await Assertions.Expect(page!.Locator(".auth-button")).ToBeVisibleAsync();
        }

        //--- 3

        [Test]
        public async Task Register_Success()
        {
            GenerateTestUser();
            await FillRegisterForm(testUsername, testEmail, testPassword,
                                   testFirstName, testLastName, testSemester, testPhone);
            await page!.Locator(".auth-button").ClickAsync();

            await Assertions.Expect(page.GetByRole(AriaRole.Heading, new() { NameString = "Login" })).ToBeVisibleAsync();

            await page.Locator(".login-email").FillAsync(testEmail);
            await page.Locator(".login-pass").FillAsync(testPassword);
            await page.Locator(".auth-button").ClickAsync();
            await Assertions.Expect(page).ToHaveTitleAsync(new Regex(".*NoteIT!.*"));
        }

        //--- 4

        [Test]
        public async Task Register_EmptyUsername()
        {
            await FillRegisterForm("", testEmail, testPassword,
                                   testFirstName, testLastName, testSemester, testPhone);
            await page!.Locator(".auth-button").ClickAsync();

            await Assertions.Expect(page.Locator(".auth-error")).ToBeVisibleAsync();
        }

        //--- 5

        [Test]
        public async Task Register_EmptyEmail()
        {
            await FillRegisterForm(testUsername, "", testPassword,
                                   testFirstName, testLastName, testSemester, testPhone);
            await page!.Locator(".auth-button").ClickAsync();

            await Assertions.Expect(page.Locator(".auth-error")).ToBeVisibleAsync();
        }

        //--- 6

        [Test]
        public async Task Register_EmptyPassword()
        {
            await FillRegisterForm(testUsername, testEmail, "",
                                   testFirstName, testLastName, testSemester, testPhone);
            await page!.Locator(".auth-button").ClickAsync();

            await Assertions.Expect(page.Locator(".auth-error")).ToBeVisibleAsync();
        }

        //--- 7

        [Test]
        public async Task Register_EmptyFirstName()
        {
            await FillRegisterForm(testUsername, testEmail, testPassword,
                                   "", testLastName, testSemester, testPhone);
            await page!.Locator(".auth-button").ClickAsync();

            await Assertions.Expect(page.Locator(".auth-error")).ToBeVisibleAsync();
        }

        //--- 8

        [Test]
        public async Task Register_EmptyLastName()
        {
            await FillRegisterForm(testUsername, testEmail, testPassword,
                                   testFirstName, "", testSemester, testPhone);
            await page!.Locator(".auth-button").ClickAsync();

            await Assertions.Expect(page.Locator(".auth-error")).ToBeVisibleAsync();
        }

        //--- 9

        [Test]
        public async Task Register_EmptySemester()
        {
            await FillRegisterForm(testUsername, testEmail, testPassword,
                                   testFirstName, testLastName, "", testPhone);
            await page!.Locator(".auth-button").ClickAsync();

            await Assertions.Expect(page.Locator(".auth-error")).ToBeVisibleAsync();
        }

        //--- 10

        [Test]
        public async Task Register_AllFieldsEmpty()
        {
            await page!.Locator(".auth-button").ClickAsync();

            await Assertions.Expect(page.Locator(".auth-error")).ToBeVisibleAsync();
        }

        //--- 11

        [Test]
        public async Task Register_InvalidEmailFormat()
        {
            await FillRegisterForm(testUsername, "not-an-email", testPassword,
                                   testFirstName, testLastName, testSemester, testPhone);
            await page!.Locator(".auth-button").ClickAsync();

            var isValid = await page.Locator(".register-email").EvaluateAsync<bool>("el => el.validity.valid");
            Assert.That(isValid, Is.False);
        }

        //--- 12

        [Test]
        public async Task Register_TogglePasswordVisibility()
        {
            await page!.Locator(".register-pass").FillAsync(testPassword);

            await Assertions.Expect(page.Locator(".register-pass")).ToHaveAttributeAsync("type", "password");

            await page.Locator(".toggle-password").ClickAsync();

            await Assertions.Expect(page.Locator(".register-pass")).ToHaveAttributeAsync("type", "text");
        }

        //--- 13

        [Test]
        public async Task Register_TogglePasswordVisibility_TogglesBack()
        {
            await page!.Locator(".register-pass").FillAsync(testPassword);
            await page.Locator(".toggle-password").ClickAsync();
            await page.Locator(".toggle-password").ClickAsync();

            await Assertions.Expect(page.Locator(".register-pass")).ToHaveAttributeAsync("type", "password");
        }

        //--- 14

        [Test]
        public async Task Register_NavigateToLoginPage()
        {
            await page!.Locator(".auth-link").ClickAsync();
            await Assertions.Expect(page.GetByRole(AriaRole.Heading, new() { NameString = "Login" })).ToBeVisibleAsync();
        }

        //--- 15

        [Test]
        public async Task Register_ErrorClearsOnNewSubmit()
        {
            await page!.Locator(".auth-button").ClickAsync();
            await Assertions.Expect(page.Locator(".auth-error")).ToBeVisibleAsync();

            GenerateTestUser();
            await FillRegisterForm(testUsername, testEmail, testPassword,
                                   testFirstName, testLastName, testSemester, testPhone);
            await page.Locator(".auth-button").ClickAsync();

            await Assertions.Expect(page.GetByRole(AriaRole.Heading, new() { NameString = "Login" })).ToBeVisibleAsync();

            await page.Locator(".login-email").FillAsync(testEmail);
            await page.Locator(".login-pass").FillAsync(testPassword);
            await page.Locator(".auth-button").ClickAsync();
            await Assertions.Expect(page).ToHaveTitleAsync(new Regex(".*NoteIT!.*"));
        }
    }
}