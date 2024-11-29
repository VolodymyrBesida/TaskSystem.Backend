using Task = System.Threading.Tasks.Task;
using EntityTask = TaskManagementSystem.Domain.Entities.Task;

namespace TaskManagementSystem.Domain.Interfaces.Repositories;

public interface ITaskRepository
{
    Task AddAsync(EntityTask task, CancellationToken cancellationToken = default);
    Task<IEnumerable<EntityTask>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<EntityTask?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task UpdateAsync(EntityTask task, CancellationToken cancellationToken = default);
}