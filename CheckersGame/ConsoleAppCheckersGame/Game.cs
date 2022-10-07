using ConsoleUI;
using GameBrain;
using MenuSystem;

namespace ConsoleAppCheckersGame;

using static Console;

public class Game
{
    public void Start()
    {
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
                ChooseBoardSizeMenu();
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
        string[] options = { "Option 1", "Option 2", "Option 3", "Go Back" };
        Menu optionsMenu = new Menu(title, options);
        int selectedIndex = optionsMenu.Run();

        switch (selectedIndex)
        {
            case 0:
                WriteLine("Option 11111111111");
                break;
            case 1:
                WriteLine("Option 22222222222");
                break;
            case 2:
                RunThirdMenu();
                break;
            case 3:
                RunMainMenu();
                break;
        }
    }

    public void ChooseBoardSizeMenu()
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
    }
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

    private void CreateNewGame(int boardSize)
    {
        var game = new CheckersBrain(boardSize, boardSize);
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
}