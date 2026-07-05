using Microsoft.EntityFrameworkCore;
using ServicioItemsTrabajo.Models;

namespace ServicioItemsTrabajo.Data
{
    public class WorkItemsDbContext : DbContext
    {
        public WorkItemsDbContext(DbContextOptions<WorkItemsDbContext> options)
            : base(options)
        {
        }

        //Se especifica que ciertos campos son obligatorios y que los enums se guardan como texto en SQLite
        public DbSet<WorkItem> WorkItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<WorkItem>(entity =>
            {
                entity.HasKey(w => w.Id);

                entity.Property(w => w.Title)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(w => w.Description)
                    .HasMaxLength(300);

                entity.Property(w => w.AssignedUsername)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(w => w.Relevance)
                    .HasConversion<string>()
                    .HasMaxLength(10);

                entity.Property(w => w.Status)
                    .HasConversion<string>()
                    .HasMaxLength(20);
            });
        }
    }
}
