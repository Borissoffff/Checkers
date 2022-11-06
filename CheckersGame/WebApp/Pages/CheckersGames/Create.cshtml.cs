using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DAL.DB;
using ProjectDomain;

namespace WebApp.Pages_CheckersGames
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

          _context.CheckersGames.Add(CheckersGame);
          await _context.SaveChangesAsync();

          return RedirectToPage("./PlayGame");
        }
    }
}
