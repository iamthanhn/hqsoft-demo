# Inventory Stock Management Design

**Date:** 2026-03-26
**Module:** HQSOFT.Inventory
**Purpose:** Implement stock checking and reservation functionality for ServiceOrder integration

## Overview

Implement `CheckStockAsync` and `ReserveStockAsync` methods in the Inventory module to support ServiceOrder's stock validation and reservation workflow. The implementation follows ABP Framework patterns with full domain-driven design.

## Requirements

ServiceOrder module needs to:
1. Check if sufficient stock is available for a product
2. Reserve stock when creating an order
3. Get product details (code, name) along with availability info

Business rule: Reservations fail completely if insufficient stock (no partial reservations).

## Architecture

### Domain Layer

**InventoryItem Entity** (AggregateRoot<Guid>):
- Inherits from `FullAuditedAggregateRoot<Guid>` for audit trail
- Properties:
  - `ProductId` (Guid, required, indexed) - references product in another module
  - `ProductCode` (string, max 50, required) - for display/reporting
  - `ProductName` (string, max 200, required) - for display/reporting
  - `Quantity` (int, >= 0) - total physical stock
  - `ReservedQuantity` (int, >= 0) - amount currently reserved
  - Audit fields inherited: CreationTime, CreatorId, LastModificationTime, LastModifierId, IsDeleted, DeletionTime, DeleterId

**Computed Property:**
```csharp
public int AvailableQuantity => Quantity - ReservedQuantity;
```

**Business Methods:**
```csharp
public bool CanReserve(int quantity)
{
    return quantity > 0 && AvailableQuantity >= quantity;
}

public void Reserve(int quantity, string reservationId)
{
    if (!CanReserve(quantity))
        throw new BusinessException(InventoryErrorCodes.InsufficientStock);

    ReservedQuantity += quantity;
}
```

**Repository Interface:**
```csharp
public interface IInventoryItemRepository : IRepository<InventoryItem, Guid>
{
    Task<InventoryItem> FindByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
}
```

### Application Layer

**InventoryIntegrationService** implements `IInventoryIntegrationService`:

**CheckStockAsync:**
1. Query InventoryItem by ProductId using repository
2. If not found: return IsAvailable=false, AvailableQuantity=0
3. If found: map to StockCheckResult with product details and availability

**ReserveStockAsync:**
1. Find InventoryItem by ProductId
2. If not found: return false
3. Call `item.CanReserve(quantity)`
4. If false: return false (insufficient stock)
5. If true: call `item.Reserve(quantity, reservationId)`, save via repository, return true

**Error Handling:**
- Use ABP's BusinessException for domain rule violations
- Define error code `InventoryErrorCodes.InsufficientStock`
- Let repository exceptions bubble up (ABP handles them)

### EF Core Layer

**DbContext Changes:**
- Add `DbSet<InventoryItem> InventoryItems { get; set; }`

**Entity Configuration (in InventoryDbContextModelCreatingExtensions):**
```csharp
builder.Entity<InventoryItem>(b =>
{
    b.ToTable(InventoryDbProperties.DbTablePrefix + "InventoryItems", InventoryDbProperties.DbSchema);
    b.ConfigureByConvention(); // ABP audit fields

    b.Property(x => x.ProductId).IsRequired();
    b.Property(x => x.ProductCode).IsRequired().HasMaxLength(50);
    b.Property(x => x.ProductName).IsRequired().HasMaxLength(200);
    b.Property(x => x.Quantity).IsRequired();
    b.Property(x => x.ReservedQuantity).IsRequired();

    b.HasIndex(x => x.ProductId); // for fast lookup
});
```

**Repository Implementation:**
```csharp
public class EfCoreInventoryItemRepository : EfCoreRepository<InventoryDbContext, InventoryItem, Guid>, IInventoryItemRepository
{
    public async Task<InventoryItem> FindByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
            .FirstOrDefaultAsync(x => x.ProductId == productId, cancellationToken);
    }
}
```

### Contracts Layer

Already defined by user requirements:
- `IInventoryIntegrationService` interface
- `StockCheckResult` DTO
- Method signatures match requirements exactly

## Data Flow

**CheckStock Flow:**
```
ServiceOrder → IInventoryIntegrationService.CheckStockAsync(productId, quantity)
  → Repository.FindByProductIdAsync(productId)
  → Map InventoryItem to StockCheckResult
  → Return result
```

