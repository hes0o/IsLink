using FreelancerPlatform.Entities; 
using FreelancerPlatform.Services.Interfaces;


namespace FreelancerPlatform.Services.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}
