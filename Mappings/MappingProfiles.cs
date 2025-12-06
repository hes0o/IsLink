using AutoMapper;
using FreelancerPlatform.Entities;
using FreelancerPlatform.Entities.Users;
using FreelancerPlatform.Entities.Gigs;
using FreelancerPlatform.Entities.Orders;
using FreelancerPlatform.DTOs;
using FreelancerPlatform.Entities.Enums;

namespace FreelancerPlatform.Mappings
{
    public class MappingProfiles : Profile
    {
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
                // Map List<string> from DTO to List<Tag> entities
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.Select(t => new Tag { Name = t }).ToList()))
                // Map List<string> from DTO to List<GigImage> entities
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.ImageUrls.Select(url => new GigImage { ImageUrl = url }).ToList()));

            CreateMap<CreateGigPackageDto, GigPackage>();

            // 2. Read (Entity -> DTO)
            CreateMap<Gig, GigDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.FreelancerName, opt => opt.MapFrom(src => src.Freelancer.Profile != null ? src.Freelancer.Profile.DisplayName : "Unknown"))
                .ForMember(dest => dest.FreelancerAvatar, opt => opt.MapFrom(src => src.Freelancer.Profile != null ? src.Freelancer.Profile.AvatarUrl : null))
                // Extract only the URL strings from the GigImage collection
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images.Select(img => img.ImageUrl).ToList()))
                // Extract only the Name strings from the Tag collection
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.Select(tag => tag.Name).ToList()));

            CreateMap<GigPackage, GigPackageDto>();

            // Order Mappings

            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.GigTitle, opt => opt.MapFrom(src => src.Gig.Title))
                .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => src.Client.Profile != null ? src.Client.Profile.DisplayName : src.Client.Email))
                .ForMember(dest => dest.FreelancerName, opt => opt.MapFrom(src => src.Freelancer.Profile != null ? src.Freelancer.Profile.DisplayName : src.Freelancer.Email));

            // After Order mappings
            CreateMap<Review, ReviewDto>()
                .ForMember(dest => dest.ClientName,
                    opt => opt.MapFrom(src => src.Order.Client.Profile != null
                        ? src.Order.Client.Profile.DisplayName
                        : src.Order.Client.Email));

            CreateMap<Conversation, ConversationDto>()
                .ForMember(dest => dest.Participants,
                opt => opt.MapFrom(src => src.Participants
                    .Select(p => p.User.Profile != null
                        ? p.User.Profile.DisplayName
                        : p.User.Email)
                    .ToList()));

            CreateMap<Message, MessageDto>()
                .ForMember(dest => dest.SenderName,
                    opt => opt.MapFrom(src => src.Sender.Profile != null
                        ? src.Sender.Profile.DisplayName
                        : src.Sender.Email));

        }
    }
}