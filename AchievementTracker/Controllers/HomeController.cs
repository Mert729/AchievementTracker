using System.Diagnostics;
using AchievementTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AchievementTracker.Controllers
{
    public class HomeController : Controller
    {
        //Veritabaný baðlantýsýný bu Controller içinde güvenle kullanabilmek için oluþturulan gizli köprü.
        private readonly AchievementTrackerDB _context;
        private readonly ILogger<HomeController> _logger;

        //Dependency Injection: Uygulama her açýldýðýnda hazýr bir veritabaný baðlantýsýný içeri davet ediyoruz.
        public HomeController(ILogger<HomeController> logger, AchievementTrackerDB context)
        {
            _logger = logger;
            _context = context;
        }

        //Ana Sayfa: Oyunlarý görüntüleme
        public IActionResult Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;

            // SQL'deki Games tablosuna gidip tüm oyunlarý bir C# listesi olarak çekiyoruz.
            var games = _context.Games.Include(g => g.Achievements).OrderBy(g => g.Name).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                // Oyun isminde, aradýðýmýz kelime geçenleri filtrele (Contains = Ýçerir)
                games = games.Where(g => g.Name.Contains(searchString));
            }

            // Çektiðimiz bu oyun listesini, ekranda gösterilmek üzere View'a paketleyip yolluyoruz.
            return View(games.ToList());
        }

        //Baþarým Sayfasý: Oyunlara baðlý baþarýmlarý ve baþarýmlara baðlý kupa seviyelerini görüntüleme
        public IActionResult Achievements(int Id, string sortOrder)
        {
            //Oyunu seçip içeriðini geri döndürüyoruz
            var selectedGame = _context.Games.Include(g => g.Achievements).ThenInclude(a => a.TrophyTier)
                                             .Include(g => g.Achievements).ThenInclude(a => a.AchievementTags)
                                             .ThenInclude(at => at.Tag).FirstOrDefault(g => g.Id == Id);
            if (selectedGame == null) return NotFound();

            selectedGame.Achievements = sortOrder switch
            {
                "alfabetik" => selectedGame.Achievements.OrderBy(a => a.Name).ToList(),
                "zorluk" => selectedGame.Achievements.OrderBy(a => a.TrophyTierId).ToList(),
                _ => selectedGame.Achievements.OrderBy(a => a.TrophyTierId).ToList()
            };

            ViewData["CurrentSort"] = sortOrder;

            return View(selectedGame);
        }

        //Baþarýmlarýn açýlýþýný veritabanýna kaydetme
        [HttpPost]
        public IActionResult ToggleAchievement(int id, bool isCompleted)
        {
            // Veritabanýndan (Context) ilgili baþarýmý bulurken, BAÐLI OYUNU VE OYUNUN TÜM BAÞARIMLARINI da Include ediyoruz (Verimlilik)
            var achievement = _context.Achievements
                .Include(a => a.Game)
                    .ThenInclude(g => g.Achievements)
                .FirstOrDefault(a => a.Id == id);

            if (achievement == null)
            {
                return Json(new { success = false, message = "Baþarým bulunamadý." });
            }

            // Durumu tersine çeviriyoruz (Tike basýlmýþsa true, kaldýrýlmýþsa false yapar)
            achievement.IsCompleted = !achievement.IsCompleted;
            // --- TARÝH KONTROLÜ ---
            isCompleted = achievement.IsCompleted;
            if (isCompleted)
            {
                achievement.UnlockDate = DateTime.Now; // Tik atýldýysa o anýn tarihini ve saatini çak
            }
            else
            {
                achievement.UnlockDate = null; // Tik kaldýrýldýysa tarihi veritabanýndan sil
            }

            // === YENÝ EKLENEN KISIM: OYUNUN TAMAMLANMA DURUMUNU HESAPLA VE GÜNCELLE ===
            ReCalculateGameCompletion(achievement.Game);
            // ======================================================================

            _context.SaveChanges();

            // Baþarýlý olduðuna dair UI (Ön yüz) tarafýna JSON formatýnda cevap dönüyoruz
            return Json(new
            {
                success = true,
                isCompleted = isCompleted,
                unlockDate = isCompleted ? DateTime.Now.ToString("dd.MM.yyyy") : null
            });
        }

        //Master Checkbox
        [HttpPost]
        public IActionResult ToggleAllAchievements(int gameId, bool IsCompleted)
        {
            try
            {
                // 1. Oyunu ve o oyuna ait tüm baþarýmlarý veritabanýndan çekiyoruz (Mevcut, Include Path var)
                var game = _context.Games
                    .Include(g => g.Achievements)
                    .FirstOrDefault(g => g.Id == gameId);

                if (game == null)
                    return Json(new { success = false, message = "Oyun bulunamadý." });

                // 2. Bütün baþarýmlarýn durumunu, dýþarýdan gelen (True/False) emrine göre güncelliyoruz
                foreach (var achievement in game.Achievements)
                {
                    achievement.IsCompleted = IsCompleted;
                    // --- TARÝH KONTROLÜ ---
                    if (IsCompleted)
                    {
                        achievement.UnlockDate = DateTime.Now; // Tik atýldýysa o anýn tarihini ve saatini çak
                    }
                    else
                    {
                        achievement.UnlockDate = null; // Tik kaldýrýldýysa tarihi veritabanýndan sil
                    }
                }

                // === YENÝ EKLENEN KISIM: OYUNUN TAMAMLANMA DURUMUNU HESAPLA VE GÜNCELLE ===
                ReCalculateGameCompletion(game);
                // ======================================================================

                // 3. Tek seferde tüm deðiþiklikleri SQL'e kaydediyoruz
                _context.SaveChanges();

                return Json(new
                {
                    success = true,
                    IsCompleted = IsCompleted,
                    unlockDate = IsCompleted ? DateTime.Now.ToString("dd.MM.yyyy") : null
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata oluþtu: " + ex.Message });
            }
        }
        private void ReCalculateGameCompletion(Game game)
        {
            if (game == null || game.Achievements == null) return;

            // 1. Oyunun tüm baþarýmlarýnýn 'IsCompleted' durumu 'true' mu?
            bool allCompleted = game.Achievements.Any() && game.Achievements.All(a => a.IsCompleted);

            // 2. Game tablosundaki deðeri güncelle
            game.IsCompleted = allCompleted;

            // 3. Tarihi ayarla
            if (game.IsCompleted)
            {
                // Tüm baþarýmlar bitti, en son açýlanýn tarihini oyunun bitiþ tarihi yap
                // Max(a => a.UnlockDate)nullable olduðu için sorun çýkarmaz, DateTime? döner.
                game.CompletionDate = game.Achievements.Any() ? game.Achievements.Max(a => a.UnlockDate) : null;
            }
            else
            {
                // Oyun bitmedi, tarihi temizle
                game.CompletionDate = null;
            }
            // NOT: context.SaveChanges() burada çaðýrmýyoruz, çaðýran metodun transaction'ýna býrakýyoruz.
        }

        //Rehber Sayfasý
        public IActionResult Guide(int id)
        {
            var game = _context.Games.Include(g => g.Achievements).ThenInclude(a => a.TrophyTier).FirstOrDefault(g => g.Id == id);

            if(game == null)
            {
                return NotFound();
            }
            game.Achievements = game.Achievements.OrderBy(a => a.TrophyTierId).ToList();
            return View(game);
        }
        //Tamamlanan Oyunlar Sayfasý
        public IActionResult CompletedGames(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;

            // SQL'deki Games tablosuna gidip tüm oyunlarý bir C# listesi olarak çekiyoruz.
            var completedGames = _context.Games.Include(g => g.Achievements)
            .Where(g => g.Achievements.Any() && g.Achievements.All(a => a.IsCompleted))
            .OrderBy(g => g.Name).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                // Oyun isminde, aradýðýmýz kelime geçenleri filtrele (Contains = Ýçerir)
                completedGames = completedGames.Where(g => g.Name.Contains(searchString));
            }

            // Çektiðimiz bu oyun listesini, ekranda gösterilmek üzere View'a paketleyip yolluyoruz.
            return View(completedGames);
        }

        //Oyun Favorileme
        [HttpPost]
        public IActionResult ToggleFavorite(int id)
        {
            var game = _context.Games.FirstOrDefault(g => g.Id == id);
            if (game == null)
            {
                return Json(new { success = false, message = "Oyun Bulunamadý." });
            }
            game.IsFavorite = !game.IsFavorite;
            _context.SaveChanges();

            return Json(new { success = true, isFavorite = game.IsFavorite });
        }

        //Favori Oyunlarý Gösterme
        public IActionResult HundredList(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;

            // SQL'deki Games tablosuna gidip tüm oyunlarý bir C# listesi olarak çekiyoruz.
            var games = _context.Games.Include(g => g.Achievements).Where(g => g.IsFavorite).OrderBy(g => g.Name).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                // Oyun isminde, aradýðýmýz kelime geçenleri filtrele (Contains = Ýçerir)
                games = games.Where(g => g.Name.Contains(searchString));
            }

            // Çektiðimiz bu oyun listesini, ekranda gösterilmek üzere View'a paketleyip yolluyoruz.
            return View(games.ToList());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
