using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DAL.DB;
using ProjectDomain;

namespace WebApp.Pages_CheckersGames
{
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IGamesRepository _gameRepo; 

        public CreateModel(AppDbContext context, IGamesRepository gameRepo)
        {
            _context = context;
            _gameRepo = gameRepo;
        }

        public IActionResult OnGet()
        {
            OptionsSelectList = new SelectList(_context.CheckersOptions, "Id", "Name");
            return Page();
        }

        [BindProperty]
        public CheckersGame CheckersGame { get; set; } = default!;

        public SelectList OptionsSelectList { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid)
          {
              return Page();
          }

          CheckersGame = _gameRepo.SaveGame(CheckersGame);

          return RedirectToPage("./LaunchGame", new {id = CheckersGame.Id});
        }
    }
}
