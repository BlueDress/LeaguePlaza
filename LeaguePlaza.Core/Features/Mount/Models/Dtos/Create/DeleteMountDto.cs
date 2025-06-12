using System.ComponentModel.DataAnnotations;

namespace LeaguePlaza.Core.Features.Mount.Models.Dtos.Create
{
    public class DeleteMountDto
    {
        [Required]
        public int Id { get; set; }
    }
}
