namespace LeaguePlaza.Core.Features.Order.Models.Dtos.ReadOnly
{
    public class AddToCartResultDto
    {
        public bool IsAddToCartSuccessful { get; set; }

        public string AddToCartMessage { get; set; } = null!;
    }
}
