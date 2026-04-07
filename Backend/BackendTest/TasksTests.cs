using Backend.Controllers;
using Backend.Repositories;
using Backend.DTOs;
using Backend.Models;
using Microsoft.Extensions.Configuration;
using Supabase;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework.Internal;
using System.Threading.Tasks;

namespace BackendTest;

public class TasksTests
{
    private TasksController tasksController;
    private TaskRepository taskRep;
    private UserRepository userRep;
    private CourseRepository courseRep;
    private ProfessorRepository professorRep;

    private TaskItem testTaskLab;
    private TaskItem testTaskKol;
    private User testUser;
    private Professor testProfessor;
    private Course testCourse;

    [OneTimeSetUp]
    public async Task GlobalSetup()
    {
        var config = new ConfigurationBuilder().AddUserSecrets<TasksTests>().Build();

        var supabaseUrl = config["Supabase:Url"] ?? throw new InvalidOperationException("Supabase Key is not configured. Please add it to User Secrets or appsettings.json");
        var supabaseKey = config["Supabase:Key"] ?? throw new InvalidOperationException("Supabase Key is not configured. Please add it to User Secrets or appsettings.json");

        var client = new Client(supabaseUrl, supabaseKey);

        userRep = new UserRepository(client);
        courseRep = new CourseRepository(client);
        professorRep = new ProfessorRepository(client);
        taskRep = new TaskRepository(client);
        tasksController = new TasksController(taskRep);

        testUser = await userRep.CreateAsync(new User
        {
            Username = "noteTestUser",
            FirstName = "Note",
            LastName = "Test User",
            Semester = 1,
            Phone = "1234",
            CreatedAt = DateTime.UtcNow
        });

        testProfessor = await professorRep.CreateAsync(new Professor
        {
            FirstName = "Note",
            LastName = "Test Professor",
            Mail = "noteTestProfessor@mail.com",
            Phone = "1212",
            Office = "123",
            CreatedAt = DateTime.UtcNow,
            UserId = testUser.Id
        });

        testCourse = await courseRep.CreateAsync(new Course
        {
            Title = "Note Test Course",
            Semester = 1,
            Description = "Ovo je kurs za potrebe testiranja note-ova",
            Grade = null,
            UserId = testUser.Id,
            ProfessorId = testProfessor.Id,
            CreatedAt = DateTime.UtcNow
        });
    }

    [OneTimeTearDown]
    public async Task GlobalCleanup()
    {
        if (testCourse != null)
            await courseRep.DeleteAsync(testCourse.Id);
        if (testProfessor != null)
            await professorRep.DeleteAsync(testProfessor.Id);
        if (testUser != null)
            await userRep.DeleteAsync(testUser.Id);
    }

    [SetUp]
    public async Task Setup()
    {
        testTaskLab = await taskRep.CreateAsync(new TaskItem
        {
            Title = "Test Task",
            Type = "Lab",
            Description = "Ovo je test task",
            Date = DateTime.UtcNow,
            Completed = false,
            GradeMax = 20,
            GradeEarned = null,
            CourseId = testCourse.Id,
            CreatedAt = DateTime.UtcNow,
            UserId = testUser.Id
        });

        testTaskKol = await taskRep.CreateAsync(new TaskItem
        {
            Title = "Test Task Completed",
            Type = "Kolokvijum",
            Description = "Ovo je test task koji je zavrsen",
            Date = DateTime.UtcNow,
            Completed = true,
            GradeMax = 20,
            GradeEarned = 18,
            CourseId = testCourse.Id,
            CreatedAt = DateTime.UtcNow,
            UserId = testUser.Id
        });
    }

    [TearDown]
    public async Task Cleanup()
    {
        if (testTaskLab != null)
            await taskRep.DeleteAsync(testTaskLab.Id);
        if (testTaskKol != null)
            await taskRep.DeleteAsync(testTaskKol.Id);
        tasksController.ModelState.Clear();
    }

    #region GET_BY_ID (READ)

    [TestCase(true, typeof(OkObjectResult))]
    [TestCase(false, typeof(NotFoundObjectResult))]
    public async Task GetById_ReturnsTask(bool useRealId, Type expectedType)
    {
        int id = useRealId ? testTaskLab.Id : -1;

        var result = await tasksController.GetById(id);

        Assert.That(result.Result, Is.InstanceOf(expectedType));
    }

    [TestCase(true, "Lab")]
    [TestCase(false, "Kolokvijum")]
    public async Task GetById_IsCorrentType(bool testingLab, string expectedType)
    {
        int id = testingLab ? testTaskLab.Id : testTaskKol.Id;
        var result = await tasksController.GetById(id);
        var ok = result.Result as OkObjectResult;
        var dto = ok!.Value as TaskDto;

        Assert.That(dto!.Type, Is.EqualTo(expectedType));
    }

    [Test]
    public async Task GetById_ReturnsCorrectTitle()
    {
        var result = await tasksController.GetById(testTaskLab.Id);
        var ok = result.Result as OkObjectResult;
        var dto = ok!.Value as TaskDto;

        Assert.That(dto!.Title, Is.EqualTo("Test Task"));
    }

    #endregion GET_BY_ID (READ)

