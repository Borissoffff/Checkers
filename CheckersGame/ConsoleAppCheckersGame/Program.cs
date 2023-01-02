using System.Security;
using ConsoleUI;
using DAL;
using DAL.DB;
using DAL.FileSystem;
using GameBrain;
using MenuSystem;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProjectDomain;
using static System.Console;

//dotnet ef database update --project DAL.DB --startup-project ConsoleAppCheckersGame

string databaseEngine = "SQlite";

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
CheckersOption angloAmericanCheckersVersion = new CheckersOption
{
    Name = "The Anglo-American version",
    /*Height = 8,
    Width = 12*/
};

CheckersOption continentalVersion = new CheckersOption
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
CheckersOption currentGameOptions = angloAmericanCheckersVersion;

IGameOptionsRepository optionsRepo = optionsRepoDb;
IGamesRepository gamesRepo = gamesRepoDb;

RunMainMenu();
WriteLine();
WriteLine("Press any key to exit ...");
ReadKey(true);

void RunMainMenu()
{
    string title = "Main Menu";
    string[] options = { "New Game", "Load Game", "Delete Game", "Options", "Data Management Method Swap", "Exit" };
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
            DeleteGame();
            break;
        case 3:
            DisplayOptionsMenu();
            break;
        case 4:
            SwapDataMethod();
            break;
        case 5:
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
    else optionsRepo.DeleteGameOptions(optionsToDelete);
    
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
        forOptionsDb:false,
        forGamesDb:true);
    
    var lastGameId = gamesRepo.GetLastGameId();

    var checkersGame = new CheckersGame
    {
        Name = gameName,
        Player1Name = "Jegor",
        Player2Name = "Igor",
        CheckersOption = currentGameOptions,
    };

    if (databaseEngine.Equals("File System"))
    {
        checkersGame.Id = gamesRepo.GetLastGameId() + 1;
    }

    gamesRepo.SaveGame(checkersGame, (lastGameId + 1).ToString());


    var gameBrain = new CheckersBrain(currentGameOptions, null);

    string[] options = { "Make move", "Save the Game" };
    var menu = new Menu("", options);
    menu.ClearCheckersTitle();
    var selectedIndex = menu.Run(DrawBoard);
    
    switch (selectedIndex)
    {
        case 0:
            WriteLine("move");
            GamePlay(game: checkersGame, brain: gameBrain);
            break;
        case 1:
            LeaveSubSettingsMenu($"Done!\nGame {gameName} was successfully saved");
            break;
    }
    
    void DrawBoard() => Ui.DrawGameBoard(gameBrain.GetBoard());
}

void LoadGame()
{
    Clear();
    var games = gamesRepo.GetGamesList();

    var res = games.Select(game => game.Name + "\n").ToArray();
    
    var menu = new Menu("Load Game", res);
    menu.ClearCheckersTitle();
    var selectedIndex = menu.Run();

    var chosenGame = games[selectedIndex];

    var game = gamesRepo.GetGame(chosenGame.Id);

    var lastState = game!.CheckersGameStates!.Last();

    var brain = new CheckersBrain(game.CheckersOption!, lastState);
    
    GamePlay(game, brain);
    
    //RunLogicForMove(game, brain);
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

void DeleteGame()
{
    Clear();
    var games = gamesRepo.GetGamesList();
    
    var res = games.Select(game => game.Name + "\n").ToArray();
    
    var menu = new Menu("Load Game", res);
    menu.ClearCheckersTitle();
    var selectedIndex = menu.Run();

    var chosenGame = games[selectedIndex];
    
    gamesRepo.DeleteGame(chosenGame.Id);
    
    string[] quitOptions = {"Go to Main Menu", "Exit" };
    var quitMenu = new Menu($"Game {chosenGame.Name} was successfully deleted!", quitOptions);
    quitMenu.ClearCheckersTitle();
    var index = quitMenu.Run();
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
        catch (Exception) { WriteErrorMessage("Input must be numeric"); }

        if (boardSize % 2 != 0) WriteErrorMessage("Please enter even number");
      
        else if (boardSize < 8) WriteErrorMessage("Board size cannot be less than 8");

        else boardSizeIsCorrect = true;
        
    } while (!boardSizeIsCorrect);

    return boardSize;
}