**ReserveStock Flow:**
```
ServiceOrder → IInventoryIntegrationService.ReserveStockAsync(productId, quantity, reservationId)
  → Repository.FindByProductIdAsync(productId)
  → InventoryItem.CanReserve(quantity) check
  → If OK: InventoryItem.Reserve(quantity, reservationId)
  → Repository.UpdateAsync() + UnitOfWork.SaveChangesAsync()
  → Return true/false
```

## Database Schema

**Table:** `AppInventoryItems` (assuming DbTablePrefix = "App")

| Column | Type | Constraints |
|--------|------|-------------|
| Id | uniqueidentifier | PK |
| ProductId | uniqueidentifier | NOT NULL, INDEX |
| ProductCode | nvarchar(50) | NOT NULL |
| ProductName | nvarchar(200) | NOT NULL |
| Quantity | int | NOT NULL, >= 0 |
| ReservedQuantity | int | NOT NULL, >= 0 |
| CreationTime | datetime2 | NOT NULL |
| CreatorId | uniqueidentifier | NULL |
| LastModificationTime | datetime2 | NULL |
| LastModifierId | uniqueidentifier | NULL |
| IsDeleted | bit | NOT NULL, default 0 |
| DeletionTime | datetime2 | NULL |
| DeleterId | uniqueidentifier | NULL |

## Testing Strategy

**Domain Tests:**
- InventoryItem.CanReserve() with various scenarios
- InventoryItem.Reserve() success and failure cases
- Business exception on insufficient stock

**Application Tests:**
- CheckStockAsync with existing/non-existing products
- ReserveStockAsync success/failure scenarios
- Integration with repository mocks

**EF Core Tests:**
- Repository FindByProductIdAsync
- Entity configuration and constraints
- Full integration test with real DbContext

## Migration

User will manually run:
```bash
cd HQSOFT.Inventory/src/HQSOFT.Inventory.EntityFrameworkCore
dotnet ef migrations add AddInventoryItemEntity
dotnet ef database update
```

## Files to Create/Modify

**Domain Layer:**
- `HQSOFT.Inventory.Domain/InventoryItems/InventoryItem.cs` (new)
- `HQSOFT.Inventory.Domain/InventoryItems/IInventoryItemRepository.cs` (new)
- `HQSOFT.Inventory.Domain.Shared/InventoryErrorCodes.cs` (modify - add InsufficientStock)

**Application Layer:**
- `HQSOFT.Inventory.Application.Contracts/Integration/IInventoryIntegrationService.cs` (new)
- `HQSOFT.Inventory.Application.Contracts/Integration/StockCheckResult.cs` (new)
- `HQSOFT.Inventory.Application/Integration/InventoryIntegrationService.cs` (new)

**EF Core Layer:**
- `HQSOFT.Inventory.EntityFrameworkCore/EntityFrameworkCore/IInventoryDbContext.cs` (modify - add DbSet)
- `HQSOFT.Inventory.EntityFrameworkCore/EntityFrameworkCore/InventoryDbContext.cs` (modify - add DbSet)
- `HQSOFT.Inventory.EntityFrameworkCore/EntityFrameworkCore/InventoryDbContextModelCreatingExtensions.cs` (modify - add configuration)
- `HQSOFT.Inventory.EntityFrameworkCore/Repositories/EfCoreInventoryItemRepository.cs` (new)
- `HQSOFT.Inventory.EntityFrameworkCore/InventoryEntityFrameworkCoreModule.cs` (modify - register repository)

**Tests:**
- `HQSOFT.Inventory.Domain.Tests/InventoryItems/InventoryItem_Tests.cs` (new)
- `HQSOFT.Inventory.Application.Tests/Integration/InventoryIntegrationService_Tests.cs` (new)
- `HQSOFT.Inventory.EntityFrameworkCore.Tests/Repositories/InventoryItemRepository_Tests.cs` (new)

## Success Criteria

1. ServiceOrder can check stock availability via `CheckStockAsync`
2. ServiceOrder can reserve stock via `ReserveStockAsync`
3. Reservations fail atomically when insufficient stock
4. All domain rules enforced at entity level
5. Full audit trail via ABP's audit fields
6. Repository pattern properly implemented
7. EF Core configuration follows ABP conventions
8. Unit and integration tests pass
