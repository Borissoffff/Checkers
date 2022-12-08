using Microsoft.EntityFrameworkCore;
using ProjectDomain;

namespace DAL.DB;

public class GamesRepositoryDb : BaseRepository, IGamesRepository
{
    public GamesRepositoryDb(AppDbContext dbContext) : base(dbContext)
    {
    }
    public List<CheckersGame> GetGamesList()
    {
        return DbContext
            .CheckersGames
            .Include(o => o.CheckersOption)
            .OrderBy(g => g.StartedAt)
            .ToList();
    }
    public CheckersGame? GetGame(int? id)
    {
        return DbContext.CheckersGames
            .Include(g => g.CheckersOption)
            .Include(g => g.CheckersGameStates)
            .FirstOrDefault(g => g.Id == id);
    }

    public int GetLastGameId() => DbContext.CheckersGames.Any() 
        ? DbContext.CheckersGames.OrderBy(g => g.Id).Last().Id 
        : 0;

    public CheckersGame SaveGame(CheckersGame game, string? id = null)
    {
        DbContext.CheckersGames.Add(game);
        DbContext.SaveChanges();
        return game;
    }

    public void DeleteGame(int id)
    {
        var gameToDelete = GetGame(id);
        DbContext.CheckersGames.Remove(gameToDelete);
        DbContext.SaveChanges();
    }
}