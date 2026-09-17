using AchievementTracker.Helpers;
using AchievementTracker.Models;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AchievementTracker.Controllers
{
    public class AdminController : Controller
    {

        private readonly AchievementTrackerDB _db;

        public AdminController(AchievementTrackerDB db)
        {
            _db = db;
        }
        // GET: Admin/AddGame (Formu Ekrana Getirir)
        [HttpGet]
        public ActionResult AddGame()
        {
            return View();
        }

        // POST: Admin/AddGame (Butona basıldığında çalışır)
        [HttpPost]
        public ActionResult AddGame(Game newGame)
        {
            // MVC'nin doğrulama (validation) mekanizmasından UrlName'i(required olduğu için) çıkarıyoruz
            ModelState.Remove("UrlName");

            if (ModelState.IsValid)
            {
                //İsimden UrlName üretiyoruz.
                newGame.UrlName = UrlHelper.GenerateUrlName(newGame.Name);

                _db.Games.Add(newGame);
                _db.SaveChanges();

                // Kayıt bitince, listeye veya forma geri yolla 
                return RedirectToAction("Index", "Home");
            }
            return View(newGame);
        }

        // GET: Admin/Edit/5 (Düzenlenecek oyunu bulup forma getirir)
        [HttpGet]
        public ActionResult EditGame(int id)
        {
            // Veritabanından gelen id'ye sahip oyunu buluyoruz
            var game = _db.Games.Find(id);
            if (game == null)
            {
                return NotFound();// Oyun yoksa hata sayfası döndür
            }
            return View(game);
        }

        // POST: Admin/Edit (Formda 'Kaydet'e basılınca güncel verileri veritabanına yazar)
        [HttpPost]
        public ActionResult EditGame(Game updatedGame)
        {
            // UrlName yine biz üreteceğiz
            ModelState.Remove("UrlName");

            if (ModelState.IsValid)
            {
                var gameInDb = _db.Games.Find(updatedGame.Id);

                if (gameInDb != null)
                {
                    // Yeni gelen verileri, eski verilerin üzerine yazıyoruz
                    gameInDb.Name = updatedGame.Name;
                    gameInDb.IconFolderName = updatedGame.IconFolderName;
                    gameInDb.CoverImageUrl = updatedGame.CoverImageUrl;
                    gameInDb.Platform = updatedGame.Platform;
                    gameInDb.ReleaseDate = updatedGame.ReleaseDate;

                    // İsmi değişmiş olabilir, UrlName'i baştan üretiyoruz
                    gameInDb.UrlName = UrlHelper.GenerateUrlName(updatedGame.Name);

                    // Değişiklikleri kaydet
                    _db.SaveChanges();

                    // İşlem bitince ana sayfaya dön
                    return RedirectToAction("Index", "Home");
                }
            }
            return View(updatedGame);
        }


        [HttpGet]
        public IActionResult DeleteGame(int id)
        {
            // Silinmek istenen oyunu veritabanından bul
            var game = _db.Games.Find(id);

            if (game == null)
            {
                return NotFound(); // Oyun yoksa 404 dön
            }

            // Oyunu bulduysa, "Emin misin?" diye sormak üzere senin oluşturduğun DeleteGame.cshtml sayfasına gönder
            return View(game);
        }

        [HttpPost, ActionName("DeleteGame")] // Butona basıldığında burası tetiklenir
        public IActionResult DeleteConfirmed(int id)
        {
            var game = _db.Games.Find(id);
            if (game != null)
            {
                _db.Games.Remove(game); // Oyunu veritabanı paketinden çıkar
                _db.SaveChanges();      // Değişikliği SQL'e kaydet
            }

            // İşlem bitince ana sayfaya yönlendir
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult AddAchievement(Achievement newAchievement)
        {
            if (ModelState.IsValid)
            {
                // YENİ MANTIK: Aynı Oyunda + Aynı İsimde + Aynı Açıklamada başarım var mı?
                bool isDuplicate = _db.Achievements.Any(a =>
                    a.GameId == newAchievement.GameId &&
                    a.Name.ToLower() == newAchievement.Name.ToLower() &&
                    a.Description.ToLower() == newAchievement.Description.ToLower());

                if (isDuplicate)
                {
                    ModelState.AddModelError("Name", "Hata: Bu isim ve açıklama kombinasyonuna sahip bir başarım zaten eklenmiş!");
                    return View(newAchievement);
                }

                // Eğer kopya değilse kaydet
                _db.Achievements.Add(newAchievement);
                _db.SaveChanges();

                // İşlem bitince oyunun kendi başarım listesine geri dön
                return RedirectToAction("Achievements", "Home", new { id = newAchievement.GameId });
            }

            return View(newAchievement);
        }

        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) } // Seçimi 1 yıl boyunca hatırla
            );

            // returnUrl boş gelirse anasayfaya yönlendir, doluysa kullanıcının tıkladığı sayfaya geri at
            return LocalRedirect(string.IsNullOrEmpty(returnUrl) ? "~/" : returnUrl);
        }
    }
}
