using System.ComponentModel.DataAnnotations;

namespace FreelancerPlatform.Entities
{
    public class Tag
    {
        public int Id { get; set; }

        public int GigId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;
    }
}