namespace ProjectDomain;

public class CheckersState
{
    public EBoardPiece?[][] GameBoard = default!;
    public bool NextMoveByBlack { get; set; }
}