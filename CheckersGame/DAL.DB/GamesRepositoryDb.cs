using ProjectDomain;

namespace DAL.DB;

public class GamesRepositoryDb : IGamesRepository
{
    private readonly AppDbContext _dbContext;
    public string Name = "DB";

    public GamesRepositoryDb(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public List<string> GetGamesList()
    {
        return _dbContext
            .CheckersGames
            .OrderBy(game => game.Name)
            .Select(game => game.Name)
            .ToList();
    }

    public CheckersGame GetGame(string id)
    {
        return _dbContext.CheckersGames.First(game => game.Name == id);
    }

    public void SaveGame(string id, CheckersGame game)
    {
        _dbContext.CheckersGames.Add(game);
        _dbContext.SaveChanges();
    }

    public void DeleteGame(string id)
    {
        var gameToDelete = GetGame(id);
        _dbContext.CheckersGames.Remove(gameToDelete);
        _dbContext.SaveChanges();
    }
}