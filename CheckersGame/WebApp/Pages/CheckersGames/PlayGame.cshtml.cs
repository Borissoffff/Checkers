using DAL;
using DAL.DB;
using GameBrain;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
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

    public ICollection<CheckersGameState>? CheckersGameState { get; set; }
    
    public PossibleMoves PossibleMoves { get; set; }
    
    public ICollection<MovementLog>? Logs { get; set; }
    
    public bool CheckerCanEatAgain { get; set; }

    public async Task<IActionResult> OnGet(int? id,int? xFrom, int?yFrom, int? x, int? y, string? cmd)
    {
        var game = _gamesRepo.GetGame(id);
        
        if (game == null || game.CheckersOption == null) return NotFound();

        Game = game;

        Brain = new CheckersBrain(game.CheckersOption, game.CheckersGameStates!.LastOrDefault());

        CheckersGameState = game.CheckersGameStates;
        

        if (x != null && y != null)
        {
            if (cmd != null)
            {
                //var checkerToMove = new Coordinate(xFrom!.Value, yFrom!.Value);
                Brain.MakeMove(x.Value, y.Value, xFrom!.Value, yFrom!.Value);
                game.CheckersGameStates!.Add(new CheckersGameState()
                {
                    SerializedGameState = Brain.GetSerializedGameState()
                });
                if (game.MovementLogs == null)
                {
                    Game.MovementLogs = new List<MovementLog>();
                }
                Game.MovementLogs!.Add(new MovementLog()
                {
                    /*MovementFrom = new DbCoordinate()
                    {
                        X = xFrom.Value,
                        Y = yFrom.Value
                    },
                    MovementTo = new DbCoordinate()
                    {
                        X = x.Value,
                        Y = y.Value
                    },*/
                    MovementFromX = xFrom.Value,
                    MovementFromY = yFrom.Value,
                    MovementToX = x.Value,
                    MovementToY = y.Value,
                    WhoMoved = GetWhoMovedLastForLog()
                });
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
        }
        Logs = _logsRepo.GetLogsByGameId(id!.Value);
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
        if (Brain.CheckerCanEatAgain) return !Brain.NextMoveByBlack() ? "White" : "Black";
        return Brain.NextMoveByBlack() ? "White" : "Black";
    }
}