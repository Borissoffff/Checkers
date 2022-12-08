using ProjectDomain;

namespace DAL;

public interface IGameOptionsRepository : IBaseRepository
{
    // crud methods

    // read
    List<string> GetGameOptionsList();
    CheckersOption GetGameOptions(string id);
    
    // create and update
    void SaveGameOptions(string id, CheckersOption options);
    
    // delete
    void DeleteGameOptions(string id);
}