using HQSOFT.Order.SaleOrders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace HQSOFT.Order.EntityFrameworkCore.Configurations;

public class SalesOrderLineConfiguration : IEntityTypeConfiguration<SalesOrderLine>
{
    public void Configure(EntityTypeBuilder<SalesOrderLine> builder)
    {
        builder.ToTable(OrderConsts.DbTablePrefix + "SaleOrderLines", OrderConsts.DbSchema);

        builder.ConfigureByConvention();

        builder.Property(x => x.ProductName)
            .IsRequired()
            .HasMaxLength(SalesOrderConsts.MaxProductName);

        builder.Property(x => x.ProductCode)
            .IsRequired()
            .HasMaxLength(SalesOrderConsts.MaxProductCode);

        builder.OwnsOne(x => x.UnitPrice, money =>
        {
            money.Property(m => m.Amount)
                .HasPrecision(18, 4);

            money.Property(m => m.Currency)
                .HasMaxLength(8);
        });

        builder.OwnsOne(x => x.LineTotal, total =>
        {
            total.Property(t => t.Amount)
                .HasPrecision(18, 4);

            total.Property(t => t.Currency)
                .HasMaxLength(8);
        });
    }
}
