using Microsoft.EntityFrameworkCore;
using School.Domain.Core;
using School.Domain.Entities;

namespace School.Infrastructure.Context;

public sealed class SchoolContext(DbContextOptions<SchoolContext> options)
    : DbContext(options)
{
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Instructor> Instructors => Set<Instructor>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Person>()
            .HasDiscriminator<string>("PersonType")
            .HasValue<Student>("Student")
            .HasValue<Instructor>("Instructor");
        modelBuilder.Entity<Person>()
            .HasIndex(person => person.Email)
            .IsUnique();

        modelBuilder.Entity<Department>()
            .HasIndex(department => department.Code)
            .IsUnique();
        modelBuilder.Entity<Department>()
            .Property(department => department.Code)
            .HasMaxLength(20)
            .IsRequired();
        modelBuilder.Entity<Department>()
            .Property(department => department.Name)
            .HasMaxLength(120)
            .IsRequired();

        modelBuilder.Entity<Course>()
            .HasIndex(course => course.Code)
            .IsUnique();
        modelBuilder.Entity<Course>()
            .HasOne(course => course.Department)
            .WithMany(department => department.Courses)
            .HasForeignKey(course => course.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Course>()
            .HasOne(course => course.Instructor)
            .WithMany(instructor => instructor.Courses)
            .HasForeignKey(course => course.InstructorId)
            .OnDelete(DeleteBehavior.SetNull);
        modelBuilder.Entity<Course>()
            .HasMany(course => course.Students)
            .WithMany(student => student.Courses)
            .UsingEntity("StudentCourses");

        modelBuilder.Entity<Student>()
            .HasIndex(student => student.StudentNumber)
            .IsUnique();
        modelBuilder.Entity<Student>()
            .HasOne(student => student.Department)
            .WithMany(department => department.Students)
            .HasForeignKey(student => student.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Instructor>()
            .HasIndex(instructor => instructor.EmployeeNumber)
            .IsUnique();
        modelBuilder.Entity<Instructor>()
            .HasOne(instructor => instructor.Department)
            .WithMany(department => department.Instructors)
            .HasForeignKey(instructor => instructor.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Modified)
            {
                entry.Entity.MarkUpdated();
            }
        }
    }
}
