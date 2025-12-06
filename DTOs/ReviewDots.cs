using System.ComponentModel.DataAnnotations;

namespace FreelancerPlatform.DTOs
{
    public class CreateReviewDto
    {
        [Range(1, 5)]
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }

    public class ReviewDto
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
