# HQSOFT Demo - Sales Order Management System

A multi-module enterprise application built with **ABP Framework 9** and **Blazor**, featuring a complete Sales Order module integrated with an Inventory module for real-time stock checking.

---

## Architecture Overview

### Module Structure

```
hqsoft-demo/
├── HQSOFT.Order/                 # Sales Order module (main focus)
│   ├── src/
│   │   ├── HQSOFT.Order.Domain/              # Entities, Domain Services, Repositories
│   │   ├── HQSOFT.Order.Domain.Shared/       # Constants, Enums, Localization resources
│   │   ├── HQSOFT.Order.Application/         # AppServices, Event Handlers, Background Jobs
│   │   ├── HQSOFT.Order.Application.Contracts/  # DTOs, Interfaces, Permissions
│   │   ├── HQSOFT.Order.EntityFrameworkCore/ # EF Core DbContext, Migrations, Configurations
│   │   ├── HQSOFT.Order.HttpApi/             # API Controllers
│   │   ├── HQSOFT.Order.Blazor/              # Blazor UI (pages, components)
│   │   └── HQSOFT.Order.DbMigrator/          # Database migration tool
│   └── test/
│       └── (Unit & Integration test projects)
└── HQSOFT.Inventory/              # Inventory module (sister module)
    └── src/
        ├── HQSOFT.Inventory.Application.Contracts/Integration/
        │   └── IInventoryIntegrationService.cs   # Cross-module integration interface
        └── HQSOFT.Inventory.Application/Integration/
            └── InventoryIntegrationService.cs     # Implementation (mock/real)
```

### Solution

- `HQSOFT.All.sln` — full solution containing both modules
- `HQSOFT.Order/HQSOFT.Order.sln` — Order module standalone
- `HQSOFT.Inventory/HQSOFT.Inventory.sln` — Inventory module standalone

---

## Technology Stack

| Layer | Technology |
|---|---|
| Framework | ABP Framework 9 (.NET 9) |
| UI | Blazor Server (Radzen Blazor) |
| ORM | Entity Framework Core 9 + PostgreSQL 15 |
| Background Jobs | ABP Background Jobs |
| Distributed Events | ABP Event Bus |
| Authentication | OpenIddict |
| Containerization | Docker & Docker Compose |

---

## Key Features

### 1. Sales Order Management

- **Auto-generated Order Number** — format `SO-YYYYMMDD-NNN` (e.g. `SO-20260327-001`)
- **Full CRUD** — Create, Read, Confirm, Cancel orders
- **Order Lines** — Add multiple product lines with quantity and unit price
- **Status Workflow** — Draft → Confirmed → Cancelled (enforced business rules)
- **Total Amount** — automatically calculated from order lines

### 2. Inventory Integration (Cross-Module)

The Order module integrates with the Inventory module through a clean interface:

```csharp
public interface IInventoryIntegrationService
{
    Task<StockCheckResult> CheckStockAsync(Guid productId, int requestedQuantity);
    Task<bool> ReserveStockAsync(Guid productId, int quantity, string reservationId);
    Task<bool> ReleaseStockAsync(Guid productId, int quantity, string reservationId);
}
```

**Integration flow:**

```
Create Order → Check stock for all products → If insufficient → throw BusinessException
                                        → If sufficient → Save order

Confirm Order → Check stock again → Reserve stock per line → Publish SalesOrderConfirmedEvent

Cancel Order → Release stock for all lines → Update status to Cancelled
```

### 3. Distributed Event Bus

When an order is confirmed, `SalesOrderConfirmedEto` is published:

```csharp
public class SalesOrderConfirmedEto
{
    public Guid OrderId { get; set; }
    public string OrderNumber { get; set; }
    public DateTime OrderDate { get; set; }
    public string Status { get; set; }
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; }
}
```

Event handler (`SalesOrderConfirmedEventHandler`) currently logs to console (mock email notification).

### 4. Background Jobs

`AutoCancelDraftSalesOrderJob` automatically cancels draft orders after 24 hours:

```csharp
await _backgroundJobManager.EnqueueAsync(
    new AutoCancelDraftSalesOrderArgs { SalesOrderId = order.Id },
    delay: TimeSpan.FromHours(24));
```

### 5. Permissions

Three granular permissions are defined in `OrderPermissions`:

| Permission | Purpose |
|---|---|
| `Order.SalesOrders.Default` | View sales order list |
| `Order.SalesOrders.Create` | Create new orders |
| `Order.SalesOrders.Confirm` | Confirm draft orders |
| `Order.SalesOrders.Cancel` | Cancel confirmed orders |

---

## API Endpoints

| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/api/app/sales-orders` | List orders (filterable by status, date range, order number) |
| `GET` | `/api/app/sales-orders/{id}` | Get single order with all lines |
| `POST` | `/api/app/sales-orders` | Create new order |
| `PUT` | `/api/app/sales-orders/{id}/confirm` | Confirm a draft order |
| `PUT` | `/api/app/sales-orders/{id}/cancel` | Cancel a confirmed order |

---

## Design Decisions

### Why Aggregate Root + Domain Service?

`SalesOrder` extends `FullAuditedAggregateRoot<Guid>`, making it the aggregate root that encapsulates all order lines. The `SalesOrderManager` (Domain Service) handles cross-entity operations like order number generation and status transitions. This follows DDD patterns — entities control their own invariants while the domain service orchestrates operations that require repository access.

### Why Cross-Module Integration via Interface?

Instead of a direct project reference from Order to Inventory (which creates tight coupling), the integration goes through `IInventoryIntegrationService` in the `Application.Contracts` layer. This means:

- **Loose coupling** — modules can be deployed independently
- **Testability** — can mock `IInventoryIntegrationService` in unit tests
- **Flexibility** — Inventory module can be swapped with any stock management system

### Why Value Object for Money?

`Money` is a value object that encapsulates amount and currency:

```csharp
public record Money(decimal Amount, string Currency = "VND")
{
    public static Money operator +(Money a, Money b) => new(a.Amount + b.Amount, a.Currency);
    public static Money operator *(Money price, int qty) => new(price.Amount * qty, price.Currency);
}
```

This ensures currency consistency and prevents bugs from mixing currencies across lines.

---

## Getting Started

### Prerequisites

- .NET 9 SDK
- Docker Desktop
- Visual Studio 2022+ or VS Code with C# Dev Kit

### Option 1: Run with Docker Compose (Recommended)

```bash
# Start all services (PostgreSQL + API + Blazor + DbMigrator)
docker compose up -d

# Check logs
docker compose logs -f
```

Services will be available at:

| Service | URL |
|---|---|
| Blazor UI | http://localhost:8081 |
| API | http://localhost:8080 |
| Swagger | http://localhost:8080/swagger |
| PostgreSQL (Order) | localhost:5433 |
| PostgreSQL (Inventory) | localhost:5434 |

### Option 2: Run Locally

```bash
# 1. Start PostgreSQL containers
docker compose up -d postgres-order postgres-inventory

# 2. Update connection string in appsettings.json
# ConnectionStrings__Default=Server=localhost;Port=5433;Database=Order;User Id=root;Password=1q2w3E

# 3. Run DbMigrator to create database
dotnet run --project HQSOFT.Order/src/HQSOFT.Order.DbMigrator

# 4. Run API
dotnet run --project HQSOFT.Order/src/HQSOFT.Order.HttpApi

# 5. Run Blazor (in another terminal)
dotnet run --project HQSOFT.Order/src/HQSOFT.Order.Blazor
```

### Default Credentials

```
Username: admin
Password: 1q2w3E*
```

---

## Project Structure Deep Dive

### Domain Layer

| File | Purpose |
|---|---|
| `SalesOrder.cs` | Aggregate root — manages order lines, status, total calculation |
| `SalesOrderLine.cs` | Child entity — product line with quantity, price, and line total |
| `Money.cs` | Value object — immutable monetary amount with currency |
| `ISalesOrderRepository.cs` | Repository interface with custom `FindWithLinesAsync` |
| `SalesOrderManager.cs` | Domain service — order creation, confirmation, cancellation logic |

### Application Layer

| File | Purpose |
|---|---|
| `SalesOrderAppService.cs` | Main application service — orchestrates business operations, integrates with Inventory |
| `SalesOrderConfirmedEto.cs` | Distributed event data (published on confirm) |
| `SalesOrderConfirmedEventHandler.cs` | Event handler — mock email notification |
| `AutoCancelDraftSalesOrderJob.cs` | Background job — auto-cancels stale drafts |

### Application.Contracts Layer

| File | Purpose |
|---|---|
| `ISalesOrderAppService.cs` | Public API interface |
| `SalesOrderDto.cs` | Data transfer object for the API |
| `CreateSalesOrderDto.cs` | Input DTO for creating orders |
| `GetSalesOrdersInput.cs` | Filter/input DTO for list queries |
| `OrderPermissions.cs` | Permission constants |
| `DecimalRangeAttribute.cs` | Custom validation attribute for decimal ranges |

### Blazor UI

| File | Purpose |
|---|---|
| `SalesOrders.razor` | Main list page — search, filter, pagination, confirm/cancel actions |
| `CreateSalesOrderModal.razor` | Create order modal with product lookup |
| `ProductLookupModal.razor` | Product search and selection modal |
| `SalesOrderDetailModal.razor` | Order detail view |
| `sales-orders.css` | Custom styles for order badges and amounts |

---

## Database Schema

### SalesOrder Table

| Column | Type | Notes |
|---|---|---|
| `Id` | `uuid` | Primary key |
| `OrderNumber` | `varchar(20)` | Unique, auto-generated |
| `OrderDate` | `date` | Order creation date |
| `Status` | `int` | 0=Draft, 1=Confirmed, 2=Cancelled |
| `TotalAmount` | `decimal(18,2)` | Denormalized total |
| `Currency` | `varchar(5)` | Default "VND" |

### SalesOrderLine Table

| Column | Type | Notes |
|---|---|---|
| `Id` | `uuid` | Primary key |
| `OrderId` | `uuid` | FK to SalesOrder |
| `ProductId` | `uuid` | Reference to Inventory product |
| `ProductCode` | `varchar(20)` | Product code |
| `ProductName` | `varchar(100)` | Product name |
| `Quantity` | `int` | Order quantity |
| `UnitPrice_Amount` | `decimal(18,2)` | Unit price |
| `UnitPrice_Currency` | `varchar(5)` | Currency |
| `LineTotal_Amount` | `decimal(18,2)` | Computed total |
| `LineTotal_Currency` | `varchar(5)` | Currency |

---

## Localization

The application supports multiple languages through ABP's localization system. All user-facing strings are externalized to `OrderResource`:

```
VI: Mã đơn hàng, Ngày đặt, Trạng thái, Tổng tiền, Xác nhận, Hủy
EN: Order Number, Order Date, Status, Total Amount, Confirm, Cancel
```

---

## AI Tools Used

This project was developed with assistance from:

- **Cursor IDE** — primary AI coding assistant
- **Claude 4 Opus** — architecture design and code generation
- **ABP Framework CLI** — scaffolding module structure

---

## References

- [ABP Framework Documentation](https://docs.abp.io/)
- [ABP Commercial: Module Development](https://docs.abp.io/en/commercial/latest/module-development/index)
- [Blazor UI Documentation](https://docs.abp.io/en/abp/latest/UI/Blazor/Overall)
- [Sample: BookStore ABP Module](https://github.com/abpframework/abp-samples/tree/master/BookStore)
