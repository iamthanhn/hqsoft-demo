# Sales Order Blazor UI Design

**Date:** 2026-03-26
**Module:** HQSOFT.Order.Blazor
**Requirement:** Section 2.4 - Giao diện Blazor cho Sales Order

## Overview

Xây dựng UI Blazor cho module Sales Order với thiết kế tối giản, sang trọng theo phong cách shadcn/ui (màu sắc nhẹ nhàng, không loè loẹt). Sử dụng ABP Blazor components (Blazorise DataGrid) kết hợp CSS tùy chỉnh.

## UI Components

### 1. Trang Danh Sách (SalesOrders.razor)

**Location:** `HQSOFT.Order.Blazor/Components/Pages/SalesOrders/SalesOrders.razor`

**Features:**
- DataGrid hiển thị danh sách đơn hàng với các cột:
  - OrderNumber (Số đơn hàng)
  - OrderDate (Ngày đặt)
  - Status (Trạng thái: Draft/Confirmed/Cancelled)
  - TotalAmount (Tổng tiền)
  - Actions (Xem chi tiết, Xác nhận, Hủy)

- Filter bar phía trên DataGrid:
  - Text search: tìm theo OrderNumber
  - Dropdown: lọc theo Status
  - Date range picker: FromDate, ToDate

- Toolbar:
  - Nút "Tạo đơn hàng" (primary button, có permission check)

- Pagination: Previous/Next với số trang, hiển thị tổng số records

**Data flow:**
- Gọi `ISalesOrderAppService.GetListAsync(GetSalesOrdersInput)` với filter + paging + sorting
- Refresh sau khi tạo/xác nhận/hủy thành công

### 2. Modal Tạo Đơn Hàng (CreateSalesOrderModal.razor)

**Location:** `HQSOFT.Order.Blazor/Components/Pages/SalesOrders/CreateSalesOrderModal.razor`

**Layout:**
- Header: "Tạo đơn hàng mới"
- Body:
  - OrderDate picker (default: hôm nay)
  - Section "Sản phẩm":
    - Nút "Thêm sản phẩm" → mở ProductLookupModal
    - Table hiển thị line items đã chọn:
      - ProductCode
      - ProductName
      - Quantity (editable)
      - UnitPrice (editable)
      - LineTotal (calculated = Quantity × UnitPrice)
      - Action: nút xóa line
    - Footer row: hiển thị TotalAmount (sum of LineTotals)
- Footer:
  - Nút "Lưu" (primary)
  - Nút "Hủy" (secondary)

**Validation:**
- OrderDate: required
- OrderLines: phải có ít nhất 1 line
- Quantity: > 0
- UnitPrice: > 0

**Data flow:**
- Submit → gọi `ISalesOrderAppService.CreateAsync(CreateSalesOrderDto)`
- Success → đóng modal, refresh danh sách, toast notification
- Error (InsufficientStock) → hiển thị error message với ProductCode

### 3. Modal Chọn Sản Phẩm (ProductLookupModal.razor)

**Location:** `HQSOFT.Order.Blazor/Components/Pages/SalesOrders/ProductLookupModal.razor`

**Features:**
- Search box: filter theo ProductCode hoặc ProductName
- DataGrid hiển thị:
  - ProductCode
  - ProductName
  - AvailableQuantity (Quantity - ReservedQuantity)
  - Action: nút "Chọn"

- Khi chọn sản phẩm:
  - Hiển thị inline form nhập Quantity + UnitPrice
  - Nút "Thêm" → add vào CreateSalesOrderModal, đóng lookup modal

**Data flow:**
- Gọi `IInventoryItemAppService.GetProductListAsync(GetInventoryItemListDto)` với filter + paging
- Return selected product info về CreateSalesOrderModal

### 4. Modal Chi Tiết (SalesOrderDetailModal.razor)

**Location:** `HQSOFT.Order.Blazor/Components/Pages/SalesOrders/SalesOrderDetailModal.razor`

