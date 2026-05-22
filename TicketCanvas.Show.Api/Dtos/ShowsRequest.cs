using TicketCanvas.Common.Application.Dtos;
using TicketCanvas.Show.Data.Models;

namespace TicketCanvas.Show.Api.Dtos;

public record ShowsRequest : PagedRequest
{
    public ShowStatus? Status { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public string? City { get; init; }
 }