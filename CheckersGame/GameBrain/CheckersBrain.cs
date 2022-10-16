using ProjectDomain;

namespace GameBrain;

public class CheckersBrain
{
    //private readonly EBoardPiece[,] _board;
    private readonly CheckersGameState _state;
    public CheckersBrain(CheckersOption options)
    {
        
        //_board = new EBoardPiece[boardWidth, boardHeight];
        var boardWidth = options.Width;
        var boardHeight = options.Height;

        _state = new CheckersGameState();

        _state.GameBoard = new EBoardPiece?[boardWidth][];
        for (int i = 0; i < boardWidth; i++)
        {
            _state.GameBoard[i] = new EBoardPiece?[boardHeight];
        }
        
        var count = 0;
        
        var opponentsRows = new[] { 0, 1, 2 };
        var myRows = new[] { boardWidth - 3, boardWidth - 2, boardWidth - 1};
        
        for (var i = 0; i < boardWidth; i++)
        {
            for (var j = 0; j < boardHeight; j++)
            {
                if (count % 2 == 0)
                {
                    _state.GameBoard[i][j] = EBoardPiece.WhiteSquare;
                    count++;
                }
                else
                {
                    if (opponentsRows.Contains(i))
                    {
                        _state.GameBoard[i][j] = EBoardPiece.BlackSquareRedChecker;
                        count++;
                    }
                    else if (myRows.Contains(i))
                    {
                        _state.GameBoard[i][j] = EBoardPiece.BlackSquareWhiteChecker;
                        count++;
                    }
                    else
                    {
                        _state.GameBoard[i][j] = EBoardPiece.BlackSquare;
                        count++; 
                    }
                }
            }
            count++;
        }
    }

    public EBoardPiece?[][] GetBoard()
    {
        var jsonStr = System.Text.Json.JsonSerializer.Serialize(_state.GameBoard);
        return System.Text.Json.JsonSerializer.Deserialize<EBoardPiece?[][]>(jsonStr)!;
    }
}