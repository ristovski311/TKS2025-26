using Backend.Controllers;
using Backend.DTOs;
using Backend.Models;
using Backend.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Supabase;

namespace BackendTest;

[TestFixture]
public class CoursesTest
{
    private CoursesController courseController;
    private CourseRepository courseRep;
    private UserRepository userRep;
    private ProfessorRepository professorRep;
    private Course testCourse;
    private Course testCourse2;
    private User testUser;
    private Professor testProfessor;

    [OneTimeSetUp]
    public async Task GlobalSetup()
    {
        var config = new ConfigurationBuilder()
            .AddUserSecrets<CoursesTest>()
            .Build();

        var supabaseUrl = config["Supabase:Url"] ?? throw new InvalidOperationException("Supabase URL is not configured.");
        var supabaseKey = config["Supabase:Key"] ?? throw new InvalidOperationException("Supabase Key is not configured.");

        var client = new Client(supabaseUrl, supabaseKey);
        courseRep = new CourseRepository(client);
        courseController = new CoursesController(courseRep);
        userRep = new UserRepository(client);
        professorRep = new ProfessorRepository(client);

        testUser = await userRep.CreateAsync(new User
        {
            Username = "courseTestUser",
            FirstName = "Course",
            LastName = "Test User",
            Semester = 1,
            Phone = "7777",
            CreatedAt = DateTime.UtcNow
        });

        testProfessor = await professorRep.CreateAsync(new Professor
        {
            FirstName = "Course",
            LastName = "Test Professor",
            Mail = "courseTestProfessor@mail.com",
            Phone = "8888",
            Office = "707",
            CreatedAt = DateTime.UtcNow,
            UserId = testUser.Id
        });
    }

    [OneTimeTearDown]
    public async Task GlobalCleanup()
    {
        if (testProfessor != null)
            await professorRep.DeleteAsync(testProfessor.Id);
        if (testUser != null)
            await userRep.DeleteAsync(testUser.Id);
    }

    [SetUp]
    public async Task Setup()
    {
        testCourse = await courseRep.CreateAsync(new Course
        {
            Title = "Test Course",
            Semester = 1,
            Description = "Ovo je test kurs za potrebe testiranja",
            Grade = null,
            UserId = testUser.Id,
            ProfessorId = testProfessor.Id,
            CreatedAt = DateTime.UtcNow
        });

        testCourse2 = await courseRep.CreateAsync(new Course
        {
            Title = "Test Course 2",
            Semester = 2,
            Description = "Ovo je drugi test kurs za potrebe testiranja",
            Grade = null,
            UserId = testUser.Id,
            ProfessorId = testProfessor.Id,
            CreatedAt = DateTime.UtcNow
        });
    }

    [TearDown]
    public async Task Cleanup()
    {
        courseController.ModelState.Clear();
        if (testCourse != null)
            await courseRep.DeleteAsync(testCourse.Id);
        if (testCourse2 != null)
            await courseRep.DeleteAsync(testCourse2.Id);
    }

    #region GET_BY_ID (READ)

    [TestCase(true, typeof(OkObjectResult))]
    [TestCase(false, typeof(NotFoundObjectResult))]
    public async Task GetById_ReturnsCorrectResult(bool useRealId, Type expectedType)
    {
        int id = useRealId ? testCourse.Id : -1;

        var result = await courseController.GetById(id);

        Assert.That(result.Result, Is.InstanceOf(expectedType));
    }

    [Test]
    public async Task GetById_ReturnsCorrectTitle()
    {
        var result = await courseController.GetById(testCourse.Id);
        var ok = result.Result as OkObjectResult;
        var dto = ok!.Value as CourseDto;

        Assert.That(dto!.Title, Is.EqualTo("Test Course"));
    }

    [Test]
    public async Task GetById_ReturnsCorrectSemester()
    {
        var result = await courseController.GetById(testCourse.Id);
        var ok = result.Result as OkObjectResult;
        var dto = ok!.Value as CourseDto;

        Assert.That(dto!.Semester, Is.EqualTo(1));
    }

    #endregion GET_BY_ID (READ)

    #region GET_BY_USER (READ)

    [Test]
    public async Task GetByUser_ReturnsOk()
    {
        var result = await courseController.GetByUser(testUser.Id);
        var okResult = result.Result as OkObjectResult;
        var courses = okResult!.Value as IEnumerable<CourseDto>;

        Assert.Multiple(() =>
        {
            Assert.That(okResult, Is.Not.Null);
            Assert.That(courses, Is.Not.Null);
            Assert.That(courses!.Count(), Is.GreaterThanOrEqualTo(2));
        });
    }

    [Test]
    public async Task GetByUser_ReturnsEmptyList()
    {
        var result = await courseController.GetByUser(-1);
        var okResult = result.Result as OkObjectResult;
        var courses = okResult!.Value as IEnumerable<CourseDto>;

        Assert.That(courses, Is.Empty);
    }

