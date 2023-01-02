using ProjectDomain;
using static System.Console;

namespace ConsoleUI;

public static class Ui
{
    public static void DrawGameBoard(EBoardPiece[][] board, PossibleMoves? possibleMoves=null)
    {
        var cols = board.GetLength(0);
        var rows = board[0].GetLength(0);

        var numberList = Enumerable.Range(1, rows).Select(c => c).ToList();
        numberList.Reverse();
        
        var charList = GetCharList().Take(cols).ToList();

        List<Coordinate>? coordOfPossibleMoves = null;
        if (possibleMoves != null)
        {
            coordOfPossibleMoves = possibleMoves.AllPossibleMoves.ToList();
        }
        
        var checker = "O";
        var empty = " ";
        
        Clear();
        BackgroundColor = ConsoleColor.Black;
 
        Write("   " + string.Concat(Enumerable.Repeat("-----", rows)) + "--");
        WriteLine();
        for (int y = 0; y < rows; y++) {
            
            for (int x = 0; x < cols; x++)
            {
                if (x == 0)
                {
                    Write(numberList[y] > 9 ? $"{numberList[y]} |" : $" {numberList[y]} |");
                }

                string pieceStr = "";
                switch (board[x][y])
                {
                    case EBoardPiece.BlackSquare:
                        pieceStr = empty;
                        if (coordOfPossibleMoves != null 
                            && coordOfPossibleMoves.Any(coordinate => coordinate.X == x && coordinate.Y == y))
                        {
                            BackgroundColor = ConsoleColor.Yellow;
                        }
                        break;
                    case EBoardPiece.BlackSquareBlackChecker:
                        ForegroundColor = ConsoleColor.DarkYellow;
                        pieceStr = checker;
                        break;
                    case EBoardPiece.BlackSquareWhiteChecker:
                        pieceStr = checker;
                        break;
                    case EBoardPiece.BlackSquareBlackKing:
                        pieceStr = checker;
                        ForegroundColor = ConsoleColor.Red;
                        break;
                    case EBoardPiece.BlackSquareWhiteKing:
                        pieceStr = checker;
                        ForegroundColor = ConsoleColor.Blue;
                        break;
                    case EBoardPiece.WhiteSquare:
                        pieceStr = empty;
                        BackgroundColor = ConsoleColor.White;
                        break;
                }
                Write($"  {pieceStr}  ");
                ResetColor();
                
            }
            WriteLine("|");
        }
        Write("   " + string.Concat(Enumerable.Repeat("-----", rows)) + "--");
        WriteLine();
        Write("   ");
        charList.ForEach(p => Write($"   {p} "));
    }

    private static List<string> GetCharList()
    {
        var charList = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToList().ToList();
        var res = charList.Select(ch => ch.ToString()).ToList();
        foreach (var ch in charList)
        {
            res.AddRange(charList.Where(ch2 => !ch.Equals(ch2)).Select(ch2 => $"{ch}{ch2}"));
        }
        return res;
    }
}
