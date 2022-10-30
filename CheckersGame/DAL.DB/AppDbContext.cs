using Microsoft.EntityFrameworkCore;
using ProjectDomain;

namespace DAL.DB;


public class AppDbContext : DbContext
{
    public DbSet<CheckersGame> CheckersGames { get; set; }
    public DbSet<CheckersOption> CheckersOptions { get; set; }
    public DbSet<CheckersGameState> CheckersGameStates { get; set; }
    
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }

}