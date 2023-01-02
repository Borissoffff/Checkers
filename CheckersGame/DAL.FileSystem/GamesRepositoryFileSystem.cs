using ProjectDomain;

namespace DAL.FileSystem;

public class GamesRepositoryFileSystem : IGamesRepository
{
    private const string JsonExtension = "json";
    private readonly string _gamesDirectory = "." + Path.DirectorySeparatorChar + "games";
    public string Name => "File System";
    public void SaveChanges(int? id)
    {
        
    }

    public void SaveChanges() { }

    public List<CheckersGame> GetGamesList()
    {
        if (DirectoryIsEmpty(_gamesDirectory))
        {
            throw new RankException("Games directory is empty");
        }
        var games = new List<CheckersGame>();
        foreach (var fileName in Directory.GetFileSystemEntries(_gamesDirectory, "*." + JsonExtension))
        {
            var file = Path.GetFileNameWithoutExtension(fileName);
            var fileNameInt = Int32.Parse(file);
            games.Add(GetGame(fileNameInt)!);
        }

        return games;
    }

    public CheckersGame? GetGame(int? id)
    {
        if (id == null) return null;
        var strId = id.ToString();
        var fileContent = File.ReadAllText(GetFileName(strId));
        var game = System.Text.Json.JsonSerializer.Deserialize<CheckersGame>(fileContent);
        if (game == null)
        {
            throw new NullReferenceException($"Could not deserialize: {fileContent}");
        }
        return game;
    }

    public int GetLastGameId()
    {
        if (DirectoryIsEmpty(_gamesDirectory)) return 0;
        var files = new List<string>();
        foreach (var fileName in Directory.GetFileSystemEntries(_gamesDirectory, "*." + JsonExtension))
        {
            var file = Path.GetFileNameWithoutExtension(fileName);
            files.Add(file);
        }

        return int.Parse(files.Last());
    }

    public CheckersGame SaveGame(CheckersGame game, string? id)
    {
        CheckOrCreateDirectory();
        var fileContent = System.Text.Json.JsonSerializer.Serialize(game);
        File.WriteAllText(GetFileName(id), fileContent);
        return game;
    }

    public void DeleteGame(int id)
    {
        File.Delete(GetFileName(id.ToString()));
    }

    private void CheckOrCreateDirectory()
    {
        if (!Directory.Exists(_gamesDirectory))
        {
            Directory.CreateDirectory(_gamesDirectory);
        }
    }
    
    private string GetFileName(string? id)
    {
        return _gamesDirectory +
               Path.DirectorySeparatorChar +
               id + "." + JsonExtension;
    }
    
    private bool DirectoryIsEmpty(string path)
    {
        return !Directory.EnumerateFileSystemEntries(path).Any();
    }
}