using Backend.Models;
using Supabase;

namespace Backend.Repositories
{
    public class ProfessorRepository : IRepository<Professor>
    {
        private readonly Client _supabase;

        public ProfessorRepository(Client supabase)
        {
            _supabase = supabase;
        }

        public async Task<IEnumerable<Professor>> GetAllAsync()
        {
            var response = await _supabase.From<Professor>().Get();
            return response.Models;
        }

        public async Task<Professor?> GetByIdAsync(int id)
        {
            var response = await _supabase.From<Professor>()
                .Where(p => p.Id == id)
                .Single();
            return response;
        }

        public async Task<Professor> CreateAsync(Professor entity)
        {
            var response = await _supabase.From<Professor>().Insert(entity);
            return response.Models.First();
        }

        public async Task<Professor> UpdateAsync(Professor entity)
        {
            var response = await _supabase.From<Professor>().Update(entity);
            return response.Models.First();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            await _supabase.From<Professor>().Where(p => p.Id == id).Delete();
            return true;
        }
    }
}
