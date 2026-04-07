using Backend.Controllers;
using Backend.DTOs;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Supabase;

namespace BackendTest;

[TestFixture]
public class AuthTest
{
    private AuthController authController;
    private AuthService authService;
    private Client supabaseClient;

    private const string TestEmail = "auth.test.user@mail.com";
    private const string TestPassword = "TestPassword123!";
    private const string TestUsername = "authTestUser";

    [OneTimeSetUp]
    public async Task GlobalSetup()
    {
        var config = new ConfigurationBuilder()
            .AddUserSecrets<AuthTest>()
            .Build();

        var supabaseUrl = config["Supabase:Url"] ?? throw new InvalidOperationException("Supabase URL is not configured.");
        var supabaseKey = config["Supabase:Key"] ?? throw new InvalidOperationException("Supabase Key is not configured.");

        supabaseClient = new Client(supabaseUrl, supabaseKey);
        await supabaseClient.InitializeAsync();

        authService = new AuthService(supabaseClient);
        authController = new AuthController(authService);
    }

    [TearDown]
    public async Task Cleanup()
    {
        try
        {
            var loginResponse = await supabaseClient.Auth.SignIn(TestEmail, TestPassword);
            if (loginResponse?.User != null)
            {
                await supabaseClient.From<Backend.Models.User>()
                    .Where(u => u.AuthUserId == loginResponse.User.Id)
                    .Delete();

                var authUserId = loginResponse.User.Id;
                await supabaseClient.Auth.SignOut();

                var config = new ConfigurationBuilder()
                    .AddUserSecrets<AuthTest>()
                    .Build();
                var serviceKey = config["Supabase:ServiceKey"];
                var supabaseUrl = config["Supabase:Url"];

                using var http = new HttpClient();
                http.DefaultRequestHeaders.Add("apikey", serviceKey);
                http.DefaultRequestHeaders.Add("Authorization", $"Bearer {serviceKey}");
                await http.DeleteAsync($"{supabaseUrl}/auth/v1/admin/users/{authUserId}");
            }
        }
        catch
        {
            // Korisnik ne postoji, nema potrebe za ciscenjem
        }
    }

    #region REGISTER

    [Test]
    public async Task Register_ReturnsOk()
    {
        var dto = new RegisterDto
        {
            Email = TestEmail,
            Password = TestPassword,
            Username = TestUsername,
            FirstName = "Auth",
            LastName = "Test",
            Semester = 1,
            Phone = "1234"
        };

        var result = await authController.Register(dto);

        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public async Task Register_ReturnsTokens()
    {
        var dto = new RegisterDto
        {
            Email = TestEmail,
            Password = TestPassword,
            Username = TestUsername,
            FirstName = "Auth",
            LastName = "Test",
            Semester = 1,
            Phone = "1234"
        };

        var result = await authController.Register(dto);
        var ok = result.Result as OkObjectResult;
        var response = ok!.Value as AuthResponseDto;

        Assert.Multiple(() =>
        {
            Assert.That(response!.AccessToken, Is.Not.Null.And.Not.Empty);
            Assert.That(response.RefreshToken, Is.Not.Null.And.Not.Empty);
        });
    }

    [Test]
    public async Task Register_ReturnsBadRequest_OnDuplicateEmail()
    {
        var dto = new RegisterDto
        {
            Email = TestEmail,
            Password = TestPassword,
            Username = TestUsername,
            FirstName = "Auth",
            LastName = "Test",
            Semester = 1,
            Phone = "1234"
        };

        await authController.Register(dto);

        var result = await authController.Register(dto);

        Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
    }

    #endregion REGISTER

    #region LOGIN

    [Test]
    public async Task Login_ReturnsOk()
    {
        await authController.Register(new RegisterDto
        {
            Email = TestEmail,
            Password = TestPassword,
            Username = TestUsername,
            FirstName = "Auth",
            LastName = "Test",
            Semester = 1,
            Phone = "1234"
        });

        await Task.Delay(1000);

        var result = await authController.Login(new LoginDto
        {
            Email = TestEmail,
            Password = TestPassword
        });

        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public async Task Login_ReturnsCorrectUser()
    {
        await authController.Register(new RegisterDto
        {
            Email = TestEmail,
            Password = TestPassword,
            Username = TestUsername,
            FirstName = "Auth",
            LastName = "Test",
            Semester = 1,
            Phone = "1234"
        });

        await Task.Delay(1000);

        var result = await authController.Login(new LoginDto
        {
            Email = TestEmail,
            Password = TestPassword
        });

        var ok = result.Result as OkObjectResult;
        var response = ok!.Value as AuthResponseDto;

        Assert.That(response!.User.Username, Is.EqualTo(TestUsername));
    }

    [Test]
    public async Task Login_ReturnsUnauthorized_OnWrongPassword()
    {
        await authController.Register(new RegisterDto
        {
            Email = TestEmail,
            Password = TestPassword,
            Username = TestUsername,
            FirstName = "Auth",
            LastName = "Test",
            Semester = 1,
            Phone = "1234"
        });

        await Task.Delay(1000);

        var result = await authController.Login(new LoginDto
        {
            Email = TestEmail,
            Password = "WrongPassword999!"
        });

        Assert.That(result.Result, Is.InstanceOf<UnauthorizedObjectResult>());
    }

    #endregion LOGIN

    #region LOGOUT

    [Test]
    public async Task Logout_ReturnsOk()
    {
        await authController.Register(new RegisterDto
        {
            Email = TestEmail,
            Password = TestPassword,
            Username = TestUsername,
            FirstName = "Auth",
            LastName = "Test",
            Semester = 1,
            Phone = "1234"
        });

        await authController.Login(new LoginDto
        {
            Email = TestEmail,
            Password = TestPassword
        });

        var result = await authController.Logout();

        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public async Task Logout_ReturnsOk_WhenNotLoggedIn()
    {
        var result = await authController.Logout();

        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public async Task Logout_LoginFailsAfterLogout()
    {
        await authController.Register(new RegisterDto
        {
            Email = TestEmail,
            Password = TestPassword,
            Username = TestUsername,
            FirstName = "Auth",
            LastName = "Test",
            Semester = 1,
            Phone = "1234"
        });

        await authController.Login(new LoginDto
        {
            Email = TestEmail,
            Password = TestPassword
        });

        await authController.Logout();

        var session = supabaseClient.Auth.CurrentSession;

        Assert.That(session, Is.Null);
    }

    #endregion LOGOUT
}