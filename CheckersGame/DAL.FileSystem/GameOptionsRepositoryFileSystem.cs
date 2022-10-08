using ProjectDomain;

namespace DAL.FileSystem;

public class GameOptionsRepositoryFileSystem : IGameOptionsRepository
{
    public List<string> GetGameOptionsList()
    {
        throw new NotImplementedException();
    }

    public CheckersOption GetGameOptions(string id)
    {
        throw new NotImplementedException();
    }

    public void SaveGameOptions(string id, CheckersOption options)
    {
        throw new NotImplementedException();
    }

    public void DeleteGameOptions(string id)
    {
        throw new NotImplementedException();
    }
}