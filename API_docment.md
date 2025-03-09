
## 系統概述

維修系統 API 是一個基於 ASP.NET Core 開發的後端服務，提供了設備維修管理的完整功能，包括用戶認證、報修單處理、設備管理等。系統採用分層架構設計，遵循關注點分離原則。

## API 端點

### 用戶認證 (AuthController)

| 端點 | 方法 | 描述 | 參數 |
|------|------|------|------|
| `/api/auth/login` | POST | 用戶登錄 | LoginModel (用戶名和密碼) |
| `/api/auth/register` | POST | 註冊新用戶 | User 資料 |
| `/api/auth/user` | GET | 獲取當前用戶信息 | 需要授權標頭 |

### 報修管理 (RepairController)

| 端點 | 方法 | 描述 | 參數 |
|------|------|------|------|
| `/api/repair/tickets` | GET | 獲取所有報修單 | 無 |
| `/api/repair/tickets/{id}` | GET | 獲取特定報修單 | id: 報修單ID |
| `/api/repair/tickets` | POST | 創建新報修單 | RepairTicketCreateModel |
| `/api/repair/tickets/{id}` | PUT | 更新報修單 | id: 報修單ID, RepairTicket 資料 |
| `/api/repair/tickets/{id}` | DELETE | 刪除報修單 | id: 報修單ID |
| `/api/repair/user/{userId}` | GET | 獲取用戶的報修單 | userId: 用戶ID |
| `/api/repair/technician/{techId}` | GET | 獲取技術人員的報修單 | techId: 技術人員ID |

### 設備管理 (EquipmentController)

| 端點 | 方法 | 描述 | 參數 |
|------|------|------|------|
| `/api/equipment` | GET | 獲取所有設備 | 支持分頁和篩選 |
| `/api/equipment/{id}` | GET | 獲取特定設備 | id: 設備ID |
| `/api/equipment` | POST | 添加新設備 | Equipment 資料 |
| `/api/equipment/{id}` | PUT | 更新設備 | id: 設備ID, Equipment 資料 |
| `/api/equipment/{id}` | DELETE | 刪除設備 | id: 設備ID |
| `/api/equipment/types` | GET | 獲取所有設備類型 | 無 |
| `/api/equipment/departments` | GET | 獲取所有部門 | 無 |

## 數據模型

### User (用戶)

```csharp
/// <summary>
/// 用戶實體類，表示系統中的用戶信息
/// </summary>
public class User
{
    /// <summary>
    /// 用戶唯一標識符
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// 用戶登錄名，不可為空
    /// </summary>
    public string Username { get; set; }
    
    /// <summary>
    /// 用戶真實姓名，不可為空
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// 用戶電子郵件地址，可為空
    /// </summary>
    public string? Email { get; set; }
    
    /// <summary>
    /// 用戶角色，默認為"User"
    /// </summary>
    public string Role { get; set; }
    
    /// <summary>
    /// 用戶電話號碼，可為空
    /// </summary>
    public string? Phone { get; set; }
    
    /// <summary>
    /// 用戶所屬部門，可為空
    /// </summary>
    public string? Department { get; set; }
    
    // 其他安全相關屬性
}
```

### RepairTicket (報修單)

```csharp
/// <summary>
/// 報修單實體類，表示維修請求
/// </summary>
public class RepairTicket
{
    /// <summary>
    /// 報修單唯一標識符
    /// </summary>
    public int TicketId { get; set; }
    
    /// <summary>
    /// 報修單標題
    /// </summary>
    public string Title { get; set; }
    
    /// <summary>
    /// 問題描述
    /// </summary>
    public string? Problem { get; set; }
    
    /// <summary>
    /// 設備類型
    /// </summary>
    public string? DeviceType { get; set; }
    
    /// <summary>
    /// 報修單狀態
    /// </summary>
    public string Status { get; set; }
    
    /// <summary>
    /// 解決方案
    /// </summary>
    public string? Solution { get; set; }
    
    /// <summary>
    /// 提交用戶ID
    /// </summary>
    public int UserId { get; set; }
    
    /// <summary>
    /// 處理人員ID
    /// </summary>
    public int? HandlerId { get; set; }
    
    /// <summary>
    /// 設備ID
    /// </summary>
    public int? EquipmentId { get; set; }
    
    /// <summary>
    /// 創建時間
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime UpdatedAt { get; set; }
    
    // 導航屬性
    public User? User { get; set; }
    public User? Handler { get; set; }
    public Equipment? Equipment { get; set; }
}
```

