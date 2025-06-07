using System.ComponentModel.DataAnnotations;

namespace LeaguePlaza.Core.Features.Mount.Models.Dtos.Create
{
    public class RateMountDto
    {
        [Required]
        public int MountId { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }
    }
}
