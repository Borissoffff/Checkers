using ProjectDomain;
using static System.Console;

namespace ConsoleUI;

public static class Ui
{
    public static void DrawGameBoard(EBoardPiece[,] board)
    {
        var cols = board.GetLength(0);
        var rows = board.GetLength(1);

        var numberList = Enumerable.Range(1, rows).Select(c => c).ToList();
        numberList.Reverse();
        
        //var charList = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToList().Take(cols).ToList();
        var charList = GetCharList().Take(cols).ToList();

        var checker = "O";
        var empty = " ";
        
        Clear();
        for (int i = 0; i < rows; i++) {
            Write("   ");
            for (int j = 0; j < cols; j++)
            {
                BackgroundColor = ConsoleColor.Black;
                Write("+-----");
            }
            WriteLine("+");

            for (int j = 0; j < cols; j++)
            {
                if (j == 0)
                {
                    Write(numberList[i] > 9 ? $"{numberList[i]} " : $" {numberList[i]} ");
                }
                BackgroundColor = ConsoleColor.Black;
                Write("|");
                string pieceStr;
                switch (board[i, j])
                {
                    case EBoardPiece.BlackSquare:
                        BackgroundColor = ConsoleColor.Black;
                        pieceStr = empty;
                        break;
                    case EBoardPiece.BlackSquareRedChecker:
                        BackgroundColor = ConsoleColor.Black;
                        ForegroundColor = ConsoleColor.Red;
                        pieceStr = checker;
                        break;
                    case EBoardPiece.BlackSquareWhiteChecker:
                        BackgroundColor = ConsoleColor.Black;
                        ForegroundColor = ConsoleColor.White;
                        pieceStr = checker;
                        break;
                    default:
                        BackgroundColor = ConsoleColor.White;
                        pieceStr = empty;
                        break;
                }
                Write("  ");
                Write(pieceStr);
                Write("  ");
                ForegroundColor = ConsoleColor.Black;

            }

            BackgroundColor = ConsoleColor.Black;
            WriteLine("|");
        }
        Write("   ");
        for (int i = 0; i < cols; i++)
        {
            Write("+-----");
        }
        WriteLine("+");
        Write("   ");
        charList.ForEach(p => Write($"   {p}  "));
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