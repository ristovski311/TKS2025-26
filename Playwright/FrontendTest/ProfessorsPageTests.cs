using Microsoft.Playwright;
using NUnit.Framework;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FrontendTest
{
    [TestFixture]
    public class ProfessorsPageTests
    {
        [Test]
        public async Task GoogleSearchTest()
        {
            using var playwright = await Microsoft.Playwright.Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false,
                SlowMo = 1000
            });

            var context = await browser.NewContextAsync();
            var page = await context.NewPageAsync();

            await page.GotoAsync("http://127.0.0.1:5500/index.html");


            await page.Locator(".login-email").FillAsync("ristovski311@gmail.com");
            await page.Locator(".login-pass").FillAsync("banana");
            await page.Locator(".auth-button").ClickAsync();

            await Assertions.Expect(page).ToHaveTitleAsync(new Regex(".*NoteIT!.*"));
            await browser.CloseAsync();
        }
    }
}