namespace LeaguePlaza.Core.Features.Mount.Models.Dtos.ReadOnly
{
    public class MountRentalResultDto
    {
        public bool IsMountRentSuccessful { get; set; }

        public string MountRentMessage { get; set; } = null!;
    }
}
