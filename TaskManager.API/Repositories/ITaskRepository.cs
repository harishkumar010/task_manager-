using TaskManager.API.Models;

namespace TaskManager.API.Repositories
{
    public interface ITaskRepository : IRepository<TaskItem>
    {
        Task<IEnumerable<TaskItem>> GetTasksByUserIdAsync(int userId);
    }
}
