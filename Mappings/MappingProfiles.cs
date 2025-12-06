using FreelancerPlatform.Entities;
using FreelancerPlatform.DTOs;
using FreelancerPlatform.Entities.Enums;

namespace FreelancerPlatform.Mappings
{
    // Inherit specifically from AutoMapper.Profile
    public class MappingProfiles : AutoMapper.Profile
    {
        // CONSTRUCTOR: All CreateMap calls MUST be inside here
        public MappingProfiles()
        {
            // User Mappings
            CreateMap<RegisterDto, User>();

            CreateMap<User, UserDto>()
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Profile != null ? src.Profile.DisplayName : "Unknown"))
                .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.Profile != null ? src.Profile.AvatarUrl : null));

            // Gig Mappings

            // 1. Create (DTO -> Entity)
            CreateMap<CreateGigDto, Gig>()
                // Map List<string> tags to List<Tag> entities
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.Select(t => new Tag { Name = t }).ToList()))
                // Map List<string> urls to List<GigImage> entities
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.ImageUrls.Select(url => new GigImage { ImageUrl = url }).ToList()));

            CreateMap<CreateGigPackageDto, GigPackage>();

            // 2. Read (Entity -> DTO)
            CreateMap<Gig, GigDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.FreelancerName, opt => opt.MapFrom(src => src.Freelancer.Profile != null ? src.Freelancer.Profile.DisplayName : "Unknown"))
                .ForMember(dest => dest.FreelancerAvatar, opt => opt.MapFrom(src => src.Freelancer.Profile != null ? src.Freelancer.Profile.AvatarUrl : null))
                // Convert Objects to Strings for Frontend
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images.Select(img => img.ImageUrl).ToList()))
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.Select(tag => tag.Name).ToList()));

            CreateMap<GigPackage, GigPackageDto>();

            // Order Mappings
            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.GigTitle, opt => opt.MapFrom(src => src.Gig.Title))
                .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => src.Client.Profile != null ? src.Client.Profile.DisplayName : src.Client.Email))
                .ForMember(dest => dest.FreelancerName, opt => opt.MapFrom(src => src.Freelancer.Profile != null ? src.Freelancer.Profile.DisplayName : src.Freelancer.Email));

            // --- Search Mappings ---
            CreateMap<Gig, GigSearchResultDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.FreelancerName, opt => opt.MapFrom(src => src.Freelancer.Profile != null ? src.Freelancer.Profile.DisplayName : "Unknown"))
                .ForMember(dest => dest.FreelancerAvatar, opt => opt.MapFrom(src => src.Freelancer.Profile != null ? src.Freelancer.Profile.AvatarUrl : null))
                // Logic: PriceFrom is the cheapest package price
                .ForMember(dest => dest.PriceFrom, opt => opt.MapFrom(src => src.Packages.Any() ? src.Packages.Min(p => p.Price) : 0))
                .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.AverageRating))
                // Logic: CoverImageUrl is the first image in the gallery
                .ForMember(dest => dest.CoverImageUrl, opt => opt.MapFrom(src => src.Images.Any() ? src.Images.First().ImageUrl : null));


            // --- Chat / Message Mappings ---

            // 1. Create Message (DTO -> Entity)
            CreateMap<CreateMessageDto, Message>();

            // 2. Read Message (Entity -> DTO)
            CreateMap<Message, MessageDto>()
                .ForMember(dest => dest.SenderName, opt => opt.MapFrom(src =>
                    src.Sender.Profile != null ? src.Sender.Profile.DisplayName : src.Sender.Email));

            // 3. Read Conversations (Entity -> DTO)
            CreateMap<Conversation, ConversationDto>()
                // Map the list of User entities to a list of names (Strings)
                .ForMember(dest => dest.Participants, opt => opt.MapFrom(src =>
                    src.Participants.Select(p =>
                        p.User.Profile != null ? p.User.Profile.DisplayName : p.User.Email).ToList()));
        }
    }
}