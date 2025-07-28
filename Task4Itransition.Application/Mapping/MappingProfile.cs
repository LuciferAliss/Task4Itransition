using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Task4Itransition.Application.DTO.User;
using Task4Itransition.Domain.Entities;

namespace Task4Itransition.Application.Mapping;

public class MappingProfile : Profile
{    
    public MappingProfile()
    {
        CreateMap<User, UserDto>().ForMember(dest => dest.IsBlocked, opt => opt.MapFrom(src => src.LockoutEnd.HasValue && src.LockoutEnd.Value > DateTimeOffset.UtcNow));
    }
}
