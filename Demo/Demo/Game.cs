using MenuSystemDemo;

namespace Demo;
using static System.Console;

public class Game
{
    public void Start()
    {
        RunMainMenu();
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
                displayOptions();
                break;
            case 3:
                Exit();
                break;
        }
    }

    private void Exit()
    {
        Environment.Exit(0);
    }

    private void CreateNewGame()
    {
        WriteLine("New game");
    }

    private void LoadGame()
    {
        WriteLine("Load Game");
    }

    private void displayOptions()
    {
        string title = "Options menu";
        string[] options = { "Option 1", "Option 2", "Option 3", "Back" };
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
                runThirdMenu();
                break;
            case 3:
                RunMainMenu();
                break;
        }
    }

    private void runThirdMenu()
    {
        string title = "Third menu";
        string[] options = { "Go Back", "Go to Main Menu" };
        Menu thirdMenu = new Menu(title, options);
        int selectedIndex = thirdMenu.Run();

        switch (selectedIndex)
        {
            case 0:
                displayOptions();
                break;
            case 1:
                RunMainMenu();
                break;
        }
    }
}