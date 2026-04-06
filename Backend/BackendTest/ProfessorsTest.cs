using Backend.Controllers;
using Backend.DTOs;
using Backend.Models;
using Backend.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Supabase;

namespace BackendTest;

[TestFixture]
public class ProfessorsTest
{
    private ProfessorsController professorController;
    private ProfessorRepository professorRep;
    private UserRepository userRep;
    private Professor testProfessor;
    private Professor testProfessor2;
    private User testUser;

    [OneTimeSetUp]
    public async Task GlobalSetup()
    {
        var config = new ConfigurationBuilder()
            .AddUserSecrets<ProfessorsTest>()
            .Build();

        var supabaseUrl = config["Supabase:Url"] ?? throw new InvalidOperationException("Supabase URL is not configured.");
        var supabaseKey = config["Supabase:Key"] ?? throw new InvalidOperationException("Supabase Key is not configured.");

        var client = new Client(supabaseUrl, supabaseKey);
        professorRep = new ProfessorRepository(client);
        professorController = new ProfessorsController(professorRep);
        userRep = new UserRepository(client);

        testUser = await userRep.CreateAsync(new User
        {
            Username = "professorTestUser",
            FirstName = "Professor",
            LastName = "Test User",
            Semester = 1,
            Phone = "9999",
            CreatedAt = DateTime.UtcNow
        });
    }

    [OneTimeTearDown]
    public async Task GlobalCleanup()
    {
        if (testUser != null)
            await userRep.DeleteAsync(testUser.Id);
    }

    [SetUp]
    public async Task Setup()
    {
        testProfessor = await professorRep.CreateAsync(new Professor
        {
            FirstName = "Marko",
            LastName = "Markovic",
            Mail = "marko.markovic@mail.com",
            Phone = "1111",
            Office = "101",
            CreatedAt = DateTime.UtcNow,
            UserId = testUser.Id
        });

        testProfessor2 = await professorRep.CreateAsync(new Professor
        {
            FirstName = "Petar",
            LastName = "Petrovic",
            Mail = "petar.petrovic@mail.com",
            Phone = "2222",
            Office = "202",
            CreatedAt = DateTime.UtcNow,
            UserId = testUser.Id
        });
    }

    [TearDown]
    public async Task Cleanup()
    {
        professorController.ModelState.Clear();
        if (testProfessor != null)
            await professorRep.DeleteAsync(testProfessor.Id);
        if (testProfessor2 != null)
            await professorRep.DeleteAsync(testProfessor2.Id);
    }

    #region GET_ALL (READ)

    [Test]
    public async Task GetAll_ReturnsOk()
    {
        var result = await professorController.GetAll();
        var okResult = result.Result as OkObjectResult;

        Assert.That(okResult, Is.Not.Null);
    }

    [Test]
    public async Task GetAll_ReturnsAtLeastTwoProfessors()
    {
        var result = await professorController.GetAll();
        var okResult = result.Result as OkObjectResult;
        var professors = okResult!.Value as IEnumerable<ProfessorDto>;

        Assert.That(professors!.Count(), Is.GreaterThanOrEqualTo(2));
    }

    [Test]
    public async Task GetAll_ContainsTestProfessor()
    {
        var result = await professorController.GetAll();
        var okResult = result.Result as OkObjectResult;
        var professors = okResult!.Value as IEnumerable<ProfessorDto>;

        Assert.That(professors!.Any(p => p.Id == testProfessor.Id), Is.True);
    }

    #endregion GET_ALL (READ)

    #region GET_BY_ID (READ)

    [TestCase(true, typeof(OkObjectResult))]
    [TestCase(false, typeof(NotFoundObjectResult))]
    public async Task GetById_ReturnsCorrectResult(bool useRealId, Type expectedType)
    {
        int id = useRealId ? testProfessor.Id : -1;

        var result = await professorController.GetById(id);

        Assert.That(result.Result, Is.InstanceOf(expectedType));
    }

    [Test]
    public async Task GetById_ReturnsCorrectFirstName()
    {
        var result = await professorController.GetById(testProfessor.Id);
        var ok = result.Result as OkObjectResult;
        var dto = ok!.Value as ProfessorDto;

        Assert.That(dto!.FirstName, Is.EqualTo("Marko"));
    }

    [Test]
    public async Task GetById_ReturnsCorrectMail()
    {
        var result = await professorController.GetById(testProfessor.Id);
        var ok = result.Result as OkObjectResult;
        var dto = ok!.Value as ProfessorDto;

        Assert.That(dto!.Mail, Is.EqualTo("marko.markovic@mail.com"));
    }

    #endregion GET_BY_ID (READ)

    #region GET_BY_USER_ID (READ)

    [Test]
    public async Task GetByUserId_ReturnsOk()
    {
        var result = await professorController.GetByUserId(testUser.Id);
        var okResult = result as OkObjectResult;
        var professors = okResult!.Value as IEnumerable<ProfessorDto>;

        Assert.Multiple(() =>
        {
            Assert.That(okResult, Is.Not.Null);
            Assert.That(professors, Is.Not.Null);
            Assert.That(professors!.Count(), Is.GreaterThanOrEqualTo(2));
        });
    }

