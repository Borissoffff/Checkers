using ConsoleUI;
using DAL;
using DAL.DB;
using DAL.FileSystem;
using GameBrain;
using MenuSystem;
using Microsoft.EntityFrameworkCore;
using ProjectDomain;
using static System.Console;

//dotnet ef database update --project DAL.DB --startup-project ConsoleAppCheckersGame

var databaseEngine = "File System";

var dbOptions =
    new DbContextOptionsBuilder<AppDbContext>()
        .UseSqlite("Data Source=/Users/jegor/CheckersDB/checkers.db")
        .Options;

var ctx = new AppDbContext(dbOptions);
IGameOptionsRepository optionsRepoDb = new GameOptionsRepositoryDb(ctx);
IGamesRepository gamesRepoDb = new GamesRepositoryDb(ctx);

IGameOptionsRepository optionsRepoFs = new GameOptionsRepositoryFileSystem();
IGamesRepository gamesRepoFs = new GamesRepositoryFileSystem();

//initialize two fundamental checkers settings and add them to FileSystem and SQLite
var angloAmericanCheckersVersion = new CheckersOption
{
    Name = "The Anglo-American version"
};

var continentalVersion = new CheckersOption
{
    Name = "Continental Version",
    Height = 10,
    Width = 10
};

optionsRepoFs.SaveGameOptions(angloAmericanCheckersVersion.Name, angloAmericanCheckersVersion);
optionsRepoFs.SaveGameOptions(continentalVersion.Name, continentalVersion);

optionsRepoDb.SaveGameOptions(angloAmericanCheckersVersion.Name, angloAmericanCheckersVersion);
optionsRepoDb.SaveGameOptions(continentalVersion.Name, continentalVersion);

//Anglo-American version is default version
var currentGameOptions = angloAmericanCheckersVersion;

var optionsRepo = optionsRepoFs;
var gamesRepo = gamesRepoFs;

RunMainMenu();
WriteLine();
WriteLine("Press any key to exit ...");
ReadKey(true);

void RunMainMenu()
{
    string title = "Main Menu";
    string[] options = { "New Game", "Load Game", "Options", "Data Management Method Swap", "Exit" };
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
            DisplayOptionsMenu();
            break;
        case 3:
            SwapDataMethod();
            break;
        case 4:
            Exit();
            break;
    }
}
    
void DisplayOptionsMenu()
{
    var title = "Options menu";
    string[] options = { "Create Options", "Display Saved Options",
        "Load Options", "Delete Options",
        "Back", "Exit"
    };
    var optionsMenu = new Menu(title, options);
    var selectedIndex = optionsMenu.Run();

    switch (selectedIndex)
    {
        case 0:
            CreateOption();
            break;
        case 1:
            ListOptions();
            break;
        case 2:
            LoadGameOptions();
            break;
        case 3:
            DeleteOptions();
            break;
        case 4:
            RunMainMenu();
            break;
        case 5:
            Exit();
            break;
    }
}

void DeleteOptions()
{
    var l = new List<string>();
    var l2 = new List<string>();
    var options = optionsRepo.GetGameOptionsList();
    foreach (var optionName in options)
    {
        var opt = optionsRepo.GetGameOptions(optionName);
        if (!opt.Name.Equals(angloAmericanCheckersVersion.Name) || !opt.Name.Equals(continentalVersion.Name))
        {
            l.Add(opt.Name + "\n" + opt);
            l2.Add(opt.Name);
        }
    }
    var res = l.ToArray();
    var menu = new Menu(
        $"Please choose settings you want to delete\n" +
        $"If you delete current settings -> {currentGameOptions}, the Anglo-American version (8x8) will automatically become current",
        res);
    
    var selectedIndex = menu.Run();
    var optionsToDelete = l2[selectedIndex];
    
    //if user deletes current settings, then anglo-american version will be placed automatically;
    if (optionsToDelete.Equals(currentGameOptions.Name))
    {
        optionsRepo.DeleteGameOptions(l2[selectedIndex]);
        currentGameOptions = angloAmericanCheckersVersion;
    }
    else
    {
        optionsRepo.DeleteGameOptions(optionsToDelete);
    }

    var message = $"Done!\n Settings with the name -> {optionsToDelete} were deleted";
    LeaveSubSettingsMenu(message);
}

