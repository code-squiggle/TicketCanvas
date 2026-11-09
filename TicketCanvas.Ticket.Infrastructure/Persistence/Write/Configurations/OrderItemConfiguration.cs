using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketCanvas.Ticket.Domain.Aggregates;

namespace TicketCanvas.Ticket.Infrastructure.Persistence.Write.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");

        builder.OwnsOne(o => o.UnitPrice, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName(nameof(OrderItem.UnitPrice));
            money.Property(m => m.Currency)
                .HasColumnName(nameof(OrderItem.UnitPrice.Currency));
        });
    }

}