using Backend.DTOs;
using Backend.Mappings;
using Backend.Models;
using Supabase;

namespace Backend.Services
{
    public class AuthService : IAuthService
    {
        private readonly Client _supabase;

        public AuthService(Client supabase)
        {
            _supabase = supabase;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            try
            {
                var authResponse = await _supabase.Auth.SignUp(dto.Email, dto.Password);

                if (authResponse?.User == null)
                    throw new Exception("Registration with Supabase Auth failed");

                var user = new User
                {
                    AuthUserId = authResponse.User.Id,
                    Email = dto.Email,
                    Username = dto.Username,
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Semester = dto.Semester,
                    Phone = dto.Phone,
                    CreatedAt = DateTime.UtcNow
                };

                var createdUser = await _supabase.From<User>().Insert(user);
                var userModel = createdUser.Models.First();

                return new AuthResponseDto
                {
                    AccessToken = authResponse.AccessToken ?? string.Empty,
                    RefreshToken = authResponse.RefreshToken ?? string.Empty,
                    User = userModel.ToDto()
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Registration failed: {ex.Message}");
            }
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            try
            {
                var authResponse = await _supabase.Auth.SignIn(dto.Email, dto.Password);

                if (authResponse?.User == null)
                    throw new Exception("Invalid email or password");

                var userResponse = await _supabase.From<User>()
                    .Where(u => u.AuthUserId == authResponse.User.Id)
                    .Single();

                if (userResponse == null)
                    throw new Exception("User profile not found");

                return new AuthResponseDto
                {
                    AccessToken = authResponse.AccessToken ?? string.Empty,
                    RefreshToken = authResponse.RefreshToken ?? string.Empty,
                    User = userResponse.ToDto()
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Login failed: {ex.Message}");
            }
        }

        public async Task<bool> LogoutAsync()
        {
            try
            {
                await _supabase.Auth.SignOut();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<User?> GetCurrentUserAsync()
        {
            try
            {
                var currentAuthUser = _supabase.Auth.CurrentUser;
                if (currentAuthUser == null) return null;

                var user = await _supabase.From<User>()
                    .Where(u => u.AuthUserId == currentAuthUser.Id)
                    .Single();

                return user;
            }
            catch
            {
                return null;
            }
        }
    }
}