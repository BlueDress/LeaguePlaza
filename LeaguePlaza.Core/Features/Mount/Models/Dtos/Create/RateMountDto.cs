using System.ComponentModel.DataAnnotations;

using static LeaguePlaza.Common.Constants.MountConstants;

namespace LeaguePlaza.Core.Features.Mount.Models.Dtos.Create
{
    public class RateMountDto
    {
        [Required]
        public int MountId { get; set; }

        [Required]
        [Range(MountMinRating, MountMaxRating)]
        public int Rating { get; set; }
    }
}
