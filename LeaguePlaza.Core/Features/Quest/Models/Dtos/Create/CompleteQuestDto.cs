using System.ComponentModel.DataAnnotations;

namespace LeaguePlaza.Core.Features.Quest.Models.Dtos.Create
{
    public class CompleteQuestDto
    {
        [Required]
        public int Id { get; set; }
    }
}
