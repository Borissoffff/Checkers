namespace MenuSystem;
using static Console;

public class Menu
{
    private const string ChessTitle = @"
    _______  __   __  _______  _______  _______ 
    |       ||  | |  ||       ||       ||       |
    |       ||  |_|  ||    ___||  _____||  _____|
    |       ||       ||   |___ | |_____ | |_____ 
    |      _||       ||    ___||_____  ||_____  |
    |     |_ |   _   ||   |___  _____| | _
        ____| |
    |_______||__| |__||_______||_______||_______|
    ";

     private int SelectedIndex;
     
     private const string ShortcutExit = "X";
     private const string ShortcutGoBack = "B";
     private const string ShortcutGoToMain = "M";
     public string Title { get; set; }
     private readonly EMenuLevel _level;

     public Dictionary<string, MenuItem> MenuItems = new Dictionary<string, MenuItem>();

     private readonly MenuItem _menuItemExit = new MenuItem(ShortcutExit, "Exit", null);
     private readonly MenuItem _menuItemGoBack = new MenuItem(ShortcutGoBack, "Back", null);
     private readonly MenuItem _menuItemGoToMain = new MenuItem(ShortcutGoToMain, "Main menu", null);

    public Menu(EMenuLevel level, string title, List<MenuItem> menuItems)
    {
        _level = level;
        Title = title;
        SelectedIndex = 0;
        foreach (var menuItem in menuItems)
        {
            MenuItems.Add(menuItem.Shortcut, menuItem);
        }

        if (_level != EMenuLevel.Main)
            MenuItems.Add(ShortcutGoBack, _menuItemGoBack);

        if (_level == EMenuLevel.Other)
            MenuItems.Add(ShortcutGoToMain, _menuItemGoToMain);

        MenuItems.Add(ShortcutExit, _menuItemExit);
    }

    public void DisplayOptions()
    {
        Clear();
        WriteLine(ChessTitle);
        WriteLine(Title);
        for (var i = 0; i < MenuItems.Count; i++)
        {
            var option = MenuItems.ElementAt(i).Value;
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
            WriteLine($"{prefix} << {option} >>");
        }
        ResetColor();
    }

    public int Run()
    {
        Clear();
        ConsoleKey keyPressed;
        do
        {
            DisplayOptions();
            var keyInfo = ReadKey(true);
            keyPressed = keyInfo.Key;

            if (keyPressed == ConsoleKey.UpArrow)
            {
                SelectedIndex--;
                if (SelectedIndex == -1)
                {
                    SelectedIndex = MenuItems.Count - 1;
                }
            } 
            else if (keyPressed == ConsoleKey.DownArrow)
            {
                SelectedIndex++;
                if (SelectedIndex == MenuItems.Count)
                {
                    SelectedIndex = 0;
                }
            }
            
        } while (keyPressed != ConsoleKey.Enter);
        return SelectedIndex;
    }

    /*public string RunMenu()
    {
        var menuDone = false;
        var userChoice = "";
        do
        {
            /*Console.WriteLine(Title);
            Console.WriteLine("===================");#1#
            DisplayOptions();
            /*foreach (var menuItem in _menuItems.Values)
            {
                Console.WriteLine(menuItem);
            }#1#
            Console.WriteLine("-------------------");
            Console.Write("Your choice:");
            userChoice = Console.ReadLine()?.ToUpper().Trim() ?? "";

            if (MenuItems.ContainsKey(userChoice))
            {
                string? runReturnValue = null;
                if (MenuItems[userChoice].MethodToRun != null)
                {
                   runReturnValue = MenuItems[userChoice].MethodToRun!();
                }

                if (userChoice == ShortcutGoBack)
                {
                    menuDone = true;
                }

                if (runReturnValue == ShortcutExit || userChoice == ShortcutExit)
                {
                    userChoice = runReturnValue ?? userChoice;
                    menuDone = true;
                }
                
                if ((userChoice == ShortcutGoToMain || runReturnValue == ShortcutGoToMain) && _level != EMenuLevel.Main)
                {
                    userChoice = runReturnValue ?? userChoice;
                    menuDone = true;
                }

            }
            else
            {
                Console.WriteLine("Wrong input");
                Console.WriteLine();
            }
        } while (menuDone == false);

        return userChoice;
    }*/
}
