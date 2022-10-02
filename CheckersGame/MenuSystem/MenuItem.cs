namespace MenuSystem;

public class MenuItem
{
    public string Title { get; set; }
    public string Shortcut { get; set; }
    public Func<int>? MethodToRun { get; set; }

    public MenuItem(string shortcut, string title, Func<int>? methodToRun )
    {
        Shortcut = shortcut;
        Title = title;
        MethodToRun = methodToRun;
    }
    
    public override string ToString() => Shortcut + ") " + Title;
}