using Microsoft.AspNetCore.Identity;

namespace LeaguePlaza.Infrastructure.Data.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<Quest> PostedQuests { get; set; } = new HashSet<Quest>();

        public ICollection<Quest> AcceptedQuests { get; set; } = new HashSet<Quest>();
    }
}