### Equipment (設備)

```csharp
/// <summary>
/// 設備實體類，表示可維修的設備
/// </summary>
public class Equipment
{
    /// <summary>
    /// 設備唯一標識符
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// 設備名稱
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// 設備類型
    /// </summary>
    public string DeviceType { get; set; }
    
    /// <summary>
    /// 設備所屬部門
    /// </summary>
    public string Department { get; set; }
    
    /// <summary>
    /// 設備位置
    /// </summary>
    public string? Location { get; set; }
    
    /// <summary>
    /// 購買日期
    /// </summary>
    public DateTime? PurchaseDate { get; set; }
}
```

## 服務和依賴注入

系統使用依賴注入模式來管理服務的生命週期和依賴關係。主要的服務包括：

### 認證服務 (IAuthService)

負責用戶身份驗證和授權，包括：
- 用戶登錄
- 用戶註冊
- 令牌生成和驗證

### 報修服務 (IRepairService)

處理報修單業務邏輯，包括：
- 創建報修單
- 更新報修單狀態
- 分配技術人員
- 完成維修流程

### 電子郵件服務 (IEmailService)

處理電子郵件通知，包括：
- 發送報修單狀態更新通知
- 發送維修通知
- 發送分配通知

### 文件服務 (IFileService)

處理文件上傳和管理，支持報修單附件。

### 設備服務 (IEquipmentService)

管理設備信息，包括：
- 添加新設備
- 更新設備信息
- 查詢設備

## 數據訪問層

系統使用 Entity Framework Core 作為 ORM 工具，通過 `RepairDbContext` 和 `IRepairRepository` 接口訪問數據庫。主要的數據操作包括：

### 報修單操作

```csharp
Task<IEnumerable<RepairTicket>> GetAllTicketsAsync();
Task<RepairTicket?> GetTicketByIdAsync(int id);
Task<RepairTicket> CreateTicketAsync(RepairTicket ticket);
Task<RepairTicket> UpdateTicketAsync(RepairTicket ticket);
Task<bool> DeleteTicketAsync(int id);
Task<IEnumerable<RepairTicket>> GetUserTicketsAsync(int userId);
Task<IEnumerable<RepairTicket>> GetTechnicianTicketsAsync(int technicianId);
```

### 設備操作

```csharp
Task<IEnumerable<Equipment>> GetAllEquipmentAsync(EquipmentQueryParams queryParams);
Task<Equipment?> GetEquipmentByIdAsync(int id);
Task<Equipment> AddEquipmentAsync(Equipment equipment);
Task<Equipment> UpdateEquipmentAsync(Equipment equipment);
Task<bool> DeleteEquipmentAsync(int id);
Task<int> GetEquipmentTotalCountAsync(EquipmentQueryParams queryParams);
Task<IEnumerable<string>> GetDeviceTypesAsync();
Task<IEnumerable<string>> GetDepartmentsAsync();
```

### 用戶操作

```csharp
Task<User?> GetUserByIdAsync(int id);
Task<User?> GetUserByUsernameAsync(string username);
Task<User?> GetUserByEmailAsync(string email);
Task<IEnumerable<User>> GetAllUsersAsync();
Task<IEnumerable<User>> GetTechniciansAsync();
Task<User> CreateUserAsync(User user);
Task<User> UpdateUserAsync(User user);
```

## 身份驗證與授權

系統使用 JWT (JSON Web Token) 進行身份驗證和授權。配置如下：

```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "DefaultKeyForDevelopment"))
        };
    });
```

## 跨域資源共享 (CORS)

系統配置了兩種 CORS 策略：

1. `AllowAll` - 允許所有來源的請求
2. `AllowSpecificOrigin` - 只允許特定來源 (前端應用) 的請求

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });

    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins("http://localhost:3000")
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials());
});
```

## 總結

維修系統 API 提供了全面的設備維修管理功能，包括用戶管理、報修單處理、設備管理等。系統採用現代化的架構設計和技術棧，具有良好的可擴展性和可維護性。
