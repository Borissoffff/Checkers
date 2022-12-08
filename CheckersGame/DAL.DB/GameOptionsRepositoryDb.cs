using ProjectDomain;

namespace DAL.DB;

public class GameOptionsRepositoryDb : BaseRepository, IGameOptionsRepository
{
    public GameOptionsRepositoryDb(AppDbContext dbContext) : base(dbContext)
    {
    }

    public List<string> GetGameOptionsList()
    {
        return DbContext
            .CheckersOptions
            .OrderBy(option => option.Name)
            .Select(option => option.Name)
            .ToList();
    }

    public CheckersOption GetGameOptions(string id)
    {
        return DbContext.CheckersOptions.First(option => option.Name == id);
    }

    public void SaveGameOptions(string id, CheckersOption options)
    {
        if (DbContext.CheckersOptions.Any(o => o.Name == options.Name)) return;
        
        DbContext.CheckersOptions.Add(options);
        DbContext.SaveChanges();

    }

    public void DeleteGameOptions(string id)
    {
        var options = GetGameOptions(id);
        DbContext.CheckersOptions.Remove(options);
        DbContext.SaveChanges();
    }

}