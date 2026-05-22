using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketCanvas.Ticket.Domain.Aggregates;

namespace TicketCanvas.Ticket.Infrastructure.Persistence.Write.Configurations;

public class TicketAllocationConfiguration : IEntityTypeConfiguration<TicketAllocation>
{
    public void Configure(EntityTypeBuilder<TicketAllocation> builder)
    {
        builder.OwnsOne(o => o.Price, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName(nameof(TicketAllocation.Price));
            money.Property(m => m.Currency)
                .HasColumnName(nameof(TicketAllocation.Price.Currency));
        });
        builder.Property(o => o.AvailableQuantity)
            .IsConcurrencyToken();
    }

}