    [Test]
    public async Task GetByUserId_ReturnsEmptyList()
    {
        var result = await professorController.GetByUserId(-1);
        var okResult = result as OkObjectResult;
        var professors = okResult!.Value as IEnumerable<ProfessorDto>;

        Assert.That(professors, Is.Empty);
    }

    [Test]
    public async Task GetByUserId_AreAllProfessorsByTheSameUser()
    {
        var result = await professorController.GetByUserId(testUser.Id);
        var okResult = result as OkObjectResult;
        var professors = okResult!.Value as IEnumerable<ProfessorDto>;

        Assert.That(professors!.All(p => p.UserId == testUser.Id), Is.True);
    }

    #endregion GET_BY_USER_ID (READ)

    #region CREATE

    [Test]
    public async Task Create_ReturnsCreated()
    {
        var dto = new CreateProfessorDto
        {
            FirstName = "Jovan",
            LastName = "Jovanovic",
            Mail = "created.professor@mail.com",
            Phone = "3333",
            Office = "303",
            UserId = testUser.Id
        };

        var result = await professorController.Create(dto);
        var created = result.Result as CreatedAtActionResult;

        Assert.That(created, Is.Not.Null);

        var createdDto = created!.Value as ProfessorDto;
        await professorRep.DeleteAsync(createdDto!.Id);
    }

    [Test]
    public async Task Create_ProfessorHasCorrectUserId()
    {
        var dto = new CreateProfessorDto
        {
            FirstName = "Goran",
            LastName = "Petrovic",
            Mail = "goran.petrovic@mail.com",
            Phone = "4444",
            Office = "404",
            UserId = testUser.Id
        };

        var result = await professorController.Create(dto);
        var created = result.Result as CreatedAtActionResult;
        var createdDto = created!.Value as ProfessorDto;

        Assert.That(createdDto!.UserId, Is.EqualTo(testUser.Id));

        await professorRep.DeleteAsync(createdDto.Id);
    }

    [Test]
    public async Task Create_ReturnsBadRequest()
    {
        var dto = new CreateProfessorDto
        {
            FirstName = "Igor",
            LastName = "Petrovic",
            Phone = "555",
            Office = "505",
            UserId = testUser.Id
        };

        professorController.ModelState.AddModelError("Mail", "Mail is required");

        var result = await professorController.Create(dto);

        Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
    }

    #endregion CREATE

    #region UPDATE

    [Test]
    public async Task Update_ReturnsOk()
    {
        string newOffice = "999";

        var dto = new UpdateProfessorDto
        {
            FirstName = testProfessor.FirstName,
            LastName = testProfessor.LastName,
            Mail = testProfessor.Mail,
            Phone = testProfessor.Phone,
            Office = newOffice,
            UserId = testUser.Id
        };

        var result = await professorController.Update(testProfessor.Id, dto);
        var okRes = result.Result as OkObjectResult;
        var updatedDto = okRes!.Value as ProfessorDto;

        Assert.Multiple(() =>
        {
            Assert.That(okRes, Is.Not.Null);
            Assert.That(updatedDto!.Id, Is.EqualTo(testProfessor.Id));
            Assert.That(updatedDto.Office, Is.EqualTo(newOffice));
        });
    }

    [Test]
    public async Task Update_ReturnsNotFound()
    {
        var dto = new UpdateProfessorDto
        {
            FirstName = "Dragan",
            LastName = "Draganovic",
            Mail = "dragan.draganovic@mail.com",
            Phone = "0000",
            Office = "000",
            UserId = testUser.Id
        };

        var result = await professorController.Update(-1, dto);

        Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
    }

    [Test]
    public async Task Update_ReturnsBadRequest()
    {
        var dto = new UpdateProfessorDto
        {
            FirstName = "Bad",
            LastName = "Request",
            Mail = "nevalidan-email-format-koji-je-veoma-dugacak-da-se-smatra-validnim-u-bilo-kom-sistemu-koji-se-testira",
            Phone = "0000",
            Office = "606",
            UserId = testUser.Id
        };

        professorController.ModelState.AddModelError("Mail", "Mail is invalid");

        var result = await professorController.Update(testProfessor.Id, dto);

        Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
    }

    #endregion UPDATE

    #region DELETE

    [Test]
    public async Task Delete_ReturnsNoContent()
    {
        var result = await professorController.Delete(testProfessor.Id);

        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }

    [Test]
    public async Task Delete_ReturnsNotFound()
    {
        var result = await professorController.Delete(-1);

        Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
    }

    [Test]
    public async Task Delete_IsReallyRemoved()
    {
        await professorController.Delete(testProfessor2.Id);

        var getResult = await professorController.GetById(testProfessor2.Id);

        Assert.That(getResult.Result, Is.InstanceOf<NotFoundObjectResult>());
    }

    #endregion DELETE
}
