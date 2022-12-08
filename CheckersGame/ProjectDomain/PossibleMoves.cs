namespace ProjectDomain;

public class PossibleMoves
{
    public Coordinate CheckerToMove { get; set; }
    public List<Coordinate> AllPossibleMoves { get; set; }
    
    public PossibleMoves(Coordinate checkerToMove, List<Coordinate> allPossibleMoves)
    {
        CheckerToMove = checkerToMove;
        AllPossibleMoves = allPossibleMoves;
    }
}