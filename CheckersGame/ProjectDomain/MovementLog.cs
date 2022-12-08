namespace ProjectDomain;

public class MovementLog
{
    public int Id { get; set; }

   // private  list = new List<int>();
    
    
    public int CheckersGameId { get; set; }
    
    public CheckersGame? CheckersGame { get; set; }

    /*public int MovementFromId { get; set; }
    public DbCoordinate MovementFrom { get; set; } = default!;*/

    public int MovementFromX { get; set; } = default!;

    public int MovementFromY { get; set; } = default!;
    
    public int MovementToX { get; set; } = default!;

    public int MovementToY { get; set; } = default!;
    
    /*public int MovementToId { get; set; }
    
    public DbCoordinate MovementTo { get; set; } = default!;*/

    public string WhoMoved { get; set; } = default!;
    
    public int EatenCheckerX { get; set; }
    
    public int EatenCheckerY { get; set; }
    
    //private ICollection<DbCoordinate>? EatenCheckers { get; set; }
}