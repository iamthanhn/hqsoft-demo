using System;
using System.Threading.Tasks;
using HQSOFT.Inventory.InventoryItems;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;

namespace HQSOFT.Inventory;

public class InventoryItemDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IRepository<InventoryItem, Guid> _inventoryItemRepository;
    private readonly IGuidGenerator _guidGenerator;

    public InventoryItemDataSeedContributor(
        IRepository<InventoryItem, Guid> inventoryItemRepository,
        IGuidGenerator guidGenerator)
    {
        _inventoryItemRepository = inventoryItemRepository;
        _guidGenerator = guidGenerator;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        if (await _inventoryItemRepository.GetCountAsync() > 0)
        {
            return;
        }

        var sampleProducts = new[]
        {
            new InventoryItem(_guidGenerator.Create(), _guidGenerator.Create(), "LAPTOP-001",
                "Dell Latitude 5420 Laptop", 50, 5),
            new InventoryItem(_guidGenerator.Create(), _guidGenerator.Create(), "MOUSE-001",
                "Logitech MX Master 3 Mouse", 200, 15),
            new InventoryItem(_guidGenerator.Create(), _guidGenerator.Create(), "KEYBOARD-001",
                "Keychron K8 Mechanical Keyboard", 100, 10),
            new InventoryItem(_guidGenerator.Create(), _guidGenerator.Create(), "MONITOR-001",
                "Dell UltraSharp 27 4K Monitor", 30, 3),
            new InventoryItem(_guidGenerator.Create(), _guidGenerator.Create(), "HEADSET-001",
                "Sony WH-1000XM5 Headset", 75, 8),
            new InventoryItem(_guidGenerator.Create(), _guidGenerator.Create(), "WEBCAM-001", "Logitech Brio 4K Webcam",
                60, 6),
            new InventoryItem(_guidGenerator.Create(), _guidGenerator.Create(), "DOCKING-001",
                "Dell WD19TB Thunderbolt Dock", 40, 4),
            new InventoryItem(_guidGenerator.Create(), _guidGenerator.Create(), "CHAIR-001",
                "Herman Miller Aeron Chair", 25, 2),
            new InventoryItem(_guidGenerator.Create(), _guidGenerator.Create(), "DESK-001", "Uplift V2 Standing Desk",
                20, 1),
            new InventoryItem(_guidGenerator.Create(), _guidGenerator.Create(), "TABLET-001", "iPad Pro 12.9 inch", 80,
                7),
            new InventoryItem(_guidGenerator.Create(), _guidGenerator.Create(), "PHONE-001", "iPhone 15 Pro Max", 120,
                12),
            new InventoryItem(_guidGenerator.Create(), _guidGenerator.Create(), "PRINTER-001", "HP LaserJet Pro M404dn",
                15, 1),
            new InventoryItem(_guidGenerator.Create(), _guidGenerator.Create(), "ROUTER-001",
                "Ubiquiti UniFi Dream Machine", 35, 3),
            new InventoryItem(_guidGenerator.Create(), _guidGenerator.Create(), "SSD-001",
                "Samsung 990 Pro 2TB NVMe SSD", 150, 20),
            new InventoryItem(_guidGenerator.Create(), _guidGenerator.Create(), "RAM-001",
                "Corsair Vengeance 32GB DDR5", 180, 18)
        };

        await _inventoryItemRepository.InsertManyAsync(sampleProducts, autoSave: true);
    }
}
