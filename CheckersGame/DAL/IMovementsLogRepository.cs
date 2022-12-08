using ProjectDomain;

namespace DAL;

public interface IMovementsLogRepository
{
    ICollection<MovementLog> GetLogsByGameId(int id);
    ICollection<MovementLog> GetAllLogs();
}