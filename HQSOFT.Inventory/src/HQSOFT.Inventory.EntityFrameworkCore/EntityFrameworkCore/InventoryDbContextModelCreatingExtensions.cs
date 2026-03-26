using HQSOFT.Inventory.InventoryItems;
using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace HQSOFT.Inventory.EntityFrameworkCore;

public static class InventoryDbContextModelCreatingExtensions
{
    public static void ConfigureInventory(
        this ModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));

        builder.Entity<InventoryItem>(b =>
        {
            b.ToTable(InventoryDbProperties.DbTablePrefix + "InventoryItems", InventoryDbProperties.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.ProductId).IsRequired();
            b.Property(x => x.ProductCode).IsRequired().HasMaxLength(50);
            b.Property(x => x.ProductName).IsRequired().HasMaxLength(200);
            b.Property(x => x.Quantity).IsRequired();
            b.Property(x => x.ReservedQuantity).IsRequired();

            b.HasIndex(x => x.ProductId);
        });
    }
}
