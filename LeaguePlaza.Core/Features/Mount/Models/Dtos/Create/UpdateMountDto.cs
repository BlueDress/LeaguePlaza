using System.ComponentModel.DataAnnotations;

namespace LeaguePlaza.Core.Features.Mount.Models.Dtos.Create
{
    public class UpdateMountDto : CreateMountDto
    {
        [Required]
        public int Id { get; set; }
    }
}
