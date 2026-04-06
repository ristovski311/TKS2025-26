using Backend.Controllers;
using Backend.DTOs;
using Backend.Models;
using Backend.Repositories;
using Microsoft.AspNetCore.Mvc;
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
        var dto = ok!.Value as Backend.DTOs.NoteDto;

        Assert.That(dto!.Type, Is.EqualTo(expectedType));
    }

    [Test]
    public async Task GetById_ReturnsCorrectTitle()
    {
        var result = await noteController.GetById(testNote.Id);
        Console.WriteLine(testFolder.Id);
        var ok = result.Result as OkObjectResult;
        var dto = ok!.Value as Backend.DTOs.NoteDto;

        Assert.That(dto!.Title, Is.EqualTo("Test Note"));
    }

    #endregion GET (READ)


    #region CREATE

    [Test]
    public async Task Create_ReturnsCreated_WhenValid()
    {
        var dto = new CreateNoteDto
        {
            Title = "Created Test Note",
            Description = "Ovo je kreirana nota",
            Content = "Sadrzaj",
            Type = "Note",
            CourseId = testCourse.Id,
            UserId = testUser.Id
        };

        var result = await noteController.Create(dto);
        var created = result.Result as CreatedAtActionResult;
        Assert.That(created, Is.Not.Null);

        var createdDto = created!.Value as Backend.DTOs.NoteDto;
        await noteRep.DeleteAsync(createdDto!.Id);
    }

    [Test]
    public async Task Create_PersistsNote_CanBeRetrievedAfterCreation()
    {
        var dto = new CreateNoteDto
        {
            Title = "Persistent Note",
            Description = "Ovo je perzistentna nota",
            Content = "Sadrzaj",
            Type = "Note",
            CourseId = testCourse.Id,
            UserId = testUser.Id
        };

        var createResult = await noteController.Create(dto);
        var created = (createResult.Result as CreatedAtActionResult)!.Value as Backend.DTOs.NoteDto;

        var getResult = await noteController.GetById(created!.Id);
        var ok = getResult.Result as OkObjectResult;
        Assert.That(ok, Is.Not.Null);

        await noteRep.DeleteAsync(created.Id);
    }

    [Test]
    public async Task Create_ReturnsCorrectData_WhenValid()
    {
        var dto = new CreateNoteDto
        {
            Title = "Data Check Note",
            Description = "Opis",
            Content = "Sadrzaj",
            Type = "Note",
            CourseId = testCourse.Id,
            UserId = testUser.Id
        };

        var result = await noteController.Create(dto);
        var created = (result.Result as CreatedAtActionResult)!.Value as Backend.DTOs.NoteDto;

        Assert.Multiple(() =>
        {
            Assert.That(created!.Title, Is.EqualTo("Data Check Note"));
            Assert.That(created.UserId, Is.EqualTo(testUser.Id));
            Assert.That(created.CourseId, Is.EqualTo(testCourse.Id));
            Assert.That(created.Type, Is.EqualTo("Note"));
        });

        await noteRep.DeleteAsync(created!.Id);
    }



    #endregion CREATE


    #region UPDATE


    #endregion UPDATE


    #region DELETE


    #endregion DELETE

}
