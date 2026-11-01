using AchievementTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace AchievementTracker.Services
{
    public class GameService : IGameService
    {
        private readonly AchievementTrackerDB _context;

        public GameService(AchievementTrackerDB context)
        {
            _context = context;
        }

        public List<Game> GetGames(string searchString)
        {
            var games = _context.Games.Include(g => g.Achievements).OrderBy(g => g.Name).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
                games = games.Where(g => g.Name.Contains(searchString));

            return games.ToList();
        }

        public List<Game> GetCompletedGames(string searchString)
        {
            var completedGames = _context.Games.Include(g => g.Achievements)
                .Where(g => g.Achievements.Any() && g.Achievements.All(a => a.IsCompleted))
                .OrderBy(g => g.Name).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
                completedGames = completedGames.Where(g => g.Name.Contains(searchString));

            return completedGames.ToList();
        }

        public List<Game> GetHundredList(string searchString)
        {
            var games = _context.Games.Include(g => g.Achievements)
                .Where(g => g.IsFavorite)
                .OrderBy(g => g.Name).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
                games = games.Where(g => g.Name.Contains(searchString));

            return games.ToList();
        }

        public (bool Success, string Message, bool IsFavorite) ToggleFavorite(int id)
        {
            var game = _context.Games.FirstOrDefault(g => g.Id == id);

            if (game == null)
                return (false, "Oyun Bulunamadı.", false);

            game.IsFavorite = !game.IsFavorite;
            _context.SaveChanges();

            return (true, "Başarılı", game.IsFavorite);
        }
    }
}
