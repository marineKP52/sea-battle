namespace Server.Data;
using Microsoft.EntityFrameworkCore;
using Server.Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    public DbSet<User> Users { get; set; }
    public DbSet<UserStats> UserStats {get; set;}
    public DbSet<Review> Reviews { get; set; }
}