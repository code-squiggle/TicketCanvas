using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketCanvas.Ticket.Domain.Aggregates;

namespace TicketCanvas.Ticket.Infrastructure.Persistence.Write.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasIndex(o => o.IdempotencyKey)
            .IsUnique();

        builder.OwnsOne(o => o.TotalAmount, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName(nameof(Order.TotalAmount));
            money.Property(m => m.Currency)
                .HasColumnName(nameof(Order.TotalAmount.Currency));
        });

        builder.Property(o => o.Status)
            .IsConcurrencyToken();
    }

}