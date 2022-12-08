using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DAL.DB;
using ProjectDomain;

namespace WebApp.Pages_CheckersGames
{
    public class EditModel : PageModel
    {
        private readonly DAL.DB.AppDbContext _context;

        public EditModel(DAL.DB.AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public CheckersGame CheckersGame { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.CheckersGames == null)
            {
                return NotFound();
            }

            var checkersgame =  await _context.CheckersGames.FirstOrDefaultAsync(m => m.Id == id);
            if (checkersgame == null)
            {
                return NotFound();
            }
            CheckersGame = checkersgame;
           ViewData["CheckersOptionId"] = new SelectList(_context.CheckersOptions, "Id", "Name");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(CheckersGame).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CheckersGameExists(CheckersGame.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool CheckersGameExists(int id)
        {
          return (_context.CheckersGames?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}