# Inventory Stock Management Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Implement Inventory stock checking and reservation with a persistent InventoryItem entity and IInventoryIntegrationService methods.

**Architecture:** Add a domain aggregate (InventoryItem) with business rules, an EF Core repository and mappings, and an application service implementing the integration interface. Tests cover domain rules, repository lookup, and service behavior.

**Tech Stack:** ABP Framework, EF Core, C#, xUnit, Shouldly.

---

## File Structure

- Create: `HQSOFT.Inventory/src/HQSOFT.Inventory.Domain/InventoryItems/InventoryItem.cs`
- Create: `HQSOFT.Inventory/src/HQSOFT.Inventory.Domain/InventoryItems/IInventoryItemRepository.cs`
- Modify: `HQSOFT.Inventory/src/HQSOFT.Inventory.Domain.Shared/InventoryErrorCodes.cs`
- Create: `HQSOFT.Inventory/src/HQSOFT.Inventory.Application.Contracts/Integration/IInventoryIntegrationService.cs`
- Create: `HQSOFT.Inventory/src/HQSOFT.Inventory.Application.Contracts/Integration/StockCheckResult.cs`
- Create: `HQSOFT.Inventory/src/HQSOFT.Inventory.Application/Integration/InventoryIntegrationService.cs`
- Modify: `HQSOFT.Inventory/src/HQSOFT.Inventory.EntityFrameworkCore/EntityFrameworkCore/IInventoryDbContext.cs`
- Modify: `HQSOFT.Inventory/src/HQSOFT.Inventory.EntityFrameworkCore/EntityFrameworkCore/InventoryDbContext.cs`
- Modify: `HQSOFT.Inventory/src/HQSOFT.Inventory.EntityFrameworkCore/EntityFrameworkCore/InventoryDbContextModelCreatingExtensions.cs`
- Create: `HQSOFT.Inventory/src/HQSOFT.Inventory.EntityFrameworkCore/Repositories/EfCoreInventoryItemRepository.cs`
- Modify: `HQSOFT.Inventory/src/HQSOFT.Inventory.EntityFrameworkCore/EntityFrameworkCore/InventoryEntityFrameworkCoreModule.cs`
- Create: `HQSOFT.Inventory/test/HQSOFT.Inventory.Domain.Tests/InventoryItems/InventoryItem_Tests.cs`
- Create: `HQSOFT.Inventory/test/HQSOFT.Inventory.Application.Tests/Integration/InventoryIntegrationService_Tests.cs`
- Create: `HQSOFT.Inventory/test/HQSOFT.Inventory.EntityFrameworkCore.Tests/Repositories/InventoryItemRepository_Tests.cs`

---

### Task 1: Domain aggregate + domain tests

**Files:**
- Create: `HQSOFT.Inventory/src/HQSOFT.Inventory.Domain/InventoryItems/InventoryItem.cs`
- Modify: `HQSOFT.Inventory/src/HQSOFT.Inventory.Domain.Shared/InventoryErrorCodes.cs`
- Create: `HQSOFT.Inventory/test/HQSOFT.Inventory.Domain.Tests/InventoryItems/InventoryItem_Tests.cs`

- [ ] **Step 1: Write failing domain tests**

```csharp
using System;
using Shouldly;
using Volo.Abp.Modularity;
using Volo.Abp;
using Xunit;

namespace HQSOFT.Inventory.InventoryItems;

public abstract class InventoryItem_Tests<TStartupModule> : InventoryDomainTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    [Fact]
    public void CanReserve_returns_true_when_available()
    {
        var item = new InventoryItem(Guid.NewGuid(), Guid.NewGuid(), "P001", "Product", 10, 2);

        item.CanReserve(5).ShouldBeTrue();
    }

    [Fact]
    public void CanReserve_returns_false_when_insufficient()
    {
        var item = new InventoryItem(Guid.NewGuid(), Guid.NewGuid(), "P001", "Product", 5, 5);

        item.CanReserve(1).ShouldBeFalse();
    }

    [Fact]
    public void Reserve_throws_when_insufficient()
    {
        var item = new InventoryItem(Guid.NewGuid(), Guid.NewGuid(), "P001", "Product", 1, 1);

        Should.Throw<BusinessException>(() => item.Reserve(1, "R1"))
            .Code.ShouldBe(InventoryErrorCodes.InsufficientStock);
    }

    [Fact]
    public void Reserve_increases_reserved_quantity()
    {
        var item = new InventoryItem(Guid.NewGuid(), Guid.NewGuid(), "P001", "Product", 10, 2);

        item.Reserve(3, "R1");

        item.ReservedQuantity.ShouldBe(5);
        item.AvailableQuantity.ShouldBe(5);
    }
}
```

