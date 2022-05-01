

using Microsoft.EntityFrameworkCore;
using File = MediaFon.FileManager.Domain.Entity.File;
using Directory = MediaFon.FileManager.Domain.Entity.Directory;


namespace MediaFon.FileManager.Infrastructure.Data
{
    public partial class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() { }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<File> Files { get; set; }
        public DbSet<Directory> Directories { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                 
                optionsBuilder.UseNpgsql("Host=localhost;Database=FileManagerDb;Username=postgres;Password=Admin@123");
            }
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
                .Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()");
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
