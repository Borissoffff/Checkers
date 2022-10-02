
using System.Text.RegularExpressions;
using MenuSystem;

var thirdMenu = new Menu(
    EMenuLevel.Other,
    ">  Checkers Third level  <",
    new List<MenuItem>()
    {
        new MenuItem("N", "Nothing", null),
    }
);

var secondMenu = new Menu(
    EMenuLevel.Second,
    ">  Checkers Second level  <",
    new List<MenuItem>()
    {
        new MenuItem("T", "Third level", thirdMenu.Run),
    }
);

var mainMenu = new Menu(
    EMenuLevel.Main,
    ">  Checkers  <",
    new List<MenuItem>()
    {
        new MenuItem("N", "New Game", DoNewGame),
        new MenuItem("L", "Load Game", LoadGame),
        new MenuItem("O", "Options", secondMenu.Run)
    }
);

void RunMainMenu()
{
    int selectedIndex = mainMenu.Run();
    mainMenu.MenuItems.ElementAt(selectedIndex).Value.MethodToRun?.Invoke();
}

RunMainMenu();

//mainMenu.RunMenu();
// void RunMainMenu()
// {
//     int selectedIndex = mainMenu.Run();
//     mainMenu.MenuItems.ElementAt(selectedIndex).Value.MethodToRun?.Invoke();
//
//
//     // switch (selectedIndex)
//     // {
//     //     case 0:
//     //         break;
//     //     case 1:
//     //         mainMenu.MenuItems.ElementAt(1).Value.MethodToRun?.Invoke();
//     //         break;
//     //     case 2:
//     //         mainMenu.MenuItems.ElementAt(2).Value.MethodToRun?.Invoke();
//     //         break;
//     // }
// }
//
// RunMainMenu();

int DoNewGame()
{
    Console.WriteLine("New game method");
    return 0;
}

int LoadGame()
{
    Console.WriteLine("Load game");
    return 0;
}

void ExitGame()
{
    Environment.Exit(0);
}


