using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AchievementTracker.Models
{
    //Oyunların özelliklerinin(Property) ve metotlarının(Function) belirtildiği class
    [Table("Game")]
    public class Game
    {
        [Key]
        public int Id { get; set; }
        [StringLength(100), Required]
        public required string Name { get; set; }

        [StringLength(100), Required]
        public string UrlName { get; set; } = string.Empty;
        [StringLength(500)]
        public string? CoverImageUrl { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Platform { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } = false;
        public bool IsFavorite { get; set; } = false;
        public string? IconFolderName { get; set; }
        public bool IsCompleted { get; set; } = false;
        public DateTime? CompletionDate { get; set; }
        //public string RawgId { get; set; } = string.Empty;
        //public string ExternalDatabaseId { get; set; } = string.Empty;

        public ICollection<Achievement> Achievements { get; set; } = new List<Achievement>();
    }
}
