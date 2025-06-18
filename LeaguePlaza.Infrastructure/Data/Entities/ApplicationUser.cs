using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeaguePlaza.Infrastructure.Data.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [ForeignKey(nameof(Cart))]
        public int CartId { get; set; }

        public CartEntity? Cart { get; set; }

        public ICollection<QuestEntity> PostedQuests { get; set; } = new HashSet<QuestEntity>();

        public ICollection<QuestEntity> AcceptedQuests { get; set; } = new HashSet<QuestEntity>();

        public ICollection<MountRentalEntity> MountRentals { get; set; } = new HashSet<MountRentalEntity>();

        public ICollection<MountRatingEntity> MountRatings { get; set; } = new HashSet<MountRatingEntity>();

        public ICollection<OrderEntity> Orders { get; set; } = new HashSet<OrderEntity>();
    }
}