- [ ] **Step 2: Run domain tests (expect fail)**

Run: `dotnet test test/HQSOFT.Inventory.Domain.Tests`
Expected: FAIL (InventoryItem not found)

- [ ] **Step 3: Implement InventoryItem entity + error code**

`HQSOFT.Inventory/src/HQSOFT.Inventory.Domain/InventoryItems/InventoryItem.cs`:

```csharp
using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp;

namespace HQSOFT.Inventory.InventoryItems;

public class InventoryItem : FullAuditedAggregateRoot<Guid>
{
    public Guid ProductId { get; private set; }
    public string ProductCode { get; private set; } = null!;
    public string ProductName { get; private set; } = null!;
    public int Quantity { get; private set; }
    public int ReservedQuantity { get; private set; }

    public int AvailableQuantity => Quantity - ReservedQuantity;

    protected InventoryItem() { }

    public InventoryItem(Guid id, Guid productId, string productCode, string productName, int quantity, int reservedQuantity = 0)
        : base(id)
    {
        ProductId = productId;
        ProductCode = productCode;
        ProductName = productName;
        Quantity = quantity;
        ReservedQuantity = reservedQuantity;
    }

    public bool CanReserve(int quantity)
    {
        return quantity > 0 && AvailableQuantity >= quantity;
    }

    public void Reserve(int quantity, string reservationId)
    {
        if (!CanReserve(quantity))
        {
            throw new BusinessException(InventoryErrorCodes.InsufficientStock);
        }

        ReservedQuantity += quantity;
    }
}
```

`HQSOFT.Inventory/src/HQSOFT.Inventory.Domain.Shared/InventoryErrorCodes.cs`:

```csharp
namespace HQSOFT.Inventory;

public static class InventoryErrorCodes
{
    public const string InsufficientStock = "Inventory:InsufficientStock";
}
```

- [ ] **Step 4: Run domain tests (expect pass)**

Run: `dotnet test test/HQSOFT.Inventory.Domain.Tests`
Expected: PASS

- [ ] **Step 5: Commit**

```bash
git add HQSOFT.Inventory/src/HQSOFT.Inventory.Domain/InventoryItems/InventoryItem.cs \
        HQSOFT.Inventory/src/HQSOFT.Inventory.Domain.Shared/InventoryErrorCodes.cs \
        HQSOFT.Inventory/test/HQSOFT.Inventory.Domain.Tests/InventoryItems/InventoryItem_Tests.cs

git commit -m "feat(inventory): add InventoryItem domain rules"
```

---

### Task 2: Repository interface + EF Core mapping + repository tests

**Files:**
- Create: `HQSOFT.Inventory/src/HQSOFT.Inventory.Domain/InventoryItems/IInventoryItemRepository.cs`
- Modify: `HQSOFT.Inventory/src/HQSOFT.Inventory.EntityFrameworkCore/EntityFrameworkCore/IInventoryDbContext.cs`
- Modify: `HQSOFT.Inventory/src/HQSOFT.Inventory.EntityFrameworkCore/EntityFrameworkCore/InventoryDbContext.cs`
- Modify: `HQSOFT.Inventory/src/HQSOFT.Inventory.EntityFrameworkCore/EntityFrameworkCore/InventoryDbContextModelCreatingExtensions.cs`
- Create: `HQSOFT.Inventory/src/HQSOFT.Inventory.EntityFrameworkCore/Repositories/EfCoreInventoryItemRepository.cs`
- Modify: `HQSOFT.Inventory/src/HQSOFT.Inventory.EntityFrameworkCore/EntityFrameworkCore/InventoryEntityFrameworkCoreModule.cs`
- Create: `HQSOFT.Inventory/test/HQSOFT.Inventory.EntityFrameworkCore.Tests/Repositories/InventoryItemRepository_Tests.cs`