void CreateOption()
{
    Clear();
    WriteLine(Menu.CheckersTitle);
    WriteLine("Create your own options!");
    
    var name = CorrectStringInput(
        "Please write the name of the settings ",
        "Options name cannot be empty",
        forOptionsDb:true,
        forGamesDb:false
        );
    
    var boardSize = CorrectBoardSize();
    var whiteStarts = WhoStartsGame();

    var createdOptions = new CheckersOption
    {
        Name = name,
        Width = boardSize,
        Height = boardSize,
        RandomMoves = 0,
        WhiteStarts = whiteStarts == "y"
    };
    
    optionsRepo.SaveGameOptions(name, createdOptions);
    
    string[] options = { "Back", "Back to Main Menu" };
    var menu = new Menu($"Done!\n{name} has been added", options);
    menu.ClearCheckersTitle();
    var selectedIndex = menu.Run();
    switch (selectedIndex)
    {
        case 0:
            DisplayOptionsMenu();
            break;
        case 1:
            RunMainMenu();
            break;
    }
}

void LoadGameOptions()
{
    Clear();
    var l = new List<string>();
    var options = optionsRepo.GetGameOptionsList();
    foreach (var optionName in options)
    {
        var opt = optionsRepo.GetGameOptions(optionName);
        l.Add(opt.Name + "\n" + opt);
    }

    var res = l.ToArray();
    var menu = new Menu("Load option", res);
    menu.ClearCheckersTitle();
    var selectedIndex = menu.Run(WriteCurrentOptions);

    var newCurrentOptions = optionsRepo.GetGameOptions(options[selectedIndex]);

    currentGameOptions = newCurrentOptions;
    
    Clear();
    LeaveSubSettingsMenu("", WriteCurrentOptions);
}

void ListOptions()
{
    WriteLine("These are all available options.");
    LeaveSubSettingsMenu("", WriteAvailableOptions);
}

void Exit()
{
    Environment.Exit(0);
}

void CreateNewGame()
{
    Clear();
    WriteLine(Menu.CheckersTitle);
    WriteLine("Create your new game!");
    
    var gameName = CorrectStringInput(
        "Please enter the name of your game",
        "Game name cannot be empty",
        forOptionsDb:false,
        forGamesDb:true
        );
    
    var game = new CheckersBrain(currentGameOptions);

    var checkersGame = new CheckersGame
    {
        Name = gameName,
        Player1Name = "Jegor",
        Player2Name = "Igor",
        CheckersOption = currentGameOptions,
    };
    
    string[] options = { "Make move", "Save the Game" };
    var menu = new Menu("", options);
    menu.ClearCheckersTitle();
    var selectedIndex = menu.Run(DrawBoard);
    
    switch (selectedIndex)
    {
        case 0:
            WriteLine("move");
            break;
        case 1:
            gamesRepo.SaveGame(gameName, checkersGame);
            LeaveSubSettingsMenu($"Done!\nGame {gameName} was successfully saved");
            break;
    }
    void DrawBoard() => Ui.DrawGameBoard(game.GetBoard());
}

void LoadGame()
{
    Clear();
    var l = new List<string>();
    var games = gamesRepo.GetGamesList();
    
    foreach (var gameName in games)
    {
        l.Add(gameName + "\n");
    }

    var res = l.ToArray();
    var menu = new Menu("Load Game", res);
    menu.ClearCheckersTitle();
    var selectedIndex = menu.Run();

    var checkersGame = gamesRepo.GetGame(games[selectedIndex]);
    
    LeaveSubGameMenu($"Done\nGame with name {checkersGame.Name} is loaded!");
    
}

void WriteCurrentOptions()
{
    ForegroundColor = ConsoleColor.Blue;
    WriteLine($"Current settings are with the name -> {currentGameOptions.Name}");
    ResetColor();
}

void WriteAvailableOptions()
{
    WriteCurrentOptions();
    WriteLine();
    foreach (var fileName in optionsRepo.GetGameOptionsList())
    {
        var option = optionsRepo.GetGameOptions(fileName);
        WriteLine(option.Name);
        ForegroundColor = ConsoleColor.Blue;
        WriteLine(option.ToString());
        WriteLine();
        ResetColor();
    }
}

void LeaveSubSettingsMenu(string title, Action? action=null)
{
    string[] quitOptions = { "Back", "Go to Main Menu", "Exit" };
    var quitMenu = new Menu(title, quitOptions);
    quitMenu.ClearCheckersTitle();
    var index = quitMenu.Run(action);
    switch (index)
    {
        case 0:
            DisplayOptionsMenu();
            break;
        case 1:
            RunMainMenu();
            break;
        case 2:
            Exit();
            break;
    }
}

