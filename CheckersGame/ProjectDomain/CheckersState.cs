namespace ProjectDomain;

public class CheckersState
{
    public EBoardPiece[][] GameBoard { get; set; } = default!;
    public bool NextMoveByBlack { get; set; }

    public bool CheckerCanEatAgain { get; set; }
}