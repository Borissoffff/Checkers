using ProjectDomain;

namespace DAL;

public interface IGamesRepository
{
    // crud methods

    // read
    List<string> GetGamesList();
    
    CheckersGame GetGame(string id);

    // create and update
    void SaveGame(string id, CheckersGame game);
    
    // delete
    void DeleteGame(string id);
}