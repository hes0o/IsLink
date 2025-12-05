namespace FreelancerPlatform.Entities.Enums
{
    public enum UserRole
    {
        Client = 0,
        Freelancer = 1,
        Admin = 2
    }

    public enum GigStatus
    {
        Draft = 0,
        Active = 1,
        Paused = 2,
        Deleted = 3
    }

    public enum OrderStatus
    {
        Pending = 0,
        InProgress = 1,
        Delivered = 2,
        Completed = 3,
        Cancelled = 4,
        Disputed = 5
    }
}