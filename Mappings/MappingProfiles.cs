using FreelancerPlatform.Entities;
using FreelancerPlatform.DTOs;
using FreelancerPlatform.Entities.Enums;


namespace FreelancerPlatform.Mappings
{
    public class MappingProfiles : AutoMapper.Profile
    {
        public MappingProfiles()
        {
            // --- User Mappings ---
            CreateMap<RegisterDto, User>();
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Profile != null ? src.Profile.DisplayName : "Unknown"))
                .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.Profile != null ? src.Profile.AvatarUrl : null));

            // --- Gig Mappings ---
            CreateMap<CreateGigDto, Gig>();
            CreateMap<CreateGigPackageDto, GigPackage>();

            CreateMap<Gig, GigDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.FreelancerName, opt => opt.MapFrom(src => src.Freelancer.Profile != null ? src.Freelancer.Profile.DisplayName : "Unknown"))
                .ForMember(dest => dest.FreelancerAvatar, opt => opt.MapFrom(src => src.Freelancer.Profile != null ? src.Freelancer.Profile.AvatarUrl : null));

            CreateMap<GigPackage, GigPackageDto>();

            // --- Order Mappings ---
            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.GigTitle, opt => opt.MapFrom(src => src.Gig.Title))
                .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => src.Client.Profile != null ? src.Client.Profile.DisplayName : src.Client.Email))
                .ForMember(dest => dest.FreelancerName, opt => opt.MapFrom(src => src.Freelancer.Profile != null ? src.Freelancer.Profile.DisplayName : src.Freelancer.Email));
        }
    }
}