# Bài Kiểm Tra Kỹ Thuật: Junior ABP .NET Developer

## 🎯 Mục tiêu Bài Kiểm Tra

Ứng viên sẽ xây dựng một **module đơn hàng bán (Sales Order)** đơn giản trên **ABP Framework 9 + Blazor**, tích hợp với module **Inventory** (giả định) để kiểm tra tồn kho. Bài kiểm tra nhằm đánh giá khả năng:

- Tự triển khai một chức năng CRUD hoàn chỉnh
- Sử dụng các tính năng có sẵn của ABP Framework
- Đọc hiểu và tích hợp với module có sẵn (Inventory)
- Tùy chỉnh module Identity (mở rộng AppUser)
- Áp dụng kiến trúc đa tầng (Multi-layered Architecture)

---

## 📋 Yêu Cầu Cụ Thể

### Phần 1: Tùy Chỉnh Module Identity (30 phút)

**Yêu cầu:**
- Mở rộng `AppUser` entity bằng cách thêm property `Department` (string, max 100 ký tự)
- Tạo migration cho thay đổi này
- Thêm trường `Department` vào trang quản lý người dùng (Identity/Users) trong giao diện Blazor
- Cho phép chỉnh sửa `Department` khi tạo/cập nhật người dùng

**Công nghệ:**
- ABP.IO Framework 9
- Blazor UI
- Entity Framework Core

---

### Phần 2: Xây Dựng Module Sales Order (3-4 giờ)

#### 2.1. Tạo Entity `SalesOrder`
```csharp
public class SalesOrder : FullAuditedAggregateRoot<Guid>
{
    public string OrderNumber { get; set; } // Format: SO-YYYYMMDD-001
    public Guid CustomerId { get; set; }
    public Guid? SalesPersonId { get; set; } // Liên kết với AppUser (Department)
    public DateTime OrderDate { get; set; }
    public SalesOrderStatus Status { get; set; } // Draft, Confirmed, Cancelled
    public decimal TotalAmount { get; private set; }
    
    // Navigation properties
    public virtual ICollection<SalesOrderLine> OrderLines { get; set; }
}

public class SalesOrderLine : Entity<Guid>
{
    public Guid SalesOrderId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductCode { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; private set; }
    
    public virtual SalesOrder SalesOrder { get; set; }
}

public enum SalesOrderStatus
{
    Draft,
    Confirmed,
    Cancelled
}
```

#### 2.2. Tích Hợp với Module Inventory (Giả định)
- Module Inventory đã có sẵn interface:
```csharp
public interface IInventoryIntegrationService
{
    Task<StockCheckResult> CheckStockAsync(Guid productId, int requestedQuantity);
    Task<bool> ReserveStockAsync(Guid productId, int quantity, string reservationId);
}

public class StockCheckResult
{
    public bool IsAvailable { get; set; }
    public int AvailableQuantity { get; set; }
    public string ProductCode { get; set; }
    public string ProductName { get; set; }
}
```

**Yêu cầu tích hợp:**
1. Khi tạo đơn hàng, gọi `IInventoryIntegrationService.CheckStockAsync()` để kiểm tra tồn kho
2. Nếu tồn kho đủ, gọi `IInventoryIntegrationService.ReserveStockAsync()` để tạm giữ hàng
3. Nếu tồn kho không đủ, hiển thị thông báo lỗi
4. Khi hủy đơn hàng, giải phóng tồn kho đã giữ (cần thêm phương thức `ReleaseStockAsync`)

#### 2.3. Business Logic
1. **Tạo đơn hàng:**
   - Tự động sinh `OrderNumber` theo format
   - Tính toán `TotalAmount` từ các dòng đơn hàng
   - Mặc định `Status = Draft`
   - Kiểm tra tồn kho trước khi xác nhận đơn hàng

2. **Xác nhận đơn hàng:**
   - Chỉ có `SalesOrder` với `Status = Draft` mới được xác nhận
   - Khi xác nhận, cập nhật `Status = Confirmed`
   - Gọi service tạm giữ tồn kho

3. **Hủy đơn hàng:**
   - Chỉ hủy đơn hàng đã xác nhận
   - Giải phóng tồn kho đã giữ

#### 2.4. Giao Diện Blazor
- Trang danh sách đơn hàng (có phân trang, sắp xếp, tìm kiếm)
- Trang tạo đơn hàng với form nhập liệu
- Trang chi tiết đơn hàng
- Nút xác nhận/hủy đơn hàng với xác nhận

#### 2.5. API Endpoints
- `GET /api/app/sales-orders` (có filter theo trạng thái, ngày)
- `GET /api/app/sales-orders/{id}`
- `POST /api/app/sales-orders`
- `PUT /api/app/sales-orders/{id}/confirm`
- `PUT /api/app/sales-orders/{id}/cancel`

