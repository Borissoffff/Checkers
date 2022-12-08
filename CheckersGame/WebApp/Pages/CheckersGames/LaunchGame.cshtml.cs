using DAL;
using DAL.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjectDomain;

namespace WebApp.Pages.CheckersGames;

public class LaunchGame : PageModel
{
    private readonly IGamesRepository _gamesRepo;

    public LaunchGame(AppDbContext context, IGamesRepository gamesRepo)
    {
        _gamesRepo = gamesRepo;
    }
    
    public IActionResult OnGet(int? id)
    {
        if (id == null) return RedirectToPage("/Index", new {error = "No such id"});

        var game = _gamesRepo.GetGame(id.Value);

        if (game == null) return RedirectToPage("/Index", new { error = "No such game" });

        if (game.Player1Type == EPlayerType.Human && game.Player2Type == EPlayerType.Human)
        {
            return Page();
        }
        return RedirectToPage("PlayGame", new { id = game.Id, playerNo = '1'});
    }
}