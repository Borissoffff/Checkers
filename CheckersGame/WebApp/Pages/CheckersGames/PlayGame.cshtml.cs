using DAL;
using DAL.DB;
using GameBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using ProjectDomain;

namespace WebApp.Pages.CheckersGames;

public class PlayGame : PageModel
{
    private readonly IGamesRepository _gamesRepo;
    private readonly IMovementsLogRepository _logsRepo;

    public PlayGame(AppDbContext context, IGamesRepository gamesRepo, IMovementsLogRepository logRepo)
    {
        _gamesRepo = gamesRepo;
        _logsRepo = logRepo;
    }

    public CheckersBrain Brain { get; set; } = default!;
    public CheckersGame Game { get; set; } = default!;
    
    public int PlayerNr { get; set; }

    public ICollection<CheckersGameState>? CheckersGameState { get; set; }
    
    public PossibleMoves? PossibleMoves { get; set; }
    
    public ICollection<MovementLog>? Logs { get; set; }
    
    public bool CheckerCanEatAgain { get; set; }

    public string Msg { get; set; }

    public async Task<IActionResult> OnGet(int? id,int? xFrom, int?yFrom, int? x, int? y, string? cmd, int? playerNr, bool? aiMoves)
    {
        if (id == null)
        {
            return RedirectToPage("/Index", new { error = "No game id" });
        }
        
        var game = _gamesRepo.GetGame(id);

        if (game == null || game.CheckersOption == null)
        {
            return RedirectToPage("/Index", new { error = "No such game" });
        }

        if (playerNr == null && game.CheckersGameStates == null)
        {
            PlayerNr = 0;
        }
        else if (playerNr!.Value < 0 || playerNr.Value > 1)
        {
            return RedirectToPage("/Index", new { error = "No player Nr" });
        }
        else
        {
            PlayerNr = playerNr.Value;
        }

        Game = game;

        Brain = new CheckersBrain(game.CheckersOption, game.CheckersGameStates!.LastOrDefault());

        CheckersGameState = game.CheckersGameStates;

        if (x.HasValue && y.HasValue)
        {
            if (cmd != null)
            {
                //if we refresh the page during the opponent's move, dont do anything
                if (RefreshWillCrushLogic(xFrom!.Value, yFrom!.Value, x.Value, y.Value))
                {
                    Logs = _logsRepo.GetLogsByGameId(id.Value);
                    return Page(); 
                }

                var log = Brain.MakeMove(x.Value, y.Value, xFrom.Value, yFrom.Value);

                if (log == null) return Page();
                
                game.CheckersGameStates!.Add(new CheckersGameState()
                {
                    SerializedGameState = Brain.GetSerializedGameState()
                });
                
                if (game.MovementLogs == null)
                {
                    Game.MovementLogs = new List<MovementLog>();
                }
                Game.MovementLogs!.Add(log);

                _gamesRepo.SaveChanges();
            }
            else
            {
                var piece = Brain.GetBoard()[x.Value][y.Value];
                if (Brain.NextMoveByBlack())
                {
                    PossibleMoves = piece 
                        is EBoardPiece.BlackSquareBlackKing 
                        or EBoardPiece.BlackSquareWhiteKing 
                        ? Brain.FindPossibleMovesForKing(x.Value, y.Value) 
                        : Brain.FindPossibleMovesForBlack(x.Value, y.Value);
                }
                else
                {
                    PossibleMoves = piece 
                        is EBoardPiece.BlackSquareBlackKing 
                        or EBoardPiece.BlackSquareWhiteKing 
                        ? Brain.FindPossibleMovesForKing(x.Value, y.Value) 
                        : Brain.FindPossibleMovesForWhite(x.Value, y.Value);
                }
            }
        } else if (aiMoves.HasValue && 
                   (
                        playerNr == 0 && Game.Player2Type == EPlayerType.Ai ||
                        playerNr == 1 && Game.Player1Type == EPlayerType.Ai
                   ))
        {
            var log = Brain.MakeMoveByAi();
            
            if (log == null) return Page();
            
            if (game.MovementLogs == null)
            {
                Game.MovementLogs = new List<MovementLog>();
            }

            Game.MovementLogs!.Add(log);
            
            game.CheckersGameStates!.Add(new CheckersGameState()
            {
                SerializedGameState = Brain.GetSerializedGameState()
            });
            _gamesRepo.SaveChanges();
            Logs = _logsRepo.GetLogsByGameId(id.Value);
        }
        Logs = _logsRepo.GetLogsByGameId(id.Value);
        FindOutCanCheckerEatAgain();

        return Page();
    }

    private void FindOutCanCheckerEatAgain()
    {
        if (Logs != null)
        {
            CheckerCanEatAgain = (Brain.NextMoveByBlack() && Logs.LastOrDefault()?.WhoMoved == "Black") ||
                                 (!Brain.NextMoveByBlack() && Logs.LastOrDefault()?.WhoMoved == "White"); 
        }
        else
        {
            CheckerCanEatAgain = false;
        }
    }

    private string GetWhoMovedLastForLog()
    {
        var state = Game.CheckersGameStates?.Last();
        
        if (state == null) return Brain.NextMoveByBlack() ? "White" : "Black";
        
        var deserialized = System.Text.Json.JsonSerializer.Deserialize<CheckersState>(state.SerializedGameState);
        return deserialized!.NextMoveByBlack ? "Black" : "White";
    }

    private bool RefreshWillCrushLogic(int xFrom, int yFrom, int x, int y)
    {
        try
        {
            if (PlayerNr == 0)
            {
                var myLastMove = _logsRepo.GetAllLogs().Last(log => log.WhoMoved == "White");
                if (xFrom == myLastMove.MovementFromX && yFrom == myLastMove.MovementFromY && x == myLastMove.MovementToX && y == myLastMove.MovementToY)
                {
                    return true;
                }
            }
            else
            {
                var myLastMove = _logsRepo.GetAllLogs().Last(log => log.WhoMoved == "Black");
                if (xFrom == myLastMove.MovementFromX && yFrom == myLastMove.MovementFromY && x == myLastMove.MovementToX && y == myLastMove.MovementToY)
                {
                    return true;
                }
            }
        }
        catch (Exception e) { /*ignored*/ }

        return false;
    }
}