using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AchievementTracker.Models
{
    //Başarımların özelliklerinin(Property) ve metotlarının(Function) belirtildiği class
    [Table("Achievement")]
    public class Achievement
    {
        
        [Key]
        public int Id { get; set; }
        // SQL veritabanında bu sütunun boş bırakılamayacağını ve maksimum karakter sınırını belirleyen güvenlik kuralları.
        [StringLength(100), Required]
        public required string Name { get; set; }
        [StringLength(200)]
        public string? Description { get; set; }
        public bool IsHidden { get; set; }
     
        //Başarım classını oyun classına bağlayan özellik(Property).
        public int GameId { get; set; }
        // GameId üzerinden o oyunun özelliklerine erişebilmemizi sağlayan sanal köprü (Navigation Property)
        public Game? Game { get; set; } = null!;
        
        public bool IsCompleted { get; set; } = false;
        public int TrophyTierId { get; set; }
        public TrophyTier? TrophyTier { get; set; }
        public string? VideoUrl { get; set; }
        public bool IsSpoiler { get; set; } = false;
        public int Points { get; set; } //Lider tablosu için tutulacak puan
        public DateTime? UnlockDate { get; set; }
        

        public ICollection<AchievementTag> AchievementTags { get; set; } = new List<AchievementTag>();



        public string ShowSpoiler()
        {
            return "";
        }
    }
}
