using DAL;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjectDomain;

namespace WebApp.Pages.CheckersGames
{
    public class IndexModel : PageModel
    {
        private readonly IGamesRepository _repository;
        private readonly IMovementsLogRepository _logRepository;

        public IndexModel(IGamesRepository repository, IMovementsLogRepository logRepository)
        {
            _repository = repository;
            _logRepository = logRepository;
        }

        public IList<CheckersGame> CheckersGame { get;set; } = default!;

        public async Task OnGetAsync()
        {
            CheckersGame = _repository.GetGamesList();
        }
    }
}
