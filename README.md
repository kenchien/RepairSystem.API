# 維修系統 API - 使用 Dapper 進行數據訪問

本項目是一個使用 ASP.NET Core 8.0 和 Dapper 構建的設備維修管理系統 API。

## 從 Entity Framework Core 遷移到 Dapper

### 為什麼選擇 Dapper？

1. **性能**：Dapper 是一個高性能的微型 ORM，相比 Entity Framework Core 的重量級特性，Dapper 只專注於高效 SQL 執行和物件映射
2. **控制**：使用 Dapper，我們可以完全控制 SQL 查詢，避免 EF Core 生成的低效查詢
3. **簡潔**：簡化了數據訪問邏輯，沒有複雜的變更跟踪和懶加載機制
4. **學習曲線**：更易於理解和學習

### 已完成的遷移工作

1. **添加 Dapper 依賴**：
   - 安裝了 Dapper 和 MySql.Data 包
   - 移除了 EF Core 的直接使用

2. **添加資料庫連線基礎設施**：
   - 創建了 DapperContext 類，負責建立資料庫連線
   - 配置了連線字串

3. **實現 Dapper 版本的儲存庫**：
   - 使用 Dapper 實現了 IRepairRepository 接口
   - 直接使用 SQL 查詢替代了 EF Core 的 LINQ 查詢
   - 手動處理物件關聯

4. **更新依賴注入配置**：
   - 在 Program.cs 中註冊了 DapperContext
   - 將 RepairRepository 替換為 DapperRepairRepository

### 效能優化

1. **連線池**：利用 ADO.NET 內建的連線池機制
2. **參數化查詢**：防止 SQL 注入並提高查詢性能
3. **多結果集映射**：用單一查詢獲取父子關係數據
4. **批量操作**：使用 Dapper 的批量操作功能

## 使用說明

### 配置數據庫連線

在 `appsettings.json` 中配置數據庫連線字串：

```json
{
  "ConnectionStrings": {
    "DapperConnection": "Server=localhost;Database=RepairSystem;User=root;Password=yourpassword;"
  },
  ...
}
```

### 數據庫初始化

使用 `mysql_db.sql` 腳本初始化數據庫結構和基本數據：

```bash
mysql -u username -p < mysql_db.sql
```

### 主要 API 端點

- **認證**：`/api/auth/login`、`/api/auth/register`
- **報修單**：`/api/repair`
- **設備**：`/api/equipment`

完整 API 文檔可通過 Swagger UI 訪問：`/api-docs`

## 下一步改進

1. **添加更多單元測試**：覆蓋所有 Repository 方法
2. **添加快取機制**：使用 Redis 或 Memory Cache 優化高頻查詢
3. **實現事務管理**：確保多步操作的原子性
4. **遷移其他存儲庫**：將所有數據訪問層從 EF Core 遷移到 Dapper 