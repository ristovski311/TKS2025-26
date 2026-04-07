using Backend.Controllers;
using Backend.DTOs;
using Backend.Models;
using Backend.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Supabase;

namespace BackendTest;

[TestFixture]
public class UsersTests
{
    private UsersController userController;
    private UserRepository userRep;
    private User testUser;

    [OneTimeSetUp]
    public void GlobalSetup()
    {
        var config = new ConfigurationBuilder()
            .AddUserSecrets<UsersTests>()
            .Build();

        var supabaseUrl = config["Supabase:Url"] ?? throw new InvalidOperationException("Supabase URL is not configured.");
        var supabaseKey = config["Supabase:Key"] ?? throw new InvalidOperationException("Supabase Key is not configured.");

        var client = new Client(supabaseUrl, supabaseKey);
        userRep = new UserRepository(client);
        userController = new UsersController(userRep);
    }

    [SetUp]
    public async Task Setup()
    {
        testUser = await userRep.CreateAsync(new User
        {
            Username = $"testUser",
            FirstName = "Test",
            LastName = "User",
            Email = "testuser@example.com",
            Semester = 1,
            Phone = "1234",
            CreatedAt = DateTime.UtcNow
        });
    }

    [TearDown]
    public async Task Cleanup()
    {
        if (testUser != null)
            await userRep.DeleteAsync(testUser.Id);
        userController.ModelState.Clear();
    }

    #region GET BY ID

    [TestCase(true, typeof(OkObjectResult))]
    [TestCase(false, typeof(NotFoundObjectResult))]
    public async Task GetById_ReturnsCorrectStatus(bool useRealId, Type expectedType)
    {
        int id = useRealId ? testUser.Id : -1;
        var result = await userController.GetById(id);
        Assert.That(result.Result, Is.InstanceOf(expectedType));
    }

    [Test]
    public async Task GetById_ReturnsCorrectData()
    {
        var result = await userController.GetById(testUser.Id);
        var ok = result.Result as OkObjectResult;
        var dto = ok!.Value as UserDto;

        Assert.Multiple(() =>
        {
            Assert.That(dto!.Id, Is.EqualTo(testUser.Id));
            Assert.That(dto.Username, Is.EqualTo(testUser.Username));
            Assert.That(dto.FirstName, Is.EqualTo("Test"));
            Assert.That(dto.LastName, Is.EqualTo("User"));
            Assert.That(dto.Semester, Is.EqualTo(1));
        });
    }

    #endregion GET BY ID

    #region CREATE

    [Test]
    public async Task Create_ReturnsCreated()
    {
        var dto = new CreateUserDto
        {
            Username = $"createdUser",
            FirstName = "Created",
            LastName = "User",
            Semester = 2,
            Phone = "5678"
        };

        var result = await userController.Create(dto);
        var created = result.Result as CreatedAtActionResult;
        Assert.That(created, Is.Not.Null);

        var createdDto = created!.Value as UserDto;
        await userRep.DeleteAsync(createdDto!.Id);
    }

    [Test]
    public async Task Create_PersistsUser()
    {
        var dto = new CreateUserDto
        {
            Username = $"persistUser",
            FirstName = "Persistent",
            LastName = "User",
            Semester = 3
        };

        var createResult = await userController.Create(dto);
        var created = (createResult.Result as CreatedAtActionResult)!.Value as UserDto;

        var getResult = await userController.GetById(created!.Id);
        var ok = getResult.Result as OkObjectResult;
        Assert.That(ok, Is.Not.Null);

        await userRep.DeleteAsync(created.Id);
    }

    [Test]
    public async Task Create_ReturnsBadRequest()
    {
        var dto = new CreateUserDto
        {
            FirstName = "No",
            LastName = "Username",
            Semester = 1
        };

        userController.ModelState.AddModelError("Username", "Username is required");

        var result = await userController.Create(dto);
        var badRequest = result.Result as BadRequestObjectResult;
        Assert.That(badRequest, Is.Not.Null);
    }

    #endregion CREATE
}