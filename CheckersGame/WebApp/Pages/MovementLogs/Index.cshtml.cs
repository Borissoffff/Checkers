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
    public class IndexModel : PageModel
    {
        private readonly DAL.DB.AppDbContext _context;

        public IndexModel(DAL.DB.AppDbContext context)
        {
            _context = context;
        }

        public IList<MovementLog> MovementLog { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.MovementLogs != null)
            {
                MovementLog = await _context.MovementLogs
                .Include(m => m.CheckersGame).ToListAsync();
            }
        }
    }
}
