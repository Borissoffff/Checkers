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
        
        var charList = GetCharList().Take(cols).ToList();

        var checker = "O";
        var empty = " ";
        
        Clear();
        BackgroundColor = ConsoleColor.Black;
 
        Write("   " + string.Concat(Enumerable.Repeat("-----", rows)) + "--");
        WriteLine();
        for (int i = 0; i < rows; i++) {
            
            for (int j = 0; j < cols; j++)
            {
                if (j == 0)
                {
                    Write(numberList[i] > 9 ? $"{numberList[i]} |" : $" {numberList[i]} |");
                }

                string pieceStr = "";
                switch (board[i, j])
                {
                    case EBoardPiece.BlackSquare:
                        pieceStr = empty;
                        break;
                    case EBoardPiece.BlackSquareRedChecker:
                        ForegroundColor = ConsoleColor.Red;
                        pieceStr = checker;
                        break;
                    case EBoardPiece.BlackSquareWhiteChecker:
                        pieceStr = checker;
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