- [ ] **Step 1: Write failing repository test**

```csharp
using System;
using System.Threading.Tasks;
using Shouldly;
using Volo.Abp.Modularity;
using Xunit;

namespace HQSOFT.Inventory.InventoryItems;

public abstract class InventoryItemRepository_Tests<TStartupModule> : InventoryEntityFrameworkCoreTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly IInventoryItemRepository _repository;

    protected InventoryItemRepository_Tests()
    {
        _repository = GetRequiredService<IInventoryItemRepository>();
    }

    [Fact]
    public async Task FindByProductIdAsync_returns_item()
    {
        var productId = Guid.NewGuid();
        var item = new InventoryItem(Guid.NewGuid(), productId, "P001", "Product", 10, 0);

        await _repository.InsertAsync(item, autoSave: true);

        var result = await _repository.FindByProductIdAsync(productId);

        result.ShouldNotBeNull();
        result!.ProductId.ShouldBe(productId);
    }
}
```

- [ ] **Step 2: Run EF Core tests (expect fail)**

Run: `dotnet test test/HQSOFT.Inventory.EntityFrameworkCore.Tests`
Expected: FAIL (missing repository/DbSet)

- [ ] **Step 3: Implement repository interface + EF Core mapping**

`HQSOFT.Inventory/src/HQSOFT.Inventory.Domain/InventoryItems/IInventoryItemRepository.cs`:

```csharp
using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace HQSOFT.Inventory.InventoryItems;

public interface IInventoryItemRepository : IRepository<InventoryItem, Guid>
{
    Task<InventoryItem?> FindByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
}
```

`HQSOFT.Inventory/src/HQSOFT.Inventory.EntityFrameworkCore/EntityFrameworkCore/IInventoryDbContext.cs`:

```csharp
using Microsoft.EntityFrameworkCore;
using HQSOFT.Inventory.InventoryItems;

public interface IInventoryDbContext : IEfCoreDbContext
{
    DbSet<InventoryItem> InventoryItems { get; }
}
```

`HQSOFT.Inventory/src/HQSOFT.Inventory.EntityFrameworkCore/EntityFrameworkCore/InventoryDbContext.cs`:

```csharp
public DbSet<InventoryItem> InventoryItems { get; set; } = null!;
```

`HQSOFT.Inventory/src/HQSOFT.Inventory.EntityFrameworkCore/EntityFrameworkCore/InventoryDbContextModelCreatingExtensions.cs`:

```csharp
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
```

`HQSOFT.Inventory/src/HQSOFT.Inventory.EntityFrameworkCore/Repositories/EfCoreInventoryItemRepository.cs`:

```csharp
using System;
using System.Threading;
using System.Threading.Tasks;
using HQSOFT.Inventory.InventoryItems;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace HQSOFT.Inventory.EntityFrameworkCore.Repositories;

public class EfCoreInventoryItemRepository
    : EfCoreRepository<InventoryDbContext, InventoryItem, Guid>, IInventoryItemRepository
{
    public EfCoreInventoryItemRepository(IDbContextProvider<InventoryDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<InventoryItem?> FindByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
            .FirstOrDefaultAsync(x => x.ProductId == productId, cancellationToken);
    }
}
```

`HQSOFT.Inventory/src/HQSOFT.Inventory.EntityFrameworkCore/EntityFrameworkCore/InventoryEntityFrameworkCoreModule.cs`:

```csharp
options.AddRepository<InventoryItem, EfCoreInventoryItemRepository>();
```

- [ ] **Step 4: Run EF Core tests (expect pass)**

Run: `dotnet test test/HQSOFT.Inventory.EntityFrameworkCore.Tests`
Expected: PASS

- [ ] **Step 5: Commit**

