/*using System.Xml.Linq;
using ConsoleUI;
using DAL;
using DAL.FileSystem;
using GameBrain;
using MenuSystem;
using ProjectDomain;

namespace ConsoleAppCheckersGame;

using static Console;

public class Game
{
    
    
    
    public void Start()
    {
        CheckersOption gameOptions = new CheckersOption();
        IGameOptionsRepository repo = new GameOptionsRepositoryFileSystem();
        CheckersBrain game = new CheckersBrain(gameOptions);
        
        repo.SaveGameOptions("test", gameOptions);
        repo.SaveGameOptions("test2", gameOptions);
        
    
        RunMainMenu();
        WriteLine();
        WriteLine("Press any key to exit ...");
        ReadKey(true);
    }

    public void RunMainMenu()
    {
        string title = "Main Menu";
        string[] options = { "New Game", "Load Game", "Options", "Exit" };
        Menu mainMenu = new Menu(title, options);
        int selectedIndex = mainMenu.Run();

        switch (selectedIndex)
        {
            case 0:
                CreateNewGame();
                break;
            case 1:
                LoadGame();
                break;
            case 2:
                DisplayOptions();
                break;
            case 3:
                Exit();
                break;
        }
    }
    
    private void DisplayOptions()
    {
        string title = "Options menu";
        string[] options = { "Create Options", "Display Saved Options",
                            "Load Options", "Delete Options",
                            "Save current Options", "Edit current Options" };
        Menu optionsMenu = new Menu(title, options);
        int selectedIndex = optionsMenu.Run();

        switch (selectedIndex)
        {
            case 0:
                WriteLine("Option 11111111111");
                break;
            case 1:
                ListOptions();
                break;
            case 2:
                LoadGameOptions();
                break;
            case 3:
                RunMainMenu();
                break;
        }
    }

    private void LoadGameOptions()
    {
        Console.Write("Options name:");
        var optionsName = Console.ReadLine();
        gameOptions = repo.GetGameOptions(optionsName);

    }

    private void ListOptions()
    {
        foreach (var fileName in repo.GetGameOptionsList())
        {
            WriteLine(fileName);
        }
    }

    /*public void ChooseBoardSizeMenu()
    {
        string title = "Choose the board size you want to play.";
        string[] options = { "8x8", "10x10", "Choose your own" };
        var menu = new Menu(title, options);
        var selectedIndex = menu.Run();
        
        switch (selectedIndex)
        {
            case 0:
                CreateNewGame(8);
                break;
            case 1:
                CreateNewGame(10);
                break;
            case 2:
                var size = CustomBoardSizeMenu();
                CreateNewGame(size);
                break;
        }
    }#1#
    private void RunThirdMenu()
    {
        string title = "Third menu";
        string[] options = { "Go Back", "Go to Main Menu" };
        Menu thirdMenu = new Menu(title, options);
        int selectedIndex = thirdMenu.Run();

        switch (selectedIndex)
        {
            case 0:
                DisplayOptions();
                break;
            case 1:
                RunMainMenu();
                break;
        }
    }
    private void Exit()
    {
        Environment.Exit(0);
    }

    private void CreateNewGame()
    {
        game = new CheckersBrain(gameOptions);
        Ui.DrawGameBoard(game.GetBoard());
    }

    private void LoadGame()
    {
        WriteLine("Load Game");
    }

    private int CustomBoardSizeMenu()
    {
        Clear();
        WriteLine(Menu.ChessTitle);
        var flag = false;
        int res = 0;
        do
        {
            ForegroundColor = ConsoleColor.White;
            WriteLine("Type your board size!\n" +
                  "(number should be even)]n" +
                  "(board size cannot be less than 8)");
            
            var size = ReadLine();
            if (size == null) continue;
            
            try
            {
                res = int.Parse(size);
            }
            catch (Exception)
            {
                // ignore
            }

            if (res % 2 != 0)
            {
                ForegroundColor = ConsoleColor.Red;
                WriteLine("Please enter even number");
            }
            else if (res < 8)
            {
                ForegroundColor = ConsoleColor.Red;
                WriteLine("Board size cannot be less than 8");
            }
            else
            {
                flag = true;
            }
        } while (flag == false);

        return res;
    }
}*/