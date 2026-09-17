using System.ComponentModel.DataAnnotations;

namespace AchievementTracker.Models
{
    public class TrophyTier
    {
        [Key]
        public int Id { get; set; }

        [StringLength(50), Required]
        public required string Name { get; set; }

        public required string IconPath {  get; set; }

        public ICollection<Achievement> Achievements { get; set; } = new List<Achievement>();
    }
}
