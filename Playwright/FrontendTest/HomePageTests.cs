using Microsoft.Playwright;
using NUnit.Framework;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FrontendTest
{
    [TestFixture]
    [NonParallelizable]
    public class HomePageTests
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
            await page.Locator(".auth-link").ClickAsync();
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
        }

        [TearDown]
        public async Task Cleanup()
        {
            await page.Locator(".delete-button").ClickAsync();
            await page.Locator(".modal-overlay .btn-submit").ClickAsync();
            //await Assertions.Expect(page.Locator(".loading-overlay")).ToBeHiddenAsync();

            if (page != null) await page.CloseAsync();
            if (context != null) await context.CloseAsync();
            if (browser != null) await browser.CloseAsync();
            if (playwright != null) playwright.Dispose();
        }

        [Test]
        public async Task CorrectUsernameGreeting()
        {
            await Assertions.Expect(page.Locator(".greeting-text")).ToHaveTextAsync(new Regex($".*{username}.*"));
        }

        //----

        [Test]
        public async Task ImageVisible()
        {
            await Assertions.Expect(page.GetByAltText("illustration")).ToBeVisibleAsync();
        }

        //---

        [Test]
        public async Task LogoutSubmit()
        {
            try
            {
                await page.Locator(".logout-button").ClickAsync();
                await page.Locator(".btn-submit").ClickAsync();
                await Assertions.Expect(page.GetByRole(AriaRole.Heading, new() { NameRegex = new Regex(".*Login.*") })).ToBeVisibleAsync();

            }
            finally
            {
                await page.Locator(".login-email").FillAsync(userMail);
                await page.Locator(".login-pass").FillAsync(userPass);
                await page.Locator(".toggle-password").ClickAsync();
                await page.Locator(".auth-button").ClickAsync();
            }

        }

        //---

        [Test]
        public async Task LogoutCancel()
        {
            await page.Locator(".logout-button").ClickAsync();
            await page.Locator(".btn-cancel").ClickAsync();
            await Assertions.Expect(page.GetByText(new Regex(".*Let's note it!.*"))).ToBeVisibleAsync();
        }

        //---

        [Test]
        public async Task NavigateToHomePage_ViaLogoImage()
        {
            await page.Locator(".main-nav-item-professors").ClickAsync();
            await page.Locator(".app-logo").ClickAsync();
            await Assertions.Expect(page.GetByText(new Regex(".*Let's note it!.*"))).ToBeVisibleAsync();
        }

        //---

        [Test]
        public async Task NavigateToHomePage_ViaNavbar()
        {
            await page.Locator(".main-nav-item-professors").ClickAsync();
            await page.Locator(".main-nav-item-home").ClickAsync();
            await Assertions.Expect(page.GetByText(new Regex(".*Let's note it!.*"))).ToBeVisibleAsync();
        }

        //---

        [Test]
        public async Task IsDateCorrect()
        {
            string today = DateTime.Now.ToString("dd.MM.yyyy.");
            await Assertions.Expect(page.Locator(".header-date")).ToHaveTextAsync(today);
        }
    }
}