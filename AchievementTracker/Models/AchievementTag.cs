using System.ComponentModel.DataAnnotations;

namespace AchievementTracker.Models
{
    public class AchievementTag
    {
        public int AchievementId { get; set; }
        public Achievement Achievement { get; set; }

        public int TagId;
        public Tag Tag {  get; set; }

    }
}
