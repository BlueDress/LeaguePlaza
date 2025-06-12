namespace LeaguePlaza.Core.Features.Mount.Models.Dtos.ReadOnly
{
    public class MountRentalDto
    {
        public int Id { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int MountId { get; set; }

        public string MountName { get; set; } = null!;
    }
}