    #region GET_BY_USER_ID
    [Test]
    public async Task GetByUserId_ReturnsOk()
    {
        var result = await tasksController.GetByUserId(testUser.Id);

        var okResult = result as OkObjectResult;
        var notes = okResult!.Value as IEnumerable<TaskDto>;

        Assert.Multiple(() =>
        {
            Assert.That(okResult, Is.Not.Null);
            Assert.That(notes, Is.Not.Null);
            Assert.That(notes!.Count(), Is.GreaterThanOrEqualTo(2)); //Ova dva nasa testa, lab i kol
        });
    }

    [Test]
    public async Task GetByUserId_ReturnsEmptyList()
    {
        int nonExistentUser = -1;

        var result = await tasksController.GetByUserId(nonExistentUser);

        var okResult = result as OkObjectResult;
        var notes = okResult!.Value as IEnumerable<TaskDto>;

        Assert.That(notes, Is.Empty);
    }

    [Test]
    public async Task GetByUserId_AreAllNotesByTheSameUser()
    {
        var result = await tasksController.GetByUserId(testUser.Id);

        var okResult = result as OkObjectResult;
        var notes = okResult!.Value as IEnumerable<TaskDto>;

        Assert.That(notes!.All(n => n.UserId == testUser.Id), Is.True);
    }

    #endregion GET_BY_USER_ID

    #region CREATE

    [Test]
    public async Task Create_ReturnsCreated()
    {
        var dto = new CreateTaskDto
        {
            Title = "Test task created",
            Type = "Lab",
            Description = "Ovo je opis test task-a koji se kreira",
            Date = DateTime.UtcNow,
            Completed = false,
            GradeMax = 30,
            GradeEarned = null,
            CourseId = testCourse.Id,
            UserId = testUser.Id
        };

        var result = await tasksController.Create(dto);

        var created = result.Result as CreatedAtActionResult;
        Assert.That(created, Is.Not.Null);

        var createdDto = created!.Value as TaskDto;
        await taskRep.DeleteAsync(createdDto!.Id);
    }

    [Test]
    public async Task Create_TaskCompleted()
    {
        var dto = new CreateTaskDto
        {
            Title = "Test task created completed",
            Type = "Lab",
            Description = "Ovo je opis test task-a koji se kreira i completed je",
            Date = DateTime.UtcNow,
            Completed = true,
            GradeMax = 30,
            GradeEarned = 25,
            CourseId = testCourse.Id,
            UserId = testUser.Id
        };

        var result = await tasksController.Create(dto);
        var created = result.Result as CreatedAtActionResult;
        var createdDto = created!.Value as TaskDto;
        Assert.That(createdDto!.GradeEarned, Is.EqualTo(25));

        await taskRep.DeleteAsync(createdDto.Id);
    }

    [Test]
    public async Task Create_ReturnsBadReqeust()
    {
        var dto = new CreateTaskDto
        {
            Title = "",
            Type = "Lab",
            Description = "Ovo je opis test task-a koji se kreira",
            Date = DateTime.UtcNow,
            Completed = true,
            GradeMax = 30,
            GradeEarned = 25,
            CourseId = testCourse.Id,
            UserId = testUser.Id
        };
        //Simuliramo da je title prazan sto ne sme
        tasksController.ModelState.AddModelError("Title", "Title is required");

        var result = await tasksController.Create(dto);

        Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
    }

    #endregion CREATE

    #region UPDATE

    [Test]
    public async Task Update_ReturnsOk()
    {
        string noviTitle = "Novi naslov taska lab";

        var dto = new UpdateTaskDto
        {
            Title = noviTitle,
            UserId = testUser.Id
        };

        var result = await tasksController.Update(testTaskLab.Id, dto);
        var okRes = result.Result as OkObjectResult;

        var updatedDto = okRes!.Value as TaskDto;

        Assert.Multiple(() =>
        {
            Assert.That(okRes, Is.Not.Null);
            Assert.That(updatedDto!.Id, Is.EqualTo(testTaskLab.Id));
            Assert.That(updatedDto.Title, Is.EqualTo(noviTitle));
        });
    }

    [Test]
    public async Task Update_ReturnsNotFound()
    {
        string noviTitle = "Novi naslov taska za neuspeh";

        var dto = new UpdateTaskDto
        {
            Title = noviTitle,
            UserId = testUser.Id
        };

        var result = await tasksController.Update(-1, dto);
        Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
    }

    [Test]
    public async Task Update_ReturnsBadRequest()
    {
        string noviTitle = "Novi naslov za bad request mora da bude predugacak zato ovaj tekst simulira da je title bio izuzetno izuzetno predugacak da bih mogao uopste da se smatra za title note-a kako bismo testirali da on zapravo ne moze da bude stavljen kao note title i s tim nas test ce to pokazati .";

        var dto = new UpdateTaskDto
        {
            Title = noviTitle,
            UserId = testUser.Id
        };

        tasksController.ModelState.AddModelError("Title", "Title is too long");

        var result = await tasksController.Update(testTaskKol.Id, dto);
        Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
    }

    #endregion UPDATE

    #region DELETE


    [Test]
    public async Task Delete_ReturnsOk()
    {
        var result = await tasksController.Delete(testTaskLab.Id);

        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }

    [Test]
    public async Task Delete_NotFound()
    {
        var result = await tasksController.Delete(-1);

        Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
    }

    [Test]
    public async Task Delete_IsReallyRemoved()
    {
        await tasksController.Delete(testTaskLab.Id);

        var getResult = await tasksController.GetById(testTaskLab.Id);

        Assert.That(getResult.Result, Is.InstanceOf<NotFoundObjectResult>());
    }


    #endregion DELETE
}

