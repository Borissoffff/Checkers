using ProjectDomain;

namespace DAL.FileSystem;

public class GameOptionsRepositoryFileSystem : IGameOptionsRepository
{
    private const string JsonExtension = "json";
    private readonly string _optionsDirectory = "." + Path.DirectorySeparatorChar + "options";
    public string Name => "File System";
    public void SaveChanges(int? id)
    {
        throw new NotImplementedException("save changes for file system does not work");
    }

    public List<string> GetGameOptionsList()
    {
        CheckOrCreateDirectory();

        var optionsFilesList = new List<string>();
        
        foreach (var fileName in Directory.GetFileSystemEntries(_optionsDirectory, "*." + JsonExtension))
        {
            optionsFilesList.Add(Path.GetFileNameWithoutExtension(fileName));
        }

        return optionsFilesList;
    }

    public CheckersOption GetGameOptions(string id)
    {
        var fileContent = File.ReadAllText(GetFileName(id));
        var options = System.Text.Json.JsonSerializer.Deserialize<CheckersOption>(fileContent);
        if (options == null)
        {
            throw new NullReferenceException($"Could not deserialize: {fileContent}");
        }

        return options;
    }

    public void SaveGameOptions(string id, CheckersOption options)
    {
        CheckOrCreateDirectory();
        var fileContent = System.Text.Json.JsonSerializer.Serialize(options);
        File.WriteAllText(GetFileName(id), fileContent);
    }

    public void DeleteGameOptions(string id)
    {
        File.Delete(GetFileName(id));
    }
    
    private void CheckOrCreateDirectory()
    {
        if (!Directory.Exists(_optionsDirectory))
        {
            Directory.CreateDirectory(_optionsDirectory);
        }
    }

    public string GetFileName(string id)
    {
        return _optionsDirectory +
               Path.DirectorySeparatorChar +
               id + "." + JsonExtension;
    }
}