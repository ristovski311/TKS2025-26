using Microsoft.Playwright;
using NUnit.Framework;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FrontendTest
{
    [TestFixture]
    public class LoginPageTests
    {
        IBrowser? browser;
        IPlaywright? playwright;
        IPage? page;
        IBrowserContext? context;

        string userMail;
        string username;
        string userFirstname;
        string userLastname;
        string userPass;
        int userSemester;
        string userPhone;

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

            DateTime now = DateTime.Now;

            userMail = $"test_{now:yyyyMMddHHmmss}@example.com";
            username = $"test_{now:yyyyMMddHHmmss}";
            userFirstname = $"test_{now:yyyyMMddHHmmss}_FN";
            userLastname = $"test_{now:yyyyMMddHHmmss}_LN";
            userPass = $"test_{now:yyyyMMddHHmmss}_pass";
            userSemester = 1;
            userPhone = "1234";

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
        }

        [TearDown]
        public async Task Cleanup()
        {
            if (page != null) await page.CloseAsync();
            if (context != null) await context.CloseAsync();
            if (browser != null) await browser.CloseAsync();
            if (playwright != null) playwright.Dispose();
        }

        [Test]

        public async Task SuccessfulLogin()
        {
            try
            {
                await page.Locator(".login-email").FillAsync(userMail);
                await page.Locator(".login-pass").FillAsync(userPass);
                await page.Locator(".toggle-password").ClickAsync();
                await page.Locator(".auth-button").ClickAsync();

                await Assertions.Expect(page).ToHaveTitleAsync(new Regex(".*NoteIT!.*"));

            }
            finally
            {
                await page.Locator(".delete-button").ClickAsync();
                await page.Locator(".modal-overlay .btn-submit").ClickAsync();
            }
        }

        //---

        [Test]
        public async Task UnsuccessfulLogin_WrongCredentials()
        {
            await page.Locator(".login-email").FillAsync("random@example.com");
            await page.Locator(".login-pass").FillAsync("test123.");
            await page.Locator(".toggle-password").ClickAsync();
            await page.Locator(".auth-button").ClickAsync();

            await Assertions.Expect(page.Locator(".auth-error")).ToBeVisibleAsync();
        }

        //---

        [Test]
        public async Task UnsuccessfulLogin_NoEmail()
        {
            await page.Locator(".login-pass").FillAsync(userPass);
            await page.Locator(".toggle-password").ClickAsync();
            await page.Locator(".auth-button").ClickAsync();

            await Assertions.Expect(page.Locator(".auth-error")).ToHaveTextAsync(new Regex("Neuspesan login"));
        }

        //---

        [Test]
        public async Task UnsuccessfulLogin_NoPassword()
        {
            await page.Locator(".login-email").FillAsync(userMail);
            await page.Locator(".auth-button").ClickAsync();

            await Assertions.Expect(page.Locator(".auth-error")).ToHaveTextAsync(new Regex("Neuspesan login"));
        }

        //---

        [Test]
        public async Task UnsuccessfulLogin_NoCredentials()
        {
            await page.Locator(".auth-button").ClickAsync(new() { Force = true });

            await Assertions.Expect(page.Locator(".auth-error")).ToHaveTextAsync(new Regex(".*Neuspesan login.*"));
        }

        //---

        [Test]
        public async Task LoadRegisterPage()
        {
            await page.Locator(".auth-link").ClickAsync();

            await Assertions.Expect(page.GetByRole(AriaRole.Heading, new() {NameString = "Register" })).ToBeVisibleAsync();
        }
    }
}