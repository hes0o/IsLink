using System.ComponentModel.DataAnnotations;

namespace FreelancerPlatform.Entities
{
    public class GigImage
    {
        public int Id { get; set; }

        public int GigId { get; set; }
        // We don't need a navigation property back to Gig here to avoid loops, 
        // but if needed by EF, we can add it. For now, keep it simple.

        [Required]
        public string ImageUrl { get; set; } = string.Empty;
    }
}