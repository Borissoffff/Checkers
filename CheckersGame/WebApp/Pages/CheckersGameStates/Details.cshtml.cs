using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DAL.DB;
using ProjectDomain;

namespace WebApp.Pages_CheckersGameStates
{
    public class DetailsModel : PageModel
    {
        private readonly DAL.DB.AppDbContext _context;

        public DetailsModel(DAL.DB.AppDbContext context)
        {
            _context = context;
        }

      public CheckersGameState CheckersGameState { get; set; } = default!; 

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.CheckersGameStates == null)
            {
                return NotFound();
            }

            var checkersgamestate = await _context.CheckersGameStates.FirstOrDefaultAsync(m => m.Id == id);
            if (checkersgamestate == null)
            {
                return NotFound();
            }
            else 
            {
                CheckersGameState = checkersgamestate;
            }
            return Page();
        }
    }
}