string CorrectStringInput(string message, bool forOptionsDb=false, bool forGamesDb=false)
{
    string? name = null;
    do
    {
        WriteLine(message);
        WriteLine();
        var input = ReadLine();
        
        if (input == "") WriteErrorMessage("Input cannot be empty");

        if (forOptionsDb)
        {
            if (input != null && !SettingsAlreadyInDb(input)) name = input;
            
            else WriteErrorMessage($"Options with the name -> {input} already exists");
           
        }
        else if (forGamesDb)
        {
            if (input != null && !GameAlreadyInDb(input)) name = input;
            
            else WriteErrorMessage($"Game with the name -> {input} already exists");
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
        if (choice == "") WriteErrorMessage("Please type y or n");
      
        else if (choice != null &&
                 (choice.ToLower().Equals("y") ||
                  choice.ToLower().Equals("n"))
                 ) whiteStarts = choice;
        
        else WriteErrorMessage("Please type y or n");
        
    } while (whiteStarts == "");

    return whiteStarts;
}

bool SettingsAlreadyInDb(string optionsName)
{
    return optionsRepo.GetGameOptionsList().Any(options => options.Equals(optionsName));
}

bool GameAlreadyInDb(string gameName)
{
    try
    {
        return gamesRepo.GetGamesList().Any(game => game.Equals(gameName));
    }
    catch (Exception) { return false; }
}

void GamePlay(CheckersGame game, CheckersBrain brain)
{
    string[] options = { "Make a move", "Go to Main Menu (Game saved automatically)", "Exit (Game saved automatically)" };
    var menu = new Menu($"{WhoMoves(brain, game)} moves\n", options);
    menu.ClearCheckersTitle();
    var selectedIndex = menu.Run(DrawBoard);

    switch (selectedIndex)
    {
        case 0:
            RunLogicForMove(game, brain);
            break;
        case 1:
           // gamesRepo.SaveGame(game, (gamesRepo.GetLastGameId() + 1).ToString());
            RunMainMenu();
            break;
        case 2:
           // gamesRepo.SaveGame(game, (gamesRepo.GetLastGameId() + 1).ToString());
            Exit();
            break;
    }
    void DrawBoard() => Ui.DrawGameBoard(brain.GetBoard());
}

void RunLogicForMove(CheckersGame game, CheckersBrain brain)
{
    while (!brain.GameOver())
    {
        Ui.DrawGameBoard(brain.GetBoard());
        
        var coordFrom = GetCheckerThatUserWantsToMove(brain, brain.CheckersUserCanPick());

        WriteLine($"you selected position {coordFrom.X} {coordFrom.Y}");

        var piece = brain.GetBoard()[coordFrom.X][coordFrom.Y];

        PossibleMoves? possibleMoves;

        if (brain.NextMoveByBlack())
        {
            possibleMoves = piece 
                is EBoardPiece.BlackSquareBlackKing 
                or EBoardPiece.BlackSquareWhiteKing 
                ? brain.FindPossibleMovesForKing(coordFrom.X, coordFrom.Y) 
                : brain.FindPossibleMovesForBlack(coordFrom.X, coordFrom.Y);
        }
        else
        {
            possibleMoves = piece 
                is EBoardPiece.BlackSquareBlackKing 
                or EBoardPiece.BlackSquareWhiteKing 
                ? brain.FindPossibleMovesForKing(coordFrom.X, coordFrom.Y) 
                : brain.FindPossibleMovesForWhite(coordFrom.X, coordFrom.Y);
        }

        var possibleMovesAsString = "";
        possibleMoves.AllPossibleMoves.ForEach(move => possibleMovesAsString += move.ToString());
    
        Ui.DrawGameBoard(brain.GetBoard(), possibleMoves);
        WriteLine();
        WriteLine($"Possible moves for {coordFrom} are {possibleMovesAsString}");

        Coordinate coordTo = GetCheckerThatUserWantsToMove(brain, possibleMoves.AllPossibleMoves);

        var log = brain.MakeMove(coordTo.X, coordTo.Y, coordFrom.X, coordFrom.Y);

        if (game.CheckersGameStates == null)
        {
            game.CheckersGameStates = new List<CheckersGameState>();
        }

        game.CheckersGameStates!.Add(new CheckersGameState()
        {
            SerializedGameState = brain.GetSerializedGameState()
        });
                
        if (game.MovementLogs == null) game.MovementLogs = new List<MovementLog>();
        
        game.MovementLogs!.Add(log);

        gamesRepo.SaveChanges();
        
        GamePlay(game, brain);
    }
}

Coordinate GetCheckerThatUserWantsToMove(CheckersBrain brain, List<Coordinate> possibleMoves)
{
    var cols = brain.GetBoard().GetLength(0);
    var rows = brain.GetBoard()[0].GetLength(0);
    
    var chars = GetBoardLetters(cols);
    var numberList = GetBoardNumbers(rows);

    var availableCheckersAsStringList = new List<string>();
    possibleMoves.ForEach(
        checker => availableCheckersAsStringList.Add($"{chars[checker.X]} {rows - checker.Y}"));
    var availableCheckersAsString = string.Join(", ", availableCheckersAsStringList);

    Coordinate? coordinate = null;
    string? letter = null;
    int number = -1;
    
    do
    {
        WriteLine("\nSelect checker you want to move.\n " +
                  "It must look like '{Letter} {Number}'." +
                  "\n For example A 3 or C 7\n" +
                  $"Available options are {availableCheckersAsString}");
        WriteLine();
        
        var input = ReadLine();
        
        if (string.IsNullOrEmpty(input)) WriteErrorMessage("Input cannot be empty");
        
        else if (input.Contains(' '))
        {
            var elements = input.Split(' ');
            
            var colLetter = elements[0];
            var rowNumberAsString = elements[1];
            
            try
            {
                var num = int.Parse(rowNumberAsString);

                if (numberList.Contains(num) && chars.Contains(colLetter))
                {
                    number = rows - num;
                    letter = colLetter;
                }
                else WriteErrorMessage("Wrong input");
            }
            catch (Exception) { WriteErrorMessage("Wrong input format"); }
        }
        else WriteErrorMessage("Wrong input format");

        if (letter.IsNullOrEmpty() || number <= -1) continue;
        
        var coord = new Coordinate(
            chars.IndexOf(letter!),
            number);
        
       // if (possibleMoves.Contains(coord)) coordinate = coord;
       if (possibleMoves.Any(c => c.X == coord.X && c.Y == coord.Y)) coordinate = coord;
       else WriteErrorMessage("You cannot pick that checker");

    } while (coordinate == null);

    return coordinate;
}

string WhoMoves(CheckersBrain brain, CheckersGame game)
    => brain.NextMoveByBlack() ? game.Player2Name : game.Player1Name;


void WriteErrorMessage(string message)
{
    ForegroundColor = ConsoleColor.Red;
    WriteLine(message);
    ResetColor(); 
}

List<string> GetBoardLetters(int cols)
{
    var charList = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToList().ToList();
    var res = charList.Select(ch => ch.ToString()).ToList();

    foreach (var ch in charList)
    {
        res.AddRange(charList.Where(ch2 => !ch.Equals(ch2)).Select(ch2 => $"{ch}{ch2}"));
    }
    return res.Take(cols).ToList();
}

List<int> GetBoardNumbers(int rows)
{
    var numberList = Enumerable.Range(1, rows).Select(c => c).ToList();
    numberList.Reverse();
    return numberList;
}