using Microsoft.EntityFrameworkCore;

using EntityTask = TaskManagementSystem.Domain.Entities.Task;
using EntityTaskStatus = TaskManagementSystem.Domain.Enums.TaskStatus;

namespace TaskManagementSystem.Infrastructure.Persistance;

public class AppDbContext : DbContext
{
    public DbSet<EntityTask> Tasks { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<EntityTask>().HasData(
            new EntityTask
            {
                Id = 1,
                Name = "Test Task",
                Description = "Make your technical task and for the Intaker company",
                Status = EntityTaskStatus.Completed,
                AssignedTo = "Volodymyr"
            },
            new EntityTask
            {
                Id = 2,
                Name = "Go to the shop",
                Description = "Don't forget to get your pack of coffee",
                Status = EntityTaskStatus.InProgress,
                AssignedTo = "Igor"
            }
        );
    }
}