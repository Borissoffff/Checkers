namespace MenuSystem;
using static Console;

public class Menu
{
    public static string ChessTitle = @"
    _______  __   __  _______  _______  _______ 
    |       ||  | |  ||       ||       ||       |
    |       ||  |_|  ||    ___||  _____||  _____|
    |       ||       ||   |___ | |_____ | |_____ 
    |      _||       ||    ___||_____  ||_____  |
    |     |_ |   _   ||   |___  _____| | _
        ____| |
    |_______||__| |__||_______||_______||_______|
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
        WriteLine(ChessTitle);
        WriteLine(_title);
        for (var i = 0; i < _options.Length; i++)
        {
            var currentOption = _options[i];
            ForegroundColor = i == _selectedIndex ? ConsoleColor.Yellow : ConsoleColor.White;
            
            WriteLine(currentOption);
        }
        ResetColor();
    }

    public int Run()
    {
        ConsoleKey keyPressed;
        do
        {
            Clear();
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
}

