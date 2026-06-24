using Microsoft.EntityFrameworkCore;
using TodoListApi.Models;

namespace TodoListApi
{
    public class TodoDbContext(DbContextOptions<TodoDbContext> options) : DbContext(options)
    {
        public DbSet<TodoItem> TodoItems => Set<TodoItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TodoItem>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.HasIndex(e => e.UserName);

                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.LastUpdate).IsRequired();
            });
        }
    }

}
