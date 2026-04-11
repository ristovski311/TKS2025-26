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
            await page.Locator(".login-email").FillAsync("ristovski311@gmail.com");
            await page.Locator(".login-pass").FillAsync("banana");
            await page.Locator(".toggle-password").ClickAsync();
            await page.Locator(".auth-button").ClickAsync();

            await Assertions.Expect(page).ToHaveTitleAsync(new Regex(".*NoteIT!.*"));
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
            await page.Locator(".login-pass").FillAsync("banana");
            await page.Locator(".toggle-password").ClickAsync();
            await page.Locator(".auth-button").ClickAsync();

            await Assertions.Expect(page.Locator(".auth-error")).ToHaveTextAsync(new Regex("Neuspesan login"));
        }

        //---

        [Test]
        public async Task UnsuccessfulLogin_NoPassword()
        {
            await page.Locator(".login-email").FillAsync("ristovski311@gmail.com");
            await page.Locator(".auth-button").ClickAsync();

            await Assertions.Expect(page.Locator(".auth-error")).ToHaveTextAsync(new Regex("Neuspesan login"));
        }

        //---

        [Test]
        public async Task UnsuccessfulLogin_NoCredentials()
        {
            await page.Locator(".auth-button").ClickAsync();

            await Assertions.Expect(page.Locator(".auth-error")).ToHaveTextAsync(new Regex("Neuspesan login"));
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