using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<TaskEntity> Tasks { get; set; } = null!;
        public DbSet<CommentEntity> Comments { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TaskEntity>()
                .HasIndex(t => t.Status);

            modelBuilder.Entity<TaskEntity>()
                .HasIndex(t => t.Executor);

            modelBuilder.Entity<CommentEntity>()
                .HasIndex(c => c.TaskId);

            modelBuilder.Entity<TaskEntity>()
                .HasMany(t => t.CommentsEntities)
                .WithOne(c => c.Task)
                .HasForeignKey(p => p.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
