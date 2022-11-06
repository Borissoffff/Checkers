using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DAL.DB;
using ProjectDomain;

namespace WebApp.Pages_CheckersGames
{
    public class DetailsModel : PageModel
    {
        private readonly DAL.DB.AppDbContext _context;

        public DetailsModel(DAL.DB.AppDbContext context)
        {
            _context = context;
        }

      public CheckersGame CheckersGame { get; set; } = default!; 

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.CheckersGames == null)
            {
                return NotFound();
            }

            var checkersgame = await _context.CheckersGames.FirstOrDefaultAsync(m => m.Id == id);
            if (checkersgame == null)
            {
                return NotFound();
            }
            else 
            {
                CheckersGame = checkersgame;
            }
            return Page();
        }
    }
}