---

### Phần 3: Sử Dụng Tính Năng ABP (Bắt buộc)

1. **Validation:**
   - Sử dụng Data Annotation hoặc FluentValidation cho DTOs
   - Custom validation: kiểm tra số lượng đặt hàng > 0

2. **Permission:**
   - Tạo permissions: `SalesOrder.Create`, `SalesOrder.Confirm`, `SalesOrder.Cancel`
   - Phân quyền cho các action trong AppService và UI

3. **Localization:**
   - Sử dụng resource files cho tất cả message hiển thị
   - Hỗ trợ ít nhất 2 ngôn ngữ (vi, en)

4. **Background Job:**
   - Tạo background job để xử lý tự động hủy đơn hàng Draft sau 24 giờ

5. **Event Bus:**
   - Publish event khi đơn hàng được xác nhận: `SalesOrderConfirmedEvent`
   - Xử lý event để gửi email thông báo (có thể mock)

---

## ✅ Tiêu Chí Đánh Giá Kết Quả

### Bắt Buộc
- [ ] **Code có thể build và chạy được** - Ứng viên phải demo được chức năng tạo/xác nhận đơn hàng với kiểm tra tồn kho
- [ ] **Tuân thủ ABP framework conventions** - Đúng cấu trúc thư mục, naming conventions, dependency injection
- [ ] **Tích hợp cross-module hoạt động** - Gọi được `IInventoryIntegrationService` và xử lý kết quả
- [ ] **Sử dụng đúng integration pattern** - Không duplicate business logic, chỉ delegate qua interface

### Khuyến Khích (Điểm Cộng)
- [ ] **Có unit tests cho business logic** - Test cho `SalesOrderManager` hoặc `SalesOrderAppService`
- [ ] **API endpoints được document đầy đủ** - Sử dụng Swagger/OpenAPI với XML comments
- [ ] **Ứng dụng AI để speed up** - Ghi chú rõ đã sử dụng AI tool nào (Cursor/Copilot), model nào, prompt nào
- [ ] **Clean code và separation of concerns** - Code dễ đọc, đúng kiến trúc đa tầng

### Yêu Cầu Kỹ Thuật Cụ Thể
- [ ] Entity kế thừa đúng base class của ABP
- [ ] DTO sử dụng đúng annotation (Serializable, DataContract)
- [ ] AppService kế thừa từ `ApplicationService` và implement đầy đủ CRUD
- [ ] Sử dụng `ObjectMapper` cho entity-DTO conversion
- [ ] Repository pattern đúng cách (không viết query trực tiếp trong AppService)
- [ ] Xử lý exception với `BusinessException` và user-friendly messages
- [ ] UI sử dụng các component của ABP Blazor (DataGrid, Modal, Notification)

---

## 🚀 Hướng Dẫn Nộp Bài

1. **Fork repository** mẫu từ: [Link đến GitHub Template]
2. **Triển khai code** theo yêu cầu
3. **Tạo file README.md** bao gồm:
   - Hướng dẫn chạy project
   - Giải thích các quyết định thiết kế
   - Mô tả cách tích hợp với Inventory module
   - Ghi chú về AI tools đã sử dụng (nếu có)
4. **Quay video demo ngắn** (3-5 phút) thể hiện:
   - Tạo user với Department
   - Tạo đơn hàng với kiểm tra tồn kho
   - Xác nhận/hủy đơn hàng
   - Hiển thị lịch sử đơn hàng
5. **Gửi link repository** và **link video demo** qua email

---

## 🧪 Môi Trường Phát Triển

- **.NET SDK**: 8.0+
- **IDE**: Visual Studio 2022+ hoặc VS Code với C# Dev Kit
- **Database**: SQL Server LocalDB hoặc SQLite
- **ABP CLI**: Version 9.x
- **Thời gian làm bài**: Tối đa 5 giờ

---

## 📚 Tài Nguyên Tham Khảo

1. [ABP Framework Documentation](https://docs.abp.io/)
2. [ABP Commercial: Module Development](https://docs.abp.io/en/commercial/latest/module-development/index)
3. [Sample Module: BookStore](https://github.com/abpframework/abp-samples/tree/master/BookStore)
4. [Blazor UI Documentation](https://docs.abp.io/en/abp/latest/UI/Blazor/Overall)

---

**Lưu ý:** Bài kiểm tra này được thiết kế để đánh giá khả năng tự học, áp dụng framework, và giải quyết vấn đề thực tế. Ứng viên được khuyến khích sử dụng AI tools để tăng tốc độ phát triển, nhưng phải hiểu và có thể giải thích code mà mình tạo ra.

**Chúc bạn thành công!** 🎯