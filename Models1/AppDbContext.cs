using Microsoft.EntityFrameworkCore;
using StudentApi.Models1;

namespace StudentApi.Models1
{
    public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Student> Students { get; set; }
    public DbSet<Department> Departments { get; set; }
}
}

