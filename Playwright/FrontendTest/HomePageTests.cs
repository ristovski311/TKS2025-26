using Microsoft.Playwright;
using NUnit.Framework;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FrontendTest
{
    [TestFixture]
    public class HomePageTests
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

        [Test]
        public async Task CorrectUsernameGreeting()
        {
            await Assertions.Expect(page.Locator(".greeting-text")).ToHaveTextAsync(new Regex(".*ristovski311.*"));
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
            await page.Locator(".logout-button").ClickAsync();
            await page.Locator(".btn-submit").ClickAsync();
            await Assertions.Expect(page.GetByRole(AriaRole.Heading, new() { NameRegex = new Regex(".*Login.*") })).ToBeVisibleAsync();
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