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

namespace WebApp.Pages_MovementLogs
{
    public class EditModel : PageModel
    {
        private readonly DAL.DB.AppDbContext _context;

        public EditModel(DAL.DB.AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public MovementLog MovementLog { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.MovementLogs == null)
            {
                return NotFound();
            }

            var movementlog =  await _context.MovementLogs.FirstOrDefaultAsync(m => m.Id == id);
            if (movementlog == null)
            {
                return NotFound();
            }
            MovementLog = movementlog;
           ViewData["CheckersGameId"] = new SelectList(_context.CheckersGames, "Id", "Name");
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

            _context.Attach(MovementLog).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MovementLogExists(MovementLog.Id))
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

        private bool MovementLogExists(int id)
        {
          return (_context.MovementLogs?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
