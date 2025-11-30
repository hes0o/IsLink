using System.Collections.Generic;

public class Category
{
    public int Id { get; set; }

    public string Name { get; set; }

    // --- Self-Referencing Relationship ---

    // Parent Category ID (Nullable if it is a root category)
    public int? ParentCategoryId { get; set; }

    // Parent Category object (Navigation property)
    public Category ParentCategory { get; set; }

    // Collection of Subcategories
    // Example: Programming -> Web, Mobile, Desktop
    public ICollection<Category> SubCategories { get; set; }


    // --- Relationship with Skills ---

    // Collection of Skills belonging to this Category
    public ICollection<Skill> Skills { get; set; }
}