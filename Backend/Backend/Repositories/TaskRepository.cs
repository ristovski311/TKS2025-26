using Backend.Models;
using Supabase;

namespace Backend.Repositories
{
    public class TaskRepository : IRepository<TaskItem>
    {
        private readonly Client _supabase;

        public TaskRepository(Client supabase)
        {
            _supabase = supabase;
        }

        public async Task<IEnumerable<TaskItem>> GetAllAsync()
        {
            var response = await _supabase.From<TaskItem>().Get();
            return response.Models;
        }

        public async Task<TaskItem?> GetByIdAsync(int id)
        {
            var response = await _supabase.From<TaskItem>()
                .Where(t => t.Id == id)
                .Single();
            return response;
        }

        public async Task<TaskItem> CreateAsync(TaskItem entity)
        {
            var response = await _supabase.From<TaskItem>().Insert(entity);
            return response.Models.First();
        }

        public async Task<TaskItem> UpdateAsync(TaskItem entity)
        {
            var response = await _supabase.From<TaskItem>().Update(entity);
            return response.Models.First();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            await _supabase.From<TaskItem>().Where(t => t.Id == id).Delete();
            return true;
        }

        public async Task<IEnumerable<TaskItem>> GetTasksByCourseAsync(int courseId)
        {
            var response = await _supabase.From<TaskItem>()
                .Where(t => t.CourseId == courseId)
                .Get();
            return response.Models;
        }

        public async Task<IEnumerable<TaskItem>> GetPendingTasksAsync()
        {
            var response = await _supabase.From<TaskItem>()
                .Where(t => t.Completed == false)
                .Get();
            return response.Models;
        }
    }
}
