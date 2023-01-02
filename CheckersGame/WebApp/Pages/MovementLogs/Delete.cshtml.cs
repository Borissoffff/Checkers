using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DAL.DB;
using ProjectDomain;

namespace WebApp.Pages_MovementLogs
{
    public class DeleteModel : PageModel
    {
        private readonly DAL.DB.AppDbContext _context;

        public DeleteModel(DAL.DB.AppDbContext context)
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

            var movementlog = await _context.MovementLogs.FirstOrDefaultAsync(m => m.Id == id);

            if (movementlog == null)
            {
                return NotFound();
            }
            else 
            {
                MovementLog = movementlog;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.MovementLogs == null)
            {
                return NotFound();
            }
            var movementlog = await _context.MovementLogs.FindAsync(id);

            if (movementlog != null)
            {
                MovementLog = movementlog;
                _context.MovementLogs.Remove(MovementLog);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
