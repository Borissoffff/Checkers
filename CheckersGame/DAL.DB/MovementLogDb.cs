using Microsoft.EntityFrameworkCore;
using ProjectDomain;

namespace DAL.DB;

public class MovementLogDb : BaseRepository, IMovementsLogRepository
{
    public ICollection<MovementLog> GetLogsByGameId(int id)
    {
        return DbContext.MovementLogs.Where(log => log.CheckersGameId == id).ToList();
    }

    public ICollection<MovementLog> GetAllLogs()
    {
        return DbContext.MovementLogs.ToList();
    }

    public MovementLogDb(AppDbContext dbContext) : base(dbContext)
    {
    }
}