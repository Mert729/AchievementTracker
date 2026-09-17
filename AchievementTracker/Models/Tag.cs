using System.ComponentModel.DataAnnotations;

namespace AchievementTracker.Models
{
    public class Tag
    {
        [Key]
        public int Id { get; set; }

        [StringLength(50), Required]
        public required string Name { get; set; }

        public ICollection<AchievementTag> AchievementTags { get; set; } = new List<AchievementTag>();

    }
}
