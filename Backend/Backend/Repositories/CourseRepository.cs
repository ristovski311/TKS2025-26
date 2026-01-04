using Backend.Models;
using Supabase;

namespace Backend.Repositories
{
    public class CourseRepository : IRepository<Course>
    {
        private readonly Client _supabase;

        public CourseRepository(Client supabase)
        {
            _supabase = supabase;
        }

        public async Task<IEnumerable<Course>> GetAllAsync()
        {
            var response = await _supabase.From<Course>().Get();
            return response.Models;
        }

        public async Task<Course?> GetByIdAsync(int id)
        {
            var response = await _supabase.From<Course>()
                .Where(c => c.Id == id)
                .Single();
            return response;
        }

        public async Task<Course> CreateAsync(Course entity)
        {
            var response = await _supabase.From<Course>().Insert(entity);
            return response.Models.First();
        }

        public async Task<Course> UpdateAsync(Course entity)
        {
            var response = await _supabase.From<Course>().Update(entity);
            return response.Models.First();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            await _supabase.From<Course>().Where(c => c.Id == id).Delete();
            return true;
        }

        public async Task<IEnumerable<Course>> GetCoursesByUserAsync(int userId)
        {
            var response = await _supabase.From<Course>()
                .Where(c => c.UserId == userId)
                .Get();
            return response.Models;
        }

        public async Task<IEnumerable<Course>> GetCoursesByProfessorAsync(int professorId)
        {
            var response = await _supabase.From<Course>()
                .Where(c => c.ProfessorId == professorId)
                .Get();
            return response.Models;
        }
    }
}
