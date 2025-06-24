namespace LeaguePlaza.Core.Features.Order.Models.Dtos.ReadOnly
{
    public class OrderDto
    {
        public int Id { get; set; }

        public string DateCreated { get; set; } = null!;

        public string? DateCompleted { get; set; }

        public string Status { get; set; } = null!;
    }
}
