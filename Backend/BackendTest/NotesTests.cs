using Backend.Controllers;
using Backend.DTOs;
using Backend.Models;
using Backend.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Supabase;

namespace BackendTest;

[TestFixture]
public class NotesTests
{
    private NotesController noteController;
    private NoteRepository noteRep;
    private CourseRepository courseRep;
    private UserRepository userRep;
    private ProfessorRepository professorRep;
    private Note testNote;
    private Note testFolder;
    private User testUser;
    private Course testCourse;
    private Professor testProfessor;

    [OneTimeSetUp]
    public async Task GlobalSetup()
    {
        var config = new ConfigurationBuilder()
            .AddUserSecrets<NotesTests>()
            .Build();

        var supabaseUrl = config["Supabase:Url"] ?? throw new InvalidOperationException("Supabase URL is not configured. Please add it to User Secrets or appsettings.json");
        var supabaseKey = config["Supabase:Key"] ?? throw new InvalidOperationException("Supabase Key is not configured. Please add it to User Secrets or appsettings.json");
        
        var client = new Client(supabaseUrl, supabaseKey);
        noteRep = new NoteRepository(client);
        noteController = new NotesController(noteRep);
        userRep = new UserRepository(client);
        courseRep = new CourseRepository(client);
        professorRep = new ProfessorRepository(client);

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
        testNote = await noteRep.CreateAsync(new Note
        {
            Title = "Test Note",
            Description = "Ovo je test note za potrebe testiranja",
            Content = "Test note sadrzaj",
            Type = "Note",
            CourseId = testCourse.Id,
            ParentId = null,
            CreatedAt = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow,
            UserId = testUser.Id
        });

        testFolder = await noteRep.CreateAsync(new Note
        {
            Title = "Test Folder",
            Description = "Ovo je test folder za potrebe testiranja",
            Content = "Test folder sadrzaj",
            Type = "Folder",
            CourseId = testCourse.Id,
            ParentId = null,
            CreatedAt = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow,
            UserId = testUser.Id
        });
    }

    [TearDown]
    public async Task Cleanup()
    {
        noteController.ModelState.Clear();
        if (testNote != null)
            await noteRep.DeleteAsync(testNote.Id);
        if (testFolder != null)
            await noteRep.DeleteAsync(testFolder.Id);
    }

    #region GET (READ)

    [TestCase(true, typeof(OkObjectResult))]
    [TestCase(false, typeof(NotFoundObjectResult))]
    public async Task GetById_ReturnsNote(bool useRealId, Type expectedType)
    {
        int id = useRealId ? testNote.Id : -1;

        var result = await noteController.GetById(id);
        
        Assert.That(result.Result, Is.InstanceOf(expectedType));
    }

    [TestCase(false, "Note")]
    [TestCase(true, "Folder")]
    public async Task GetById_IsCorrentType(bool testingFolder, string expectedType)
    {
        int id = testingFolder ? testFolder.Id : testNote.Id;
        var result = await noteController.GetById(id);
        var ok = result.Result as OkObjectResult;
        var dto = ok!.Value as NoteDto;

        Assert.That(dto!.Type, Is.EqualTo(expectedType));
    }

    [Test]
    public async Task GetById_ReturnsCorrectTitle()
    {
        var result = await noteController.GetById(testNote.Id);
        Console.WriteLine(testFolder.Id);
        var ok = result.Result as OkObjectResult;
        var dto = ok!.Value as NoteDto;

        Assert.That(dto!.Title, Is.EqualTo("Test Note"));
    }

    #endregion GET (READ)

    #region GET_BY_USER_ID (READ)

    [Test]
    public async Task GetByUserId_ReturnsOk()
    {
        var result = await noteController.GetByUserId(testUser.Id);

        var okResult = result as OkObjectResult;
        var notes = okResult!.Value as IEnumerable<NoteDto>;

        Assert.Multiple(() =>
        {
            Assert.That(okResult, Is.Not.Null);
            Assert.That(notes, Is.Not.Null);
            Assert.That(notes!.Count(), Is.GreaterThanOrEqualTo(2)); //Note i folder test koji smo u setup kreirali
        });
    }

    [Test]
    public async Task GetByUserId_ReturnsEmptyList()
    {
        int nonExistentUser = -1;

        var result = await noteController.GetByUserId(nonExistentUser);

        var okResult = result as OkObjectResult;
        var notes = okResult!.Value as IEnumerable<NoteDto>;

        Assert.That(notes, Is.Empty);
    }

