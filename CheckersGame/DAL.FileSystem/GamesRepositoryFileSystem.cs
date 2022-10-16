using ProjectDomain;

namespace DAL.FileSystem;

public class GamesRepositoryFileSystem : IGamesRepository
{
    private const string JsonExtension = "json";
    private readonly string _gamesDirectory = "." + Path.DirectorySeparatorChar + "games";

    public List<string> GetGamesList()
    {
        CheckOrCreateDirectory();

        var gamesFilesList = new List<string>();
        
        foreach (var fileName in Directory.GetFileSystemEntries(_gamesDirectory, "*." + JsonExtension))
        {
            gamesFilesList.Add(Path.GetFileNameWithoutExtension(fileName));
        }

        return gamesFilesList;
    }

    public CheckersGame GetGame(string id)
    {
        var fileContent = File.ReadAllText(GetFileName(id));
        var game = System.Text.Json.JsonSerializer.Deserialize<CheckersGame>(fileContent);
        if (game == null)
        {
            throw new NullReferenceException($"Could not deserialize: {fileContent}");
        }
        return game;
    }
    public void SaveGame(string id, CheckersGame game)
    {
        CheckOrCreateDirectory();
        var fileContent = System.Text.Json.JsonSerializer.Serialize(game);
        File.WriteAllText(GetFileName(id), fileContent);
    }

    public void DeleteGame(string id)
    {
        File.Delete(GetFileName(id));

    }
    
    private void CheckOrCreateDirectory()
    {
        if (!Directory.Exists(_gamesDirectory))
        {
            Directory.CreateDirectory(_gamesDirectory);
        }
    }
    
    public string GetFileName(string id)
    {
        return _gamesDirectory +
               Path.DirectorySeparatorChar +
               id + "." + JsonExtension;
    }
}