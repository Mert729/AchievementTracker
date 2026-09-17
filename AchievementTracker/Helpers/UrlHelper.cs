using System.Text.RegularExpressions;

namespace AchievementTracker.Helpers
{
    public class UrlHelper
    {
        public static string GenerateUrlName(string text)
        {
            if (string.IsNullOrEmpty(text)) return "";

            // Türkçe karakterleri çevir
            text = text.Replace("ı", "i").Replace("ğ", "g").Replace("ü", "u")
                       .Replace("ş", "s").Replace("ö", "o").Replace("ç", "c")
                       .Replace("İ", "i").Replace("Ğ", "g").Replace("Ü", "u")
                       .Replace("Ş", "s").Replace("Ö", "o").Replace("Ç", "c");

            text = text.ToLower(); // Hepsini küçük harf yap
            text = Regex.Replace(text, @"[^a-z0-9\s-]", ""); // Özel karakterleri (noktalama işaretleri vb.) sil
            text = Regex.Replace(text, @"\s+", " ").Trim(); // Birden fazla boşluğu tek boşluğa indir
            text = Regex.Replace(text, @"\s", "-"); // Boşlukları tire (-) yap

            return text;
        }
    }
}