    [Test]
    public async Task GetByUserId_AreAllNotesByTheSameUser()
    {
        var result = await noteController.GetByUserId(testUser.Id);

        var okResult = result as OkObjectResult;
        var notes = okResult!.Value as IEnumerable<NoteDto>;

        Assert.That(notes!.All(n => n.UserId == testUser.Id), Is.True);
    }


    #endregion GET_BY_USER_ID (READ)

    #region CREATE

    [Test]
    public async Task Create_ReturnsCreated()
    {
        var dto = new CreateNoteDto
        {
            Title = "Created Test Note",
            Description = "Ovo je note kojeg kreiramo",
            Content = "Sadrzaj note-a kojeg kreiramo",
            Type = "Note",
            CourseId = testCourse.Id,
            ParentId = null,
            UserId = testUser.Id
        };

        var result = await noteController.Create(dto);

        var created = result.Result as CreatedAtActionResult;
        Assert.That(created, Is.Not.Null);

        var createdDto = created!.Value as NoteDto;
        await noteRep.DeleteAsync(createdDto!.Id);
    }

    [Test]
    public async Task Create_NoteInFolder()
    {
        var dto = new CreateNoteDto
        {
            Title = "Note koji je u folderu",
            Description = "Ovo je note u folderu",
            Content = "Sadrzaj note-a koji je u folderu",
            Type = "Note",
            CourseId = testCourse.Id,
            ParentId = testFolder.Id,
            UserId = testUser.Id
        };

        var result = await noteController.Create(dto);
        var created = result.Result as CreatedAtActionResult;
        var createdDto = created!.Value as NoteDto;
        Assert.That(createdDto!.ParentId, Is.EqualTo(testFolder.Id));

        await noteRep.DeleteAsync(createdDto.Id);
    }

    [Test]
    public async Task Create_ReturnsBadReqeust()
    {
        var dto = new CreateNoteDto
        {
            Description = "Ovo je note bez naslova",
            Content = "Sadrzaj note-a koji nema naslov",
            Type = "Note",
            CourseId = testCourse.Id,
            ParentId = null,
            UserId = testUser.Id
        };
        //postoji greska sa title-om
        noteController.ModelState.AddModelError("Title", "Title is required");

        var result = await noteController.Create(dto);

        Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
    }



    #endregion CREATE

    #region UPDATE

    [Test]
    public async Task Update_ReturnsOk()
    {
        string noviTitle = "Novi naslov";

        var dto = new UpdateNoteDto
        {
            Title = noviTitle,
            UserId = testUser.Id
        };

        var result = await noteController.Update(testNote.Id, dto);
        var okRes = result.Result as OkObjectResult;

        var updatedDto = okRes!.Value as NoteDto;

        Assert.Multiple(() =>
        {
            Assert.That(okRes, Is.Not.Null);
            Assert.That(updatedDto!.Id, Is.EqualTo(testNote.Id));
            Assert.That(updatedDto.Title, Is.EqualTo(noviTitle));
        });
    }

    [Test]
    public async Task Update_ReturnsNotFound()
    {
        string noviTitle = "Novi naslov za neuspeh";

        var dto = new UpdateNoteDto
        {
            Title = noviTitle,
            UserId = testUser.Id
        };

        var result = await noteController.Update(-1, dto);
        Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
    }

    [Test]
    public async Task Update_ReturnsBadRequest()
    {
        string noviTitle = "Novi naslov za bad request mora da bude predugacak zato ovaj tekst simulira da je title bio izuzetno izuzetno predugacak da bih mogao uopste da se smatra za title note-a kako bismo testirali da on zapravo ne moze da bude stavljen kao note title i s tim nas test ce to pokazati .";

        var dto = new UpdateNoteDto
        {
            Title = noviTitle,
            UserId = testUser.Id
        };

        noteController.ModelState.AddModelError("Title", "Title is too long");

        var result = await noteController.Update(testNote.Id, dto);
        Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
    }

    #endregion UPDATE

    #region DELETE

    [Test]
    public async Task Delete_ReturnsOk()
    {
        var result = await noteController.Delete(testNote.Id);

        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }

    [Test]
    public async Task Delete_NotFound()
    {
        var result = await noteController.Delete(-1);

        Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
    }

    [Test]
    public async Task Delete_IsReallyRemoved()
    {
        await noteController.Delete(testFolder.Id);

        var getResult = await noteController.GetById(testFolder.Id);

        Assert.That(getResult.Result, Is.InstanceOf<NotFoundObjectResult>());
    }

    #endregion DELETE

}
