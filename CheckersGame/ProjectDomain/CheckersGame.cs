namespace ProjectDomain;

public class CheckersGame
{
    public string Name = default!;
    public EBoardPiece?[][] currentState;
    public List<EBoardPiece[][]> states = new List<EBoardPiece[][]>();
}