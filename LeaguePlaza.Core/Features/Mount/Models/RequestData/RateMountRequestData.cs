using System.ComponentModel.DataAnnotations;

namespace LeaguePlaza.Core.Features.Mount.Models.RequestData
{
    public class RateMountRequestData
    {
        [Required]
        public int MountId { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }
    }
}
