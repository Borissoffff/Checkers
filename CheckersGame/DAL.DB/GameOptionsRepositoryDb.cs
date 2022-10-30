using ProjectDomain;

namespace DAL.DB;

public class GameOptionsRepositoryDb : IGameOptionsRepository
{
    private readonly AppDbContext _dbContext;
    public string Name = "DB";

    public GameOptionsRepositoryDb(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public List<string> GetGameOptionsList()
    {
        return _dbContext
            .CheckersOptions
            .OrderBy(option => option.Name)
            .Select(option => option.Name)
            .ToList();
    }

    public CheckersOption GetGameOptions(string id)
    {
        return _dbContext.CheckersOptions.First(option => option.Name == id);
    }

    public void SaveGameOptions(string id, CheckersOption options)
    {
        if (_dbContext.CheckersOptions.Any(o => o.Name == options.Name)) return;
        
        _dbContext.CheckersOptions.Add(options);
        _dbContext.SaveChanges();

    }

    public void DeleteGameOptions(string id)
    {
        var options = GetGameOptions(id);
        _dbContext.CheckersOptions.Remove(options);
        _dbContext.SaveChanges();
    }
}