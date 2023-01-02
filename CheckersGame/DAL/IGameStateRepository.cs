using ProjectDomain;

namespace DAL;

public interface IGameStateRepository
{
    void AddState(CheckersGameState state);
    void GetState(int id);
    void GetLatestStateForGame(int gameId);
}