using Microsoft.EntityFrameworkCore;
using Quizer.Models;

namespace Quizer.Context;

public class ApplicationContext : DbContext
{
    public DbSet<Users> Users { get; set; }
    public DbSet<Groups> Groups { get; set; }
    public DbSet<Subjects> Subjects { get; set; }
    public DbSet<Tasks> Tasks { get; set; } 
    public DbSet<Admin> Admin { get; set; }
    public DbSet<Teacher> Teachers { get; set; }

    public ApplicationContext()
    {
        Database.EnsureCreated();
    }

    public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=Quizer;Username=postgres;" +
                                 "Password=Hofman95");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Teacher>()
            .HasMany(x => x.Tasks)
            .WithOne(x => x.Teachers)
            .HasForeignKey(x => x.teacherid);
    }

}