```bash
git add HQSOFT.Inventory/src/HQSOFT.Inventory.Domain/InventoryItems/IInventoryItemRepository.cs \
        HQSOFT.Inventory/src/HQSOFT.Inventory.EntityFrameworkCore/EntityFrameworkCore/IInventoryDbContext.cs \
        HQSOFT.Inventory/src/HQSOFT.Inventory.EntityFrameworkCore/EntityFrameworkCore/InventoryDbContext.cs \
        HQSOFT.Inventory/src/HQSOFT.Inventory.EntityFrameworkCore/EntityFrameworkCore/InventoryDbContextModelCreatingExtensions.cs \
        HQSOFT.Inventory/src/HQSOFT.Inventory.EntityFrameworkCore/Repositories/EfCoreInventoryItemRepository.cs \
        HQSOFT.Inventory/src/HQSOFT.Inventory.EntityFrameworkCore/EntityFrameworkCore/InventoryEntityFrameworkCoreModule.cs \
        HQSOFT.Inventory/test/HQSOFT.Inventory.EntityFrameworkCore.Tests/Repositories/InventoryItemRepository_Tests.cs

git commit -m "feat(inventory): add repository and EF Core mapping"
```

---

### Task 3: Contracts + application service tests

**Files:**
- Create: `HQSOFT.Inventory/src/HQSOFT.Inventory.Application.Contracts/Integration/IInventoryIntegrationService.cs`
- Create: `HQSOFT.Inventory/src/HQSOFT.Inventory.Application.Contracts/Integration/StockCheckResult.cs`
- Create: `HQSOFT.Inventory/test/HQSOFT.Inventory.Application.Tests/Integration/InventoryIntegrationService_Tests.cs`

- [ ] **Step 1: Write failing application tests**

```csharp
using System;
using System.Threading.Tasks;
using Shouldly;
using Volo.Abp.Modularity;
using Xunit;

namespace HQSOFT.Inventory.Integration;

public abstract class InventoryIntegrationService_Tests<TStartupModule> : InventoryApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly IInventoryIntegrationService _service;
    private readonly IInventoryItemRepository _repository;

    protected InventoryIntegrationService_Tests()
    {
        _service = GetRequiredService<IInventoryIntegrationService>();
        _repository = GetRequiredService<IInventoryItemRepository>();
    }

    [Fact]
    public async Task CheckStockAsync_returns_availability()
    {
        var productId = Guid.NewGuid();
        await _repository.InsertAsync(new InventoryItem(Guid.NewGuid(), productId, "P001", "Product", 10, 2), autoSave: true);

        var result = await _service.CheckStockAsync(productId, 5);

        result.IsAvailable.ShouldBeTrue();
        result.AvailableQuantity.ShouldBe(8);
        result.ProductCode.ShouldBe("P001");
        result.ProductName.ShouldBe("Product");
    }

    [Fact]
    public async Task ReserveStockAsync_returns_false_when_insufficient()
    {
        var productId = Guid.NewGuid();
        await _repository.InsertAsync(new InventoryItem(Guid.NewGuid(), productId, "P001", "Product", 1, 1), autoSave: true);

        var result = await _service.ReserveStockAsync(productId, 1, "R1");

        result.ShouldBeFalse();
    }

    [Fact]
    public async Task ReserveStockAsync_reserves_when_available()
    {
        var productId = Guid.NewGuid();
        var item = new InventoryItem(Guid.NewGuid(), productId, "P001", "Product", 10, 0);
        await _repository.InsertAsync(item, autoSave: true);

        var result = await _service.ReserveStockAsync(productId, 3, "R1");

        result.ShouldBeTrue();
        (await _repository.FindByProductIdAsync(productId))!.ReservedQuantity.ShouldBe(3);
    }
}
```

- [ ] **Step 2: Run application tests (expect fail)**

Run: `dotnet test test/HQSOFT.Inventory.Application.Tests`
Expected: FAIL (service/interface missing)

- [ ] **Step 3: Implement contracts**

`HQSOFT.Inventory/src/HQSOFT.Inventory.Application.Contracts/Integration/IInventoryIntegrationService.cs`:

```csharp
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace HQSOFT.Inventory.Integration;

public interface IInventoryIntegrationService : IApplicationService
{
    Task<StockCheckResult> CheckStockAsync(Guid productId, int requestedQuantity);
    Task<bool> ReserveStockAsync(Guid productId, int quantity, string reservationId);
}
```

