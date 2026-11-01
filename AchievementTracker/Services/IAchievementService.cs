using AchievementTracker.Models;
using System.Collections.Generic;

namespace AchievementTracker.Services
{
    public interface IAchievementService
    {
        // Controller'a Tuple (çoklu değer) döneceğiz ki Json'ı orada rahatça bassın
        (bool Success, string Message, bool IsCompleted, string UnlockDate) ToggleAchievement(int id);
        (bool Success, string Message, bool IsCompleted, string UnlockDate) ToggleAllAchievements(int gameId, bool isCompleted);
        (Game Game, Dictionary<string, List<Achievement>> GroupedAchievements) GetAchievementsPageData(int gameId, string sortOrder);
        Game GetGuidePageData(int gameId);
    }
}
