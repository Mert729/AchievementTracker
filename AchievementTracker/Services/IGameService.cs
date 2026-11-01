using AchievementTracker.Models;

namespace AchievementTracker.Services
{
    public interface IGameService
    {
        List<Game> GetGames(string searchString);
        List<Game> GetCompletedGames(string searchString);
        List<Game> GetHundredList(string searchString);
        (bool Success, string Message, bool IsFavorite) ToggleFavorite(int id);
    }
}
