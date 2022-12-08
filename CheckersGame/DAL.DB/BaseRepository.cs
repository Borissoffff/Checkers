namespace DAL.DB;

public abstract class BaseRepository : IBaseRepository
{
    public string Name { get; } = "SQLite DB";
    protected readonly AppDbContext DbContext;

    protected BaseRepository(AppDbContext dbContext)
    {
        DbContext = dbContext;
    }
    
    public void SaveChanges()
    {
        DbContext.SaveChanges();
    }
}