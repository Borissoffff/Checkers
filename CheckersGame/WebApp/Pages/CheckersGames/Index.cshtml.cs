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
    public class IndexModel : PageModel
    {
        private readonly DAL.DB.AppDbContext _context;

        public IndexModel(DAL.DB.AppDbContext context)
        {
            _context = context;
        }

        public IList<CheckersGame> CheckersGame { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.CheckersGames != null)
            {
                CheckersGame = await _context.CheckersGames
                .Include(c => c.CheckersOption).ToListAsync();
            }
        }
    }
}
