namespace MenuSystem;
using static Console;

public class Menu
{
    public static string CheckersTitle = @"
   _____ _               _                 
  / ____| |             | |                
 | |    | |__   ___  ___| | _____ _ __ ___ 
 | |    | '_ \ / _ \/ __| |/ / _ \ '__/ __|
 | |____| | | |  __/ (__|   <  __/ |  \__ \
  \_____|_| |_|\___|\___|_|\_\___|_|  |___/
    ";
    
    private int _selectedIndex;
    private readonly string[] _options;
    private readonly string _title;

    public Menu(string title, string[] options)
    {
        _title = title;
        _options = options;
        _selectedIndex = 0;
    }

    public void DisplayMenu()
    {
        WriteLine(CheckersTitle);
        ForegroundColor = ConsoleColor.Blue;
        WriteLine(_title);
        ResetColor();
        for (var i = 0; i < _options.Length; i++)
        {
            var currentOption = _options[i];
            ForegroundColor = i == _selectedIndex ? ConsoleColor.Yellow : ConsoleColor.White;
            
            WriteLine(currentOption);
        }
        ResetColor();
    }

    public int Run(Action? addInfo = null)
    {
        ConsoleKey keyPressed;
        do
        {
            Clear();
            addInfo?.Invoke();
            DisplayMenu();
            
            ConsoleKeyInfo keyInfo = ReadKey(true);
            keyPressed = keyInfo.Key;
            
            //update SelectedIndex based on arrow keys.
            if (keyPressed == ConsoleKey.UpArrow)
            {
                _selectedIndex--;
                if (_selectedIndex == -1)
                {
                    _selectedIndex = _options.Length - 1;
                }
            } 
            else if (keyPressed == ConsoleKey.DownArrow)
            {
                _selectedIndex++;
                if (_selectedIndex == _options.Length)
                {
                    _selectedIndex = 0;
                }
            }

        } while (keyPressed != ConsoleKey.Enter);

        return _selectedIndex;
    }


    public void ClearCheckersTitle() => CheckersTitle = "";
}

