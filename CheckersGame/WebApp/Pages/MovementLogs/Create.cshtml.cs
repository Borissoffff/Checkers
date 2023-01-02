using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DAL.DB;
using ProjectDomain;

namespace WebApp.Pages_MovementLogs
{
    public class CreateModel : PageModel
    {
        private readonly DAL.DB.AppDbContext _context;

        public CreateModel(DAL.DB.AppDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["CheckersGameId"] = new SelectList(_context.CheckersGames, "Id", "Name");
            return Page();
        }

        [BindProperty]
        public MovementLog MovementLog { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || _context.MovementLogs == null || MovementLog == null)
            {
                return Page();
            }

            _context.MovementLogs.Add(MovementLog);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
