using AutoMapper;
using TicketCanvas.Common.Application.IntegrationEvents;
using TicketCanvas.Show.Api.Dto;
using TicketCanvas.Show.Data.Models;
using ShowModel = TicketCanvas.Show.Data.Models.Show;
using TicketType = TicketCanvas.Show.Data.Models.TicketType;
using TicketTypeIntegrationEvent = TicketCanvas.Common.Application.IntegrationEvents.TicketType;

namespace TicketCanvas.Show.Api.MappingProfiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ShowModel, CreateShowRequest>();
        CreateMap<CreateShowRequest, ShowModel>();
        CreateMap<TicketType, CreateTicketTypeRequest>();
        CreateMap<CreateTicketTypeRequest, TicketType>();
        CreateMap<ShowModel, ShowSummaryResponse>(); 
        CreateMap<Venue, VenueResponse>(); 
        CreateMap<UpdateShowRequest, ShowModel>(); 
        CreateMap<ShowModel, ShowDetailResponse>(); 
        CreateMap<TicketType, CreateTicketTypeRequest>(); 
        CreateMap<TicketType, UpdateTicketTypeRequest>(); 
        CreateMap<TicketType, TicketTypeResponse>(); 
        CreateMap<ShowModel, ShowPublished>();
        CreateMap<TicketType, TicketTypeIntegrationEvent>();
    }
}