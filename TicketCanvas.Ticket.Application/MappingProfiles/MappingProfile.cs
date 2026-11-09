using AutoMapper;
using TicketCanvas.Ticket.Application.Dtos;
using TicketCanvas.Ticket.Domain.Aggregates;
using TicketModel = TicketCanvas.Ticket.Application.ReadModels.Ticket;

namespace TicketCanvas.Ticket.Application.MappingProfiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Order, OrderSummaryResponse>();
        CreateMap<Order, OrderDetailResponse>();
        CreateMap<OrderItem, OrderItemResponse>();
        CreateMap<TicketModel, TicketSummaryResponse>()
            .ForMember(nameof(TicketSummaryResponse.UserId), opt => opt.MapFrom(ticket => ticket.OrderItem.Order.UserId))
            .ForMember(nameof(TicketSummaryResponse.ShowName), opt => opt.MapFrom(ticket => ticket.OrderItem.ShowName))
            .ForMember(nameof(TicketSummaryResponse.TicketTypeName), opt => opt.MapFrom(ticket => ticket.OrderItem.TicketTypeName));
    }
}