void LeaveSubGameMenu(string title, Action? action=null)
{
    string[] quitOptions = {"Go to Main Menu", "Exit"};
    var quitMenu = new Menu(title, quitOptions);
    quitMenu.ClearCheckersTitle();
    var index = quitMenu.Run(action);
    switch (index)
    {
        case 0:
            RunMainMenu();
            break;
        case 1:
            Exit();
            break;
    }
}

void SwapDataMethod()
{
    string[] options = { "Use File System", "Use SQLite" };
    var menu = new Menu($"Choose data accessing method\nCurrent method is {databaseEngine}\n", options);
    menu.ClearCheckersTitle();
    var selectedIndex = menu.Run();
    switch (selectedIndex)
    {
        case 0:
            databaseEngine = "File System";
            optionsRepo = optionsRepoFs;
            gamesRepo = gamesRepoFs;
            break;
        case 1:
            databaseEngine = "SQlite";
            optionsRepo = optionsRepoDb;
            gamesRepo = gamesRepoDb;
            break;
    }
    LeaveSubSettingsMenu($"Done!\nThe application uses {databaseEngine}");
}

int CorrectBoardSize()
{
    var boardSize = 0;
    var boardSizeIsCorrect = false;
    do 
    {
        WriteLine("Type your board size!\n" +
                  "(number should be even)\n" +
                  "(board size cannot be less than 8)");
            
        var size = ReadLine();
        if (size == null) continue;
            
        try
        {
            boardSize = int.Parse(size);
        }
        catch (Exception)
        { /*ignore*/ }

        if (boardSize % 2 != 0)
        {
            ForegroundColor = ConsoleColor.Red;
            WriteLine("Please enter even number");
            ResetColor();
        }
        else if (boardSize < 8)
        {
            ForegroundColor = ConsoleColor.Red;
            WriteLine("Board size cannot be less than 8");
            ResetColor();
        }
        else
        {
            boardSizeIsCorrect = true;
        }
    } while (!boardSizeIsCorrect);

    return boardSize;
}

string CorrectStringInput(string message, string errorMessage, bool forOptionsDb=false, bool forGamesDb=false)
{
    string? name = null;
    do
    {
        WriteLine(message);
        WriteLine();
        var input = ReadLine();
        
        if (input == "")
        {
            ForegroundColor = ConsoleColor.Red;
            WriteLine(errorMessage);
            ResetColor();
        }

        if (forOptionsDb)
        {
            if (input != null && !SettingsAlreadyInDb(input))
            {
                name = input;
            }
            else
            {
                ForegroundColor = ConsoleColor.Red;
                WriteLine($"Options with the name -> {input} already exists");
                ResetColor();
            }
        }
        else if (forGamesDb)
        {
            if (input != null && !GameAlreadyInDb(input))
            {
                name = input;
            }
            else
            {
                ForegroundColor = ConsoleColor.Red;
                WriteLine($"Game with the name -> {input} already exists");
                ResetColor();
            }
        }
    } while (name == null);

    return name;
}

string WhoStartsGame()
{
    var whiteStarts = "";
    do
    {
        WriteLine("Do you want white checkers make first move (y/n)");
        WriteLine();
        var choice = ReadLine();
        if (choice == "")
        {
            ForegroundColor = ConsoleColor.Red;
            WriteLine("Please type y or n");
            ResetColor();
        }
        else if (choice != null && (choice.ToLower().Equals("y") || choice.ToLower().Equals("n")))
        {
            whiteStarts = choice;
        }
        else
        {
            ForegroundColor = ConsoleColor.Red;
            WriteLine("Please type y or n");
            ResetColor();
        }
    } while (whiteStarts == "");

    return whiteStarts;
}

bool SettingsAlreadyInDb(string optionsName)
{
    return optionsRepo.GetGameOptionsList().Any(options => options.Equals(optionsName));
}

bool GameAlreadyInDb(string gameName)
{
    return gamesRepo.GetGamesList().Any(game => game.Equals(gameName));
}

EBoardPiece?[][] DeserializeBoard(string board)
{
    return System.Text.Json.JsonSerializer.Deserialize<EBoardPiece?[][]>(board)!;
}

