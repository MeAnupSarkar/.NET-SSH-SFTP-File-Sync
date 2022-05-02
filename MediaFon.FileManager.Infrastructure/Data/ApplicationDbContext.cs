

using Microsoft.EntityFrameworkCore;
using File = MediaFon.FileManager.Domain.Entity.File;
using Directory = MediaFon.FileManager.Domain.Entity.Directory;
using MediaFon.FileManager.Domain.Entity;

namespace MediaFon.FileManager.Infrastructure.Data
{
    public partial class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() { }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<File> Files { get; set; }
        public DbSet<Directory> Directories { get; set; }
        public DbSet<EventLogs> EventLogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            OnModelCreatingPartial(modelBuilder);

            modelBuilder
                .Entity<File>()
                .Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()");

            modelBuilder
                .Entity<Directory>()
                .HasKey(c=>c.Name);


        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
