using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Domain.Interfaces.Repositories;
using TaskManagementSystem.Infrastructure.Persistance;

using EntityTask = TaskManagementSystem.Domain.Entities.Task;
using Task = System.Threading.Tasks.Task;

namespace TaskManagementSystem.Infrastructure.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly AppDbContext _context;

    public TaskRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(EntityTask task, CancellationToken cancellationToken = default)
    {
        await _context.Tasks.AddAsync(task, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<EntityTask>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.Tasks.ToListAsync(cancellationToken);

    public async Task<EntityTask?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await _context.Tasks.FindAsync(id, cancellationToken);

    public async Task UpdateAsync(EntityTask task, CancellationToken cancellationToken = default)
    {
        _context.Tasks.Update(task);
        await _context.SaveChangesAsync(cancellationToken);
    }
}