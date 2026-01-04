using Backend.Models;
using Supabase;

namespace Backend.Repositories
{
    public class NoteRepository : IRepository<Note>
    {
        private readonly Client _supabase;

        public NoteRepository(Client supabase)
        {
            _supabase = supabase;
        }

        public async Task<IEnumerable<Note>> GetAllAsync()
        {
            var response = await _supabase.From<Note>().Get();
            return response.Models;
        }

        public async Task<Note?> GetByIdAsync(int id)
        {
            var response = await _supabase.From<Note>()
                .Where(n => n.Id == id)
                .Single();
            return response;
        }

        public async Task<Note> CreateAsync(Note entity)
        {
            entity.LastUpdated = DateTime.UtcNow;
            var response = await _supabase.From<Note>().Insert(entity);
            return response.Models.First();
        }

        public async Task<Note> UpdateAsync(Note entity)
        {
            entity.LastUpdated = DateTime.UtcNow;
            var response = await _supabase.From<Note>().Update(entity);
            return response.Models.First();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            await _supabase.From<Note>().Where(n => n.Id == id).Delete();
            return true;
        }

        public async Task<IEnumerable<Note>> GetNotesByCourseAsync(int courseId)
        {
            var response = await _supabase.From<Note>()
                .Where(n => n.CourseId == courseId)
                .Get();
            return response.Models;
        }

        public async Task<IEnumerable<Note>> GetNotesByTypeAsync(string type)
        {
            var response = await _supabase.From<Note>()
                .Where(n => n.Type == type)
                .Get();
            return response.Models;
        }
    }
}
