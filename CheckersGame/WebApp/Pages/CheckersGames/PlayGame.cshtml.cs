using DAL.DB;
using GameBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages_CheckersGames;

public class PlayGame : PageModel
{
    private readonly AppDbContext _context;

    public PlayGame(AppDbContext context)
    {
        _context = context;
    }

    public CheckersBrain Brain { get; set; } = default!;
    
    public async Task<IActionResult> OnGet(int? id)
    {
        var game = _context.CheckersGames
            .Include(g => g.CheckersOption)
            .Include(g => g.CheckersGameStates)
            .FirstOrDefault(g => g.Id == id);
        
        if (game == null || game.CheckersOption == null) return NotFound();

        Brain = new CheckersBrain(game.CheckersOption);

        return Page();

    }
}