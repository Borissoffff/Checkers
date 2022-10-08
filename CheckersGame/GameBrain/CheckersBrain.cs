using ProjectDomain;

namespace GameBrain;

public class CheckersBrain
{
    private readonly EBoardPiece[,] _board;
    public CheckersBrain(int boardWidth, int boardHeight)
    {
        _board = new EBoardPiece[boardWidth, boardHeight];

        var count = 0;
        
        var opponentsRows = new[] { 0, 1, 2 };
        var myRows = new[] { boardWidth - 3, boardWidth - 2, boardWidth - 1};
        
        for (var i = 0; i < boardWidth; i++)
        {
            for (var j = 0; j < boardHeight; j++)
            {
                if (count % 2 == 0)
                {
                    _board[i, j] = EBoardPiece.WhiteSquare;
                    count++;
                }
                else
                {
                    if (opponentsRows.Contains(i))
                    {
                        _board[i, j] = EBoardPiece.BlackSquareRedChecker;
                        count++;
                    }
                    else if (myRows.Contains(i))
                    {
                        _board[i, j] = EBoardPiece.BlackSquareWhiteChecker;
                        count++;
                    }
                    else
                    {
                        _board[i, j] = EBoardPiece.BlackSquare;
                        count++; 
                    }
                }
            }
            count++;
        }
    }

    public EBoardPiece[,] GetBoard()
    {
        var board = new EBoardPiece[_board.GetLength(0), _board.GetLength(1)];
        Array.Copy(_board, board, _board.Length);
        return board;
    }
}