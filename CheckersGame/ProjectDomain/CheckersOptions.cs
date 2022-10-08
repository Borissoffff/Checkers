namespace ProjectDomain;

public class CheckersOption
{
    public int Width { get; set; } = 8;
    public int Height { get; set; } = 8;
    public int RandomMoves { get; set; } = 0;
    public bool WhiteStarts { get; set; } = true;

    public override string ToString()
    {
        return $"Board: {Width}x{Height} Random: {RandomMoves} WhiteStarts:{WhiteStarts}";
    }
}
