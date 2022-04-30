using System;
using System.Collections.Generic;
using MediaFon.FileManager.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace MediaFon.FileManager.Infrastructure.Data
{
    public partial class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() { }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<FileMetaData> Files { get; set; }

 

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
                .Entity<FileMetaData>()
                .Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()");

 
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