`HQSOFT.Inventory/src/HQSOFT.Inventory.Application.Contracts/Integration/StockCheckResult.cs`:

```csharp
namespace HQSOFT.Inventory.Integration;

public class StockCheckResult
{
    public bool IsAvailable { get; set; }
    public int AvailableQuantity { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
}
```

- [ ] **Step 4: Run application tests (expect fail)**

Run: `dotnet test test/HQSOFT.Inventory.Application.Tests`
Expected: FAIL (service implementation missing)

- [ ] **Step 5: Commit**

```bash
git add HQSOFT.Inventory/src/HQSOFT.Inventory.Application.Contracts/Integration/IInventoryIntegrationService.cs \
        HQSOFT.Inventory/src/HQSOFT.Inventory.Application.Contracts/Integration/StockCheckResult.cs \
        HQSOFT.Inventory/test/HQSOFT.Inventory.Application.Tests/Integration/InventoryIntegrationService_Tests.cs

git commit -m "feat(inventory): add inventory integration contracts"
```

---

### Task 4: Application service implementation

**Files:**
- Create: `HQSOFT.Inventory/src/HQSOFT.Inventory.Application/Integration/InventoryIntegrationService.cs`
- Modify: `HQSOFT.Inventory/src/HQSOFT.Inventory.Application/InventoryApplicationModule.cs`

- [ ] **Step 1: Implement service**

```csharp
using System;
using System.Threading.Tasks;
using HQSOFT.Inventory.InventoryItems;
using Volo.Abp.Application.Services;

namespace HQSOFT.Inventory.Integration;

public class InventoryIntegrationService : InventoryAppService, IInventoryIntegrationService
{
    private readonly IInventoryItemRepository _repository;

    public InventoryIntegrationService(IInventoryItemRepository repository)
    {
        _repository = repository;
    }

    public async Task<StockCheckResult> CheckStockAsync(Guid productId, int requestedQuantity)
    {
        var item = await _repository.FindByProductIdAsync(productId);

        if (item == null)
        {
            return new StockCheckResult
            {
                IsAvailable = false,
                AvailableQuantity = 0,
                ProductCode = string.Empty,
                ProductName = string.Empty
            };
        }

        return new StockCheckResult
        {
            IsAvailable = item.AvailableQuantity >= requestedQuantity,
            AvailableQuantity = item.AvailableQuantity,
            ProductCode = item.ProductCode,
            ProductName = item.ProductName
        };
    }

    public async Task<bool> ReserveStockAsync(Guid productId, int quantity, string reservationId)
    {
        var item = await _repository.FindByProductIdAsync(productId);

        if (item == null)
        {
            return false;
        }

        if (!item.CanReserve(quantity))
        {
            return false;
        }

        item.Reserve(quantity, reservationId);
        await _repository.UpdateAsync(item, autoSave: true);

        return true;
    }
}
```

- [ ] **Step 2: Ensure Application module registers services**

If module uses conventional registration, no change needed. If not, add to `InventoryApplicationModule`:

```csharp
context.Services.AddTransient<IInventoryIntegrationService, InventoryIntegrationService>();
```

- [ ] **Step 3: Run application tests (expect pass)**

Run: `dotnet test test/HQSOFT.Inventory.Application.Tests`
Expected: PASS

- [ ] **Step 4: Commit**

```bash
git add HQSOFT.Inventory/src/HQSOFT.Inventory.Application/Integration/InventoryIntegrationService.cs \
        HQSOFT.Inventory/src/HQSOFT.Inventory.Application/InventoryApplicationModule.cs

git commit -m "feat(inventory): implement inventory integration service"
```

---

## Plan Self-Review

- **Spec coverage:** All requirements mapped to Tasks 1–4; DB mapping and repository tests included.
- **Placeholder scan:** No TODO/TBD.
- **Type consistency:** Method signatures and DTO properties match spec and huongdan.md.

---

## Execution Handoff

Plan complete and saved to `docs/superpowers/plans/2026-03-26-inventory-stock-management.md`. Two execution options:

1. **Subagent-Driven (recommended)** - I dispatch a fresh subagent per task, review between tasks, fast iteration
2. **Inline Execution** - Execute tasks in this session using executing-plans, batch execution with checkpoints

Which approach?
