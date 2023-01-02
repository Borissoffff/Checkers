namespace ProjectDomain;

public class MovementLog
{
    public int Id { get; set; }

    public int CheckersGameId { get; set; }
    
    public CheckersGame? CheckersGame { get; set; }

    public int MovementFromX { get; set; } = default!;

    public int MovementFromY { get; set; } = default!;
    
    public int MovementToX { get; set; } = default!;

    public int MovementToY { get; set; } = default!;

    public string WhoMoved { get; set; } = default!;
    
    public int EatenCheckerX { get; set; }
    
    public int EatenCheckerY { get; set; }
}