    [Test]
    public async Task GetByUser_AreAllCoursesByTheSameUser()
    {
        var result = await courseController.GetByUser(testUser.Id);
        var okResult = result.Result as OkObjectResult;
        var courses = okResult!.Value as IEnumerable<CourseDto>;

        Assert.That(courses!.All(c => c.UserId == testUser.Id), Is.True);
    }

    #endregion GET_BY_USER (READ)

    #region CREATE

    [Test]
    public async Task Create_ReturnsCreated()
    {
        var dto = new CreateCourseDto
        {
            Title = "Created Test Course",
            Semester = 3,
            Description = "Ovo je kurs kojeg kreiramo u testu",
            Grade = null,
            UserId = testUser.Id,
            ProfessorId = testProfessor.Id
        };

        var result = await courseController.Create(dto);
        var created = result.Result as CreatedAtActionResult;

        Assert.That(created, Is.Not.Null);

        var createdDto = created!.Value as CourseDto;
        await courseRep.DeleteAsync(createdDto!.Id);
    }

    [Test]
    public async Task Create_CourseHasCorrectProfessor()
    {
        var dto = new CreateCourseDto
        {
            Title = "Course With Professor",
            Semester = 1,
            Description = "Kurs sa profesorom",
            Grade = null,
            UserId = testUser.Id,
            ProfessorId = testProfessor.Id
        };

        var result = await courseController.Create(dto);
        var created = result.Result as CreatedAtActionResult;
        var createdDto = created!.Value as CourseDto;

        Assert.That(createdDto!.ProfessorId, Is.EqualTo(testProfessor.Id));

        await courseRep.DeleteAsync(createdDto.Id);
    }

    [Test]
    public async Task Create_ReturnsBadRequest()
    {
        var dto = new CreateCourseDto
        {
            Description = "Kurs bez naslova",
            Semester = 1,
            Grade = null,
            UserId = testUser.Id,
            ProfessorId = testProfessor.Id
        };

        courseController.ModelState.AddModelError("Title", "Title is required");

        var result = await courseController.Create(dto);

        Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
    }

    #endregion CREATE

    #region UPDATE

    [Test]
    public async Task Update_ReturnsOk()
    {
        string newTitle = "Azurirani naziv kursa";

        var dto = new UpdateCourseDto
        {
            Title = newTitle,
            Semester = testCourse.Semester,
            Description = testCourse.Description,
            Grade = testCourse.Grade,
            UserId = testUser.Id,
            ProfessorId = testProfessor.Id
        };

        var result = await courseController.Update(testCourse.Id, dto);
        var okRes = result.Result as OkObjectResult;
        var updatedDto = okRes!.Value as CourseDto;

        Assert.Multiple(() =>
        {
            Assert.That(okRes, Is.Not.Null);
            Assert.That(updatedDto!.Id, Is.EqualTo(testCourse.Id));
            Assert.That(updatedDto.Title, Is.EqualTo(newTitle));
        });
    }

    [Test]
    public async Task Update_ReturnsNotFound()
    {
        var dto = new UpdateCourseDto
        {
            Title = "Nepostojeci kurs",
            Semester = 1,
            Description = "Opis nepostojeceg kursa",
            Grade = null,
            UserId = testUser.Id,
            ProfessorId = testProfessor.Id
        };

        var result = await courseController.Update(-1, dto);

        Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
    }

    [Test]
    public async Task Update_ReturnsBadRequest()
    {
        string tooLongTitle = "Ovaj naziv kursa je toliko dugacak da ne moze biti prihvacen kao validan naziv kursa u sistemu jer prelazi maksimalnu dozvoljenu duzinu naslova i samim tim bi trebalo da rezultuje greskom u validaciji podataka koji se salju na server.";

        var dto = new UpdateCourseDto
        {
            Title = tooLongTitle,
            Semester = testCourse.Semester,
            Description = testCourse.Description,
            Grade = testCourse.Grade,
            UserId = testUser.Id,
            ProfessorId = testProfessor.Id
        };

        courseController.ModelState.AddModelError("Title", "Title is too long");

        var result = await courseController.Update(testCourse.Id, dto);

        Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
    }

    #endregion UPDATE

    #region DELETE

    [Test]
    public async Task Delete_ReturnsNoContent()
    {
        var result = await courseController.Delete(testCourse.Id);

        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }

    [Test]
    public async Task Delete_ReturnsNotFound()
    {
        var result = await courseController.Delete(-1);

        Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
    }

    [Test]
    public async Task Delete_IsReallyRemoved()
    {
        await courseController.Delete(testCourse2.Id);

        var getResult = await courseController.GetById(testCourse2.Id);

        Assert.That(getResult.Result, Is.InstanceOf<NotFoundObjectResult>());
    }

    #endregion DELETE
}