public interface ICategoryRepository
{
    Task<List<Category>> GetAllAsync();
}
