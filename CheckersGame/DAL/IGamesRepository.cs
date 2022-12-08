using ProjectDomain;

namespace DAL;

public interface IGamesRepository : IBaseRepository
{
    // crud methods

    // read
    List<CheckersGame> GetGamesList();
    
    CheckersGame? GetGame(int? id);
    int GetLastGameId();

    // create and update
    CheckersGame SaveGame(CheckersGame game, string? id=null);
    
    // delete
    void DeleteGame(int id);
}