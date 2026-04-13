using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;
using NUnit.Framework;

namespace FrontendTest
{
    [TestFixture]
    [NonParallelizable]
    public class CalendarPageTests
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

        string profFirstName = "Cal";
        string profLastName = "Prof";
        string profEmail = "calprof@example.com";
        string profPhone = "123456";
        string profOffice = "C1";

        string courseTitle = "Cal Test Course";

        string taskTitle = "Test Task";
        string taskType = "Exam";
        string taskDescription = "Test task description";
        string taskDate = "";
        string taskGrade = "100";

        [SetUp]
        public async Task Setup()
        {
            uniqueId = Guid.NewGuid().ToString("N").Substring(0, 8);
            testUsername = $"caluser_{uniqueId}";
            testEmail = $"caluser_{uniqueId}@example.com";
            taskDate = DateTime.Now.ToString("yyyy-MM-dd");

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
                            .ToBeVisibleAsync(new() { Timeout = 15000 });
            await page.Locator(".login-email").FillAsync(testEmail);
            await page.Locator(".login-pass").FillAsync(testPassword);
            await page.Locator(".auth-button").ClickAsync();
            await Assertions.Expect(page).ToHaveTitleAsync(new Regex(".*NoteIT!.*"));

            await page.Locator(".main-nav-item-professors").ClickAsync();
            await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Add Professor.*") }).ClickAsync();
            await page.GetByPlaceholder(new Regex("First name")).FillAsync(profFirstName);
            await page.GetByPlaceholder(new Regex("Last name")).FillAsync(profLastName);
            await page.GetByPlaceholder(new Regex(".*email@.*")).FillAsync(profEmail);
            await page.Locator("[name='phone']").FillAsync(profPhone);
            await page.Locator("[name='office']").FillAsync(profOffice);
            await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Add professor.*") }).ClickAsync();
            await page.Locator(".modal-overlay").WaitForAsync(new() { State = WaitForSelectorState.Hidden });

            await page.Locator(".main-nav-item-courses").ClickAsync();
            await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Add Course.*") }).ClickAsync();
            await page.Locator("[name='title']").FillAsync(courseTitle);
            await page.Locator("[name='semester']").FillAsync("1");
            await page.WaitForSelectorAsync("select[name='professorId'] option:nth-child(2)",
                new PageWaitForSelectorOptions { State = WaitForSelectorState.Attached });
            await page.Locator("[name='professorId']").SelectOptionAsync(new SelectOptionValue { Index = 1 });
            await page.Locator(".form-textarea").FillAsync("Test course for calendar tests");
            await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Create course.*") }).ClickAsync();
            await page.Locator(".modal-overlay").WaitForAsync(new() { State = WaitForSelectorState.Hidden });

            await page.Locator(".main-nav-item-calendar").ClickAsync();
            await Assertions.Expect(page.Locator(".page-title")).ToHaveTextAsync("Calendar");
        }

        [TearDown]
        public async Task Cleanup()
        {
            await page!.GotoAsync("http://127.0.0.1:5500/index.html");

            var deleteBtn = page.Locator(".delete-button");
            try
            {
                await deleteBtn.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 5000 });
                await deleteBtn.ClickAsync();
                await page.Locator(".btn-submit").ClickAsync();
                await page.Locator(".modal-overlay").WaitForAsync(new() { State = WaitForSelectorState.Hidden });
                await Task.Delay(1000);
            }
            catch (TimeoutException)
            {

            }

            if (page != null) await page.CloseAsync();
            if (context != null) await context.CloseAsync();
            if (browser != null) await browser.CloseAsync();
            if (playwright != null) playwright.Dispose();
        }

        public async Task FillTaskModal(string title, string type, string description,
                                        string date, string grade)
        {
            await page!.Locator("[name='title']").FillAsync(title);
            await page.Locator("[name='type']").FillAsync(type);
            await page.Locator("[name='description']").FillAsync(description);
            await page.Locator("[name='date']").FillAsync(date);
            await page.Locator("[name='grade']").FillAsync(grade);

            await page.WaitForSelectorAsync("select[name='courseId'] option:nth-child(2)",
                new PageWaitForSelectorOptions { State = WaitForSelectorState.Attached });
            await page.Locator("[name='courseId']").SelectOptionAsync(new SelectOptionValue { Index = 1 });
        }

        public async Task OpenAddTaskModal()
        {
            await page!.Locator(".fab").ClickAsync();
            await page.Locator(".modal-overlay").WaitForAsync(new() { State = WaitForSelectorState.Visible });
        }

        public async Task DeleteTask(string title)
        {
            // Wait for any ongoing re-renders to settle before checking modals
            await page!.Locator(".calendar-grid").WaitForAsync(new() { State = WaitForSelectorState.Visible });

            // Close any modals that are still open
            int modalCount = await page.Locator(".modal-overlay").CountAsync();
            for (int i = 0; i < modalCount; i++)
            {
                try
                {
                    await page.Locator(".modal-overlay").Last
                              .Locator(".modal-close")
                              .ClickAsync(new() { Timeout = 2000 });
                    await page.Locator(".modal-overlay").Last
                              .WaitForAsync(new() { State = WaitForSelectorState.Hidden, Timeout = 2000 });
                }
                catch (TimeoutException) { break; }
            }

            await page.Locator(".task-pill").Filter(new() { HasTextString = title }).ClickAsync();
            await page.Locator(".modal-overlay").WaitForAsync(new() { State = WaitForSelectorState.Visible });
            await page.Locator(".btn-remove").ClickAsync();

            var confirmModal = page.Locator(".modal-overlay").Last;
            await confirmModal.Locator(".btn-submit").ClickAsync();
            await page.Locator(".modal-overlay").WaitForAsync(new() { State = WaitForSelectorState.Hidden });
        }

        //--- 1

        [Test]
        public async Task NavigateToCalendarPage()
        {
            await Assertions.Expect(page!.Locator(".page-title")).ToHaveTextAsync("Calendar");
        }

        //--- 2

        [Test]
        public async Task CalendarPage_GridIsVisible()
        {
            await Assertions.Expect(page!.Locator(".calendar-grid")).ToBeVisibleAsync();
        }

        //--- 3

        [Test]
        public async Task CalendarPage_CurrentMonthDisplayed()
        {
            var months = new[] { "January", "February", "March", "April", "May", "June",
                                  "July", "August", "September", "October", "November", "December" };
            string expectedMonth = months[DateTime.Now.Month - 1];
            string expectedYear = DateTime.Now.Year.ToString();

            await Assertions.Expect(page!.Locator(".calendar-month-year"))
                            .ToHaveTextAsync(new Regex($".*{expectedMonth}.*{expectedYear}.*"));
        }

        //--- 4

        [Test]
        public async Task CalendarPage_TodayIsHighlighted()
        {
            await Assertions.Expect(page!.Locator(".calendar-day.today")).ToBeVisibleAsync();
        }

        //--- 5

        [Test]
        public async Task CalendarPage_NavigateToNextMonth()
        {
            var currentMonthYear = await page!.Locator(".calendar-month-year").InnerTextAsync();

            await page.Locator(".calendar-nav-btn").Nth(1).ClickAsync();

            await Assertions.Expect(page.Locator(".calendar-month-year"))
                            .Not.ToHaveTextAsync(currentMonthYear);
        }

        //--- 6

        [Test]
        public async Task CalendarPage_NavigateToPreviousMonth()
        {
            var currentMonthYear = await page!.Locator(".calendar-month-year").InnerTextAsync();

            await page.Locator(".calendar-nav-btn").Nth(0).ClickAsync();

            await Assertions.Expect(page.Locator(".calendar-month-year"))
                            .Not.ToHaveTextAsync(currentMonthYear);
        }

        //--- 7

        [Test]
        public async Task CalendarPage_TodayButtonReturnsToCurrentMonth()
        {
            await page!.Locator(".calendar-nav-btn").Nth(1).ClickAsync();
            await page.Locator(".calendar-nav-btn").Nth(1).ClickAsync();

            await page.Locator(".calendar-today-btn").ClickAsync();

            var months = new[] { "January", "February", "March", "April", "May", "June",
                                  "July", "August", "September", "October", "November", "December" };
            string expectedMonth = months[DateTime.Now.Month - 1];
            string expectedYear = DateTime.Now.Year.ToString();

            await Assertions.Expect(page.Locator(".calendar-month-year"))
                            .ToHaveTextAsync(new Regex($".*{expectedMonth}.*{expectedYear}.*"));
        }

        //--- 8

        [Test]
        public async Task AddTask_OpenModal()
        {
            await OpenAddTaskModal();
            await Assertions.Expect(page!.Locator(".modal-overlay")).ToBeVisibleAsync();
        }

        //--- 9

        [Test]
        public async Task AddTask_ModalHasCorrectTitle()
        {
            await OpenAddTaskModal();
            await Assertions.Expect(page!.Locator(".modal-title")).ToHaveTextAsync("Add New Task");
        }

        //--- 10

        [Test]
        public async Task AddTask_CloseModal()
        {
            await OpenAddTaskModal();
            await page!.Locator(".modal-close").ClickAsync();
            await Assertions.Expect(page.Locator(".modal-overlay")).ToBeHiddenAsync();
        }

        //--- 11

        [Test]
        public async Task AddTask_Success()
        {
            try
            {
                await OpenAddTaskModal();
                await FillTaskModal(taskTitle, taskType, taskDescription, taskDate, taskGrade);
                await page!.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Create task.*") }).ClickAsync();

                await page.Locator(".modal-overlay").WaitForAsync(new() { State = WaitForSelectorState.Hidden });

                await Assertions.Expect(page.Locator(".task-pill").Filter(new() { HasTextString = taskTitle }))
                                .ToBeVisibleAsync();
            }
            finally
            {
                await DeleteTask(taskTitle);
            }
        }

        //--- 12

        [Test]
        public async Task AddTask_EmptyTitle()
        {
            await OpenAddTaskModal();
            await FillTaskModal("", taskType, taskDescription, taskDate, taskGrade);
            await page!.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Create task.*") }).ClickAsync();

            await Assertions.Expect(page.Locator(".modal-overlay")).ToBeVisibleAsync();

            bool isInvalid = !await page.Locator("[name='title']")
                                        .EvaluateAsync<bool>("el => el.validity.valid");
            Assert.That(isInvalid, Is.True);
        }

        //--- 13

        [Test]
        public async Task AddTask_Cancel()
        {
            await OpenAddTaskModal();
            await page!.Locator("[name='title']").FillAsync(taskTitle);
            await page.Locator("[name='type']").FillAsync(taskType);
            await page.Locator("[name='description']").FillAsync(taskDescription);
            await page.Locator("[name='date']").FillAsync(taskDate);
            await page.Locator("[name='grade']").FillAsync(taskGrade);

            await page.Locator(".modal-overlay").Locator(".btn-cancel").ClickAsync();

            await Assertions.Expect(page.Locator(".modal-overlay")).ToBeHiddenAsync();

            await Assertions.Expect(page.Locator(".task-pill").Filter(new() { HasTextString = taskTitle }))
                            .ToHaveCountAsync(0, new() { Timeout = 2000 });
        }

        //--- 14

        [Test]
        public async Task EditTask_OpenModal()
        {
            try
            {
                await OpenAddTaskModal();
                await FillTaskModal(taskTitle, taskType, taskDescription, taskDate, taskGrade);
                await page!.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Create task.*") }).ClickAsync();
                await page.Locator(".modal-overlay").WaitForAsync(new() { State = WaitForSelectorState.Hidden });

                await page.Locator(".task-pill").Filter(new() { HasTextString = taskTitle }).ClickAsync();

                await Assertions.Expect(page.Locator(".modal-title")).ToHaveTextAsync("Edit Task");
            }
            finally
            {
                await DeleteTask(taskTitle);
            }
        }

        //--- 15

        [Test]
        public async Task EditTask_TitleChanged()
        {
            string editedTitle = "Edited Task Title";
            try
            {
                await OpenAddTaskModal();
                await FillTaskModal(taskTitle, taskType, taskDescription, taskDate, taskGrade);
                await page!.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Create task.*") }).ClickAsync();
                await page.Locator(".modal-overlay").WaitForAsync(new() { State = WaitForSelectorState.Hidden });

                await page.Locator(".task-pill").Filter(new() { HasTextString = taskTitle }).ClickAsync();
                await page.Locator(".modal-overlay").WaitForAsync(new() { State = WaitForSelectorState.Visible });

                await page.Locator("[name='title']").FillAsync(editedTitle);
                await page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Save changes.*") }).ClickAsync();
                await page.Locator(".modal-overlay").WaitForAsync(new() { State = WaitForSelectorState.Hidden });

                await Assertions.Expect(page.Locator(".task-pill").Filter(new() { HasTextString = editedTitle }))
                                .ToBeVisibleAsync();
            }
            finally
            {
                await DeleteTask(editedTitle);
            }
        }

        //--- 16

        [Test]
        public async Task EditTask_Cancel()
        {
            string editedTitle = "Edited Task Title";
            try
            {
                await OpenAddTaskModal();
                await FillTaskModal(taskTitle, taskType, taskDescription, taskDate, taskGrade);
                await page!.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Create task.*") }).ClickAsync();
                await page.Locator(".modal-overlay").WaitForAsync(new() { State = WaitForSelectorState.Hidden });

                await page.Locator(".task-pill").Filter(new() { HasTextString = taskTitle }).ClickAsync();
                await page.Locator(".modal-overlay").WaitForAsync(new() { State = WaitForSelectorState.Visible });

                await page.Locator("[name='title']").FillAsync(editedTitle);
                await page.Locator(".btn-cancel").ClickAsync();
                await page.Locator(".modal-overlay").WaitForAsync(new() { State = WaitForSelectorState.Hidden });

                await Assertions.Expect(page.Locator(".task-pill").Filter(new() { HasTextString = editedTitle }))
                                .ToHaveCountAsync(0);
            }
            finally
            {
                await DeleteTask(taskTitle);
            }
        }

        //--- 17

        [Test]
        public async Task DeleteTask_Success()
        {
            await OpenAddTaskModal();
            await FillTaskModal(taskTitle, taskType, taskDescription, taskDate, taskGrade);
            await page!.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Create task.*") }).ClickAsync();
            await page.Locator(".modal-overlay").WaitForAsync(new() { State = WaitForSelectorState.Hidden });

            await DeleteTask(taskTitle);

            await Assertions.Expect(page.Locator(".task-pill").Filter(new() { HasTextString = taskTitle }))
                            .ToHaveCountAsync(0);
        }

        //--- 18

        [Test]
        public async Task CompleteTask_Success()
        {
            int earnedGrade = 85;
            await OpenAddTaskModal();
            await FillTaskModal(taskTitle, taskType, taskDescription, taskDate, taskGrade);
            await page!.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Create task.*") }).ClickAsync();
            await page.Locator(".modal-overlay").WaitForAsync(new() { State = WaitForSelectorState.Hidden });

            await page.Locator(".task-pill").Filter(new() { HasTextString = taskTitle }).ClickAsync();
            await page.Locator(".modal-overlay").WaitForAsync(new() { State = WaitForSelectorState.Visible });
            await page.Locator(".btn-pass").ClickAsync();

            var completeModal = page.Locator(".modal-overlay").Nth(1);
            await completeModal.Locator(".form-input").FillAsync(earnedGrade.ToString());
            await completeModal.Locator(".btn-submit").ClickAsync();

            await Assertions.Expect(page.Locator(".modal-overlay")).ToHaveCountAsync(0, new() { Timeout = 10000 });

            await Assertions.Expect(page.Locator(".task-completed")
                                        .Filter(new() { HasTextString = taskTitle }))
                            .ToBeVisibleAsync(new() { Timeout = 5000 });
        }

        //--- 19

        [Test]
        public async Task CompleteTask_Cancel()
        {
            try
            {
                await OpenAddTaskModal();
                await FillTaskModal(taskTitle, taskType, taskDescription, taskDate, taskGrade);
                await page!.GetByRole(AriaRole.Button, new() { NameRegex = new Regex(".*Create task.*") }).ClickAsync();
                await page.Locator(".modal-overlay").WaitForAsync(new() { State = WaitForSelectorState.Hidden });

                await page.Locator(".task-pill").Filter(new() { HasTextString = taskTitle }).ClickAsync();
                await page.Locator(".modal-overlay").WaitForAsync(new() { State = WaitForSelectorState.Visible });
                await page.Locator(".btn-pass").ClickAsync();

                var completeModal = page.Locator(".modal-overlay").Nth(1);
                await completeModal.Locator(".btn-cancel").ClickAsync();

                await Assertions.Expect(page.Locator(".task-pill.task-completed")
                                           .Filter(new() { HasTextString = taskTitle }))
                                .ToHaveCountAsync(0);
            }
            finally
            {
                await DeleteTask(taskTitle);
            }
        }
    }
}