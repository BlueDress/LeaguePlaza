namespace LeaguePlaza.Core.Features.Mount.Models.Dtos.ReadOnly
{
    public class MountDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public decimal RentPrice { get; set; }

        public string? ImageUrl { get; set; }

        public string Type { get; set; } = null!;

        public double Rating { get; set; }
    }
}
