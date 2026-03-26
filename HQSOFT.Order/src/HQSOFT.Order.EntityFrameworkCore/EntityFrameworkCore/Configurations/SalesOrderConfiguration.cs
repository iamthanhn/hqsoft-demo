using HQSOFT.Order.SaleOrders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace HQSOFT.Order.EntityFrameworkCore.Configurations;

public class SalesOrderConfiguration : IEntityTypeConfiguration<SalesOrder>
{
    public void Configure(EntityTypeBuilder<SalesOrder> builder)
    {
        builder.ToTable(OrderConsts.DbTablePrefix + "SaleOrders", OrderConsts.DbSchema);

        builder.ConfigureByConvention();

        builder.Property(x => x.OrderNumber)
            .IsRequired()
            .HasMaxLength(SalesOrderConsts.MaxOrderNumber);

        builder.Property(x => x.OrderDate);

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.HasMany(x => x.OrderLines)
            .WithOne()
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.OrderLines)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
