using Microsoft.AspNetCore.Identity;

namespace LeaguePlaza.Infrastructure.Data.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<QuestEntity> PostedQuests { get; set; } = new HashSet<QuestEntity>();

        public ICollection<QuestEntity> AcceptedQuests { get; set; } = new HashSet<QuestEntity>();

        public ICollection<MountRentalEntity> MountRentals { get; set; } = new HashSet<MountRentalEntity>();

        public ICollection<MountRatingEntity> MountRatings { get; set; } = new HashSet<MountRatingEntity>();
    }
}
