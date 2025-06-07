using System.ComponentModel.DataAnnotations;

namespace LeaguePlaza.Core.Features.Mount.Models.RequestData
{
    public class RentMountRequestData
    {
        [Required]
        public int MountId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }
    }
}
