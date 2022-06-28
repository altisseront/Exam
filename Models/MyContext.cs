#pragma warning disable CS8618

using Microsoft.EntityFrameworkCore;
namespace Exam.Models;

public class MyContext : DbContext 
{ 
    public MyContext(DbContextOptions options) : base(options) { }
    public DbSet<User> Users { get; set; } 
    public DbSet<MeetUp> MeetUps { get; set; } 
    public DbSet<Association> Associations { get; set; } 
}