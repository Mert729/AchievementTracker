using AchievementTracker.Models;
using AchievementTracker.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace AchievementTracker.Controllers
{
    public class HomeController : Controller
    {
        //Veritabaný baðlantýsýný bu Controller içinde güvenle kullanabilmek için oluþturulan gizli köprü.
        private readonly AchievementTrackerDB _context;
        private readonly ILogger<HomeController> _logger;
        private readonly IAchievementService _achievementService;
        private readonly IGameService _gameService;

        //Dependency Injection: Uygulama her açýldýðýnda hazýr bir veritabaný baðlantýsýný içeri davet ediyoruz.
        public HomeController(ILogger<HomeController> logger, AchievementTrackerDB context, 
                              IAchievementService achievementService, IGameService gameService)
        {
            _logger = logger;
            _context = context;
            _achievementService = achievementService;
            _gameService = gameService;
        }

        //Ana Sayfa: Oyunlarý görüntüleme
        public IActionResult Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;
            // Sadece servisten (mutfaktan) hazýr listeyi istiyoruz
            var games = _gameService.GetGames(searchString);
            return View(games);
        }

        //Baþarým Sayfasý: Oyunlara baðlý baþarýmlarý ve baþarýmlara baðlý kupa seviyelerini görüntüleme
        public IActionResult Achievements(int Id, string sortOrder)
        {
            var data = _achievementService.GetAchievementsPageData(Id, sortOrder);

            if (data.Game == null)
                return NotFound();

            // Sadece ön yüze (View) aktarýlacak çantalarý (ViewBag/ViewData) burada dolduruyoruz
            ViewBag.GroupedAchievements = data.GroupedAchievements;
            ViewData["CurrentSort"] = sortOrder;

            return View(data.Game);
        }

        [HttpPost]
        public IActionResult ToggleAchievement(int id)
        {
            // Bütün iþi Servis'e (Beyin takýmýna) kilitledik!
            var result = _achievementService.ToggleAchievement(id);

            if (!result.Success)
                return Json(new { success = false, message = result.Message });

            return Json(new { success = true, isCompleted = result.IsCompleted, unlockDate = result.UnlockDate });
        }

        [HttpPost]
        public IActionResult ToggleAllAchievements(int gameId, bool IsCompleted)
        {
            // Bütün iþi Servis'e kilitledik!
            var result = _achievementService.ToggleAllAchievements(gameId, IsCompleted);

            if (!result.Success)
                return Json(new { success = false, message = result.Message });

            return Json(new { success = true, isCompleted = result.IsCompleted, unlockDate = result.UnlockDate });
        }

        //Rehber Sayfasý
        public IActionResult Guide(int id)
        {
            var game = _achievementService.GetGuidePageData(id);

            if (game == null)
                return NotFound();

            return View(game);
        }

        //Tamamlanan Oyunlar Sayfasý
        public IActionResult CompletedGames(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;
            var completedGames = _gameService.GetCompletedGames(searchString);
            return View(completedGames);
        }

        //Oyun Favorileme
        [HttpPost]
        public IActionResult ToggleFavorite(int id)
        {
            var result = _gameService.ToggleFavorite(id);

            if (!result.Success)
                return Json(new { success = false, message = result.Message });

            return Json(new { success = true, isFavorite = result.IsFavorite });
        }

        //Favori Oyunlarý Gösterme
        public IActionResult HundredList(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;
            var games = _gameService.GetHundredList(searchString);
            return View(games);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
