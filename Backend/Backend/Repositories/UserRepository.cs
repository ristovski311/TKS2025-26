using Backend.Models;
using Supabase;

namespace Backend.Repositories
{
    public class UserRepository : IRepository<User>
    {
        private readonly Client _supabase;

        public UserRepository(Client supabase)
        {
            _supabase = supabase;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            var response = await _supabase.From<User>().Get();
            return response.Models;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            var response = await _supabase.From<User>()
                .Where(u => u.Id == id)
                .Single();
            return response;
        }

        public async Task<User> CreateAsync(User entity)
        {
            var response = await _supabase.From<User>().Insert(entity);
            return response.Models.First();
        }

        public async Task<User> UpdateAsync(User entity)
        {
            var response = await _supabase.From<User>().Update(entity);
            return response.Models.First();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            await _supabase.From<User>().Where(u => u.Id == id).Delete();
            return true;
        }

        public async Task<IEnumerable<User>> GetUsersBySemesterAsync(int semester)
        {
            var response = await _supabase.From<User>()
                .Where(u => u.Semester == semester)
                .Get();
            return response.Models;
        }
    }
}
