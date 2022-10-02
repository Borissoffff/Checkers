namespace MenuSystemDemo;
using static System.Console;

public class Menu
{
    private int SelectedIndex;
    private string[] Options;
    private string Title;

    public Menu(string title, string[] options)
    {
        Title = title;
        Options = options;
        SelectedIndex = 0;
    }

    public void DisplayOptions()
    {
        WriteLine(Title);
        for (int i = 0; i < Options.Length; i++)
        {
            string currentOption = Options[i];
            string prefix;
            if (i == SelectedIndex)
            {
                prefix = "*";
                ForegroundColor = ConsoleColor.Black;
                BackgroundColor = ConsoleColor.White;
            }
            else
            {
                prefix = " ";
                ForegroundColor = ConsoleColor.White;
                BackgroundColor = ConsoleColor.Black;
            }
            
            WriteLine($"<< {currentOption} >>");
        }
        ResetColor();
    }

    public int Run()
    {
        ConsoleKey keyPressed;
        do
        {
            Clear();
            DisplayOptions();
            
            ConsoleKeyInfo keyInfo = ReadKey(true);
            keyPressed = keyInfo.Key;
            
            //update SelectedIndex based on arrow keys.
            if (keyPressed == ConsoleKey.UpArrow)
            {
                SelectedIndex--;
                if (SelectedIndex == -1)
                {
                    SelectedIndex = Options.Length - 1;
                }
            } 
            else if (keyPressed == ConsoleKey.DownArrow)
            {
                SelectedIndex++;
                if (SelectedIndex == Options.Length)
                {
                    SelectedIndex = 0;
                }
            }

        } while (keyPressed != ConsoleKey.Enter);

        return SelectedIndex;
    }
}