using AutoMapper;
using Backend.Dtos;
using Backend.Dtos.TrelloDto;
using Backend.Models;

namespace Backend.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<UserForRegisterDto, User>();
            CreateMap<CreateProjectDto, Project>();
            CreateMap<User, UserForDetailedDto>();
            CreateMap<User, UserForListDto>();
            CreateMap<User, UserForLoginDto>();
            CreateMap<TrelloBoardDto, TrelloBoard>();
            CreateMap<LoggedInUserDto, User>();
            CreateMap<CustomerListDto, Customer>();
        }
    }
}