**Layout:**
- Header: "Chi tiết đơn hàng #{OrderNumber}"
- Body (readonly):
  - OrderNumber
  - OrderDate
  - Status (badge với màu: Draft=gray, Confirmed=green, Cancelled=red)
  - Table line items (readonly):
    - ProductCode
    - ProductName
    - Quantity
    - UnitPrice
    - LineTotal
  - TotalAmount (bold, larger font)
- Footer:
  - Nút "Xác nhận" (hiển thị nếu Status = Draft, có permission check)
  - Nút "Hủy" (hiển thị nếu Status = Confirmed, có permission check)
  - Nút "Đóng"

**Data flow:**
- Load: gọi `ISalesOrderAppService.GetAsync(id)`
- Xác nhận/Hủy: mở ConfirmationModal

### 5. Modal Xác Nhận (ConfirmationModal.razor)

**Location:** `HQSOFT.Order.Blazor/Components/Shared/ConfirmationModal.razor`

**Reusable component:**
- Props:
  - Title (string)
  - Message (string)
  - ConfirmText (string, default: "Có")
  - CancelText (string, default: "Không")
  - OnConfirm (EventCallback)

**Usage:**
- Xác nhận đơn hàng: "Bạn có chắc muốn xác nhận đơn hàng này?"
- Hủy đơn hàng: "Bạn có chắc muốn hủy đơn hàng này?"

**Data flow:**
- OnConfirm → gọi `ConfirmAsync(id)` hoặc `CancelAsync(id)`
- Success → đóng modal, refresh danh sách/chi tiết, toast notification

## Styling (shadcn-like)

**CSS file:** `HQSOFT.Order.Blazor/wwwroot/styles/sales-orders.css`

