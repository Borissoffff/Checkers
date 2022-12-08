namespace ConsoleAppCheckersGame;

public class Test
{
    public bool Flag { get; set; } = true;

    public void Foo()
    {
        Flag = !Flag;
    }
}