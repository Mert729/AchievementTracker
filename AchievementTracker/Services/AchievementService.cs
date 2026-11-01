using AchievementTracker.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace AchievementTracker.Services
{
    public class AchievementService : IAchievementService
    {
        // Veritabanı bağlantımızı (Dependency Injection ile) alıyoruz
        private readonly AchievementTrackerDB _context; // Kendi DbContext adın neyse (örn: ApplicationDbContext) onunla değiştir

        public AchievementService(AchievementTrackerDB context)
        {
            _context = context;
        }

        //Başarımların açılışını veritabanına kaydetme
        public (bool Success, string Message, bool IsCompleted, string UnlockDate) ToggleAchievement(int id)
        {
            var achievement = _context.Achievements
                .Include(a => a.Game)
                    .ThenInclude(g => g.Achievements)
                .FirstOrDefault(a => a.Id == id);

            if (achievement == null)
                return (false, "Başarım bulunamadı.", false, null);

            // Durumu tersine çeviriyoruz
            achievement.IsCompleted = !achievement.IsCompleted;
            bool isCompleted = achievement.IsCompleted;

            achievement.UnlockDate = isCompleted ? DateTime.Now : null;

            ReCalculateGameCompletion(achievement.Game);
            _context.SaveChanges();

            return (true, "Başarılı", isCompleted, isCompleted ? DateTime.Now.ToString("dd.MM.yyyy") : null);
        }

        //Master CheckBox
        public (bool Success, string Message, bool IsCompleted, string UnlockDate) ToggleAllAchievements(int gameId, bool isCompleted)
        {
            try
            {
                var game = _context.Games
                    .Include(g => g.Achievements)
                    .FirstOrDefault(g => g.Id == gameId);

                if (game == null)
                    return (false, "Oyun bulunamadı.", false, null);

                foreach (var achievement in game.Achievements)
                {
                    achievement.IsCompleted = isCompleted;
                    achievement.UnlockDate = isCompleted ? DateTime.Now : null;
                }

                ReCalculateGameCompletion(game);
                _context.SaveChanges();

                return (true, "Başarılı", isCompleted, isCompleted ? DateTime.Now.ToString("dd.MM.yyyy") : null);
            }
            catch (Exception ex)
            {
                return (false, $"Hata oluştu: {ex.Message}", false, null);
            }
        }

        //İlerleme Durumu Hesaplama
        private void ReCalculateGameCompletion(Game game) // Eğer hata verirse Game sınıfının yolunu (using) yukarıya ekle
        {
            if (game == null || game.Achievements == null) return;

            bool allCompleted = game.Achievements.Any() && game.Achievements.All(a => a.IsCompleted);
            game.IsCompleted = allCompleted;

            if (game.IsCompleted)
                game.CompletionDate = game.Achievements.Any() ? game.Achievements.Max(a => a.UnlockDate) : null;
            else
                game.CompletionDate = null;
        }

        //Başarımları Çekme ve Başarım Sıralama işlemleri
        public (Game Game, Dictionary<string, List<Achievement>> GroupedAchievements) GetAchievementsPageData(int gameId, string sortOrder)
        {
            var selectedGame = _context.Games
                .Include(g => g.Achievements).ThenInclude(a => a.TrophyTier)
                .Include(g => g.Achievements).ThenInclude(a => a.AchievementTags).ThenInclude(at => at.Tag)
                .FirstOrDefault(g => g.Id == gameId);

            if (selectedGame == null)
                return (null, null);

            var sortedAchievements = sortOrder switch
            {
                "alfabetik" => selectedGame.Achievements.OrderBy(a => a.Name).ToList(),
                "tarih" => selectedGame.Achievements.OrderByDescending(a => a.IsCompleted)
                                                    .ThenByDescending(a => a.UnlockDate)
                                                    .ThenBy(a => a.TrophyTierId).ToList(),
                "zorluk" => selectedGame.Achievements.OrderBy(a => a.TrophyTierId).ThenBy(a => a.Name).ToList(),
                _ => selectedGame.Achievements.OrderBy(a => a.TrophyTierId).ThenBy(a => a.Name).ToList(),
            };

            selectedGame.Achievements = sortedAchievements;

            var groupedAchievements = sortedAchievements
                .GroupBy(a => string.IsNullOrWhiteSpace(a.DLCName) ? "Base Game" : a.DLCName)
                .OrderBy(g => g.Key == "Base Game" ? 0 : 1)
                .ThenBy(g => g.Key)
                .ToDictionary(g => g.Key, g => g.ToList());

            return (selectedGame, groupedAchievements);
        }

        //Rehber Sayfası Getirme
        public Game GetGuidePageData(int gameId)
        {
            var game = _context.Games
                .Include(g => g.Achievements).ThenInclude(a => a.TrophyTier)
                .FirstOrDefault(g => g.Id == gameId);

            if (game != null)
            {
                game.Achievements = game.Achievements.OrderBy(a => a.TrophyTierId).ToList();
            }

            return game;
        }
    }
}
