namespace IsLink.Models
{
    public class Skill
    {
        public int Id { get; set; }

        public string Name { get; set; }

        // --- Relationship with Category ---

        // Foreign Key
        public int CategoryId { get; set; }

        // Navigation Property (To access the Category object directly)
        public Category Category { get; set; }
    }
}