**Design principles:**
- Màu chủ đạo: trắng (#ffffff), xám nhạt (#f8f9fa, #e9ecef)
- Border: mảnh (1px), màu #dee2e6
- Border radius: 6px (bo góc nhẹ)
- Shadow: subtle (0 1px 3px rgba(0,0,0,0.1))
- Typography:
  - Font family: system-ui, -apple-system, sans-serif
  - Font size: 14px (base), 16px (headings)
  - Line height: 1.5
  - Color: #212529 (text), #6c757d (muted)

**Button styles:**
- Primary: background #0d6efd (ABP default), hover darker
- Secondary: border only, background transparent
- Danger: background #dc3545 (for cancel actions)
- Padding: 8px 16px
- Font weight: 500

**Table styles:**
- Header: background #f8f9fa, font weight 600
- Row hover: background #f8f9fa
- Cell padding: 12px
- No zebra striping

**Modal styles:**
- Max width: 800px (create/detail), 600px (lookup)
- Backdrop: rgba(0,0,0,0.5)
- Animation: fade in

**Status badges:**
- Draft: background #6c757d (gray)
- Confirmed: background #198754 (green)
- Cancelled: background #dc3545 (red)
- Padding: 4px 8px, border radius 4px, font size 12px

## Localization

**Files to update:**
- `HQSOFT.Order.Domain.Shared/Localization/Order/en.json`
- `HQSOFT.Order.Domain.Shared/Localization/Order/vi.json`

**New keys:**
```json
{
  "Menu:SalesOrders": "Sales Orders / Đơn bán hàng",
  "SalesOrders": "Sales Orders / Đơn bán hàng",
  "SalesOrders:OrderNumber": "Order Number / Số đơn hàng",
  "SalesOrders:OrderDate": "Order Date / Ngày đặt",
  "SalesOrders:Status": "Status / Trạng thái",
  "SalesOrders:TotalAmount": "Total Amount / Tổng tiền",
  "SalesOrders:Create": "Create Order / Tạo đơn hàng",
  "SalesOrders:Detail": "Order Detail / Chi tiết đơn hàng",
  "SalesOrders:Confirm": "Confirm / Xác nhận",
  "SalesOrders:Cancel": "Cancel / Hủy",
  "SalesOrders:AddProduct": "Add Product / Thêm sản phẩm",
  "SalesOrders:ProductCode": "Product Code / Mã sản phẩm",
  "SalesOrders:ProductName": "Product Name / Tên sản phẩm",
  "SalesOrders:Quantity": "Quantity / Số lượng",
  "SalesOrders:UnitPrice": "Unit Price / Đơn giá",
  "SalesOrders:LineTotal": "Line Total / Thành tiền",
  "SalesOrders:AvailableQuantity": "Available / Tồn kho",
  "SalesOrders:ConfirmMessage": "Are you sure you want to confirm this order? / Bạn có chắc muốn xác nhận đơn hàng này?",
  "SalesOrders:CancelMessage": "Are you sure you want to cancel this order? / Bạn có chắc muốn hủy đơn hàng này?",
  "SalesOrders:CreateSuccess": "Order created successfully / Tạo đơn hàng thành công",
  "SalesOrders:ConfirmSuccess": "Order confirmed successfully / Xác nhận đơn hàng thành công",
  "SalesOrders:CancelSuccess": "Order cancelled successfully / Hủy đơn hàng thành công",
  "SalesOrders:Status:Draft": "Draft / Nháp",
  "SalesOrders:Status:Confirmed": "Confirmed / Đã xác nhận",
  "SalesOrders:Status:Cancelled": "Cancelled / Đã hủy"
}
```

## Menu Integration

**File:** `HQSOFT.Order.Blazor/Menus/OrderMenuContributor.cs`

**Add menu item:**
```csharp
context.Menu.Items.Insert(
    1,
    new ApplicationMenuItem(
        OrderMenus.SalesOrders,
        l["Menu:SalesOrders"],
        "/sales-orders",
        icon: "fas fa-shopping-cart",
        order: 2,
        requiredPermissionName: OrderPermissions.SalesOrders.Default
    )
);
```

## Permissions

Sử dụng permissions đã có:
- `OrderPermissions.SalesOrders.Default` - xem danh sách
- `OrderPermissions.SalesOrders.Create` - tạo đơn hàng
- `OrderPermissions.SalesOrders.Confirm` - xác nhận đơn hàng
- `OrderPermissions.SalesOrders.Cancel` - hủy đơn hàng

## Error Handling

**Business exceptions:**
- `OrderDomainErrorCodes.InsufficientStock` - hiển thị message với ProductCode
- `OrderDomainErrorCodes.ReserveStockFailed` - hiển thị message với ProductCode

**UI error display:**
- Toast notification (ABP UiMessageService)
- Inline validation errors (Blazorise Validation)

## Technical Notes

**Dependencies:**
- Blazorise DataGrid (đã có trong OrderBlazorModule)
- ABP Blazor components (Modal, Button, DatePicker, etc.)
- IInventoryItemAppService (inject từ Inventory module)

**Render mode:**
- InteractiveServer (đã config trong OrderBlazorModule)

**State management:**
- Component state (không cần global state)
- Refresh list sau mỗi mutation

**Performance:**
- Server-side paging (không load toàn bộ data)
- Debounce search input (300ms)

## Implementation Order

1. Localization keys (en.json, vi.json)
2. CSS file (sales-orders.css)
3. ConfirmationModal (reusable)
4. ProductLookupModal
5. CreateSalesOrderModal
6. SalesOrderDetailModal
7. SalesOrders page (main list)
8. Menu integration
9. Testing

## Success Criteria

- [ ] Danh sách đơn hàng hiển thị đúng với paging/sorting/filter
- [ ] Tạo đơn hàng với lookup sản phẩm từ Inventory
- [ ] Xác nhận đơn hàng với modal confirmation
- [ ] Hủy đơn hàng với modal confirmation
- [ ] Xem chi tiết đơn hàng
- [ ] Error handling cho insufficient stock
- [ ] UI style theo shadcn (tối giản, sang trọng)
- [ ] Localization đầy đủ (en + vi)
- [ ] Permission check đúng cho từng action
