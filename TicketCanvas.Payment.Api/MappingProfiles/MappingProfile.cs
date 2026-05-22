using AutoMapper;
using TicketCanvas.Common.Application.IntegrationEvents;
using TicketCanvas.Payment.Api.Dtos;
using PaymentModel = TicketCanvas.Payment.Data.Models.Payment;

namespace TicketCanvas.Payment.Api.MappingProfiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<OrderPlaced, PaymentModel>()
            .ForMember(dest => dest.OrderId, o => o.MapFrom(src => src.Id))
            .ForMember(dest => dest.Amount, o => o.MapFrom(src => src.TotalAmount));

        CreateMap<PaymentModel, PaymentResponse>();
    }
}