using AutoMapper;
using TicketCanvas.User.Api.Dtos;
using UserModel = TicketCanvas.User.Data.Models.User;

namespace TicketCanvas.Ticket.Api.MappingProfiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<RegisterUserRequest, UserModel>();
        CreateMap<UserModel, UserResponse>();
        CreateMap<UpdateUserRequest, UserModel>();
    }
}