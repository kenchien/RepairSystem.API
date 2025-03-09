@echo off
echo 創建測試輔助類...

echo 創建 MockDapperContext 類...
if not exist RepairSystem.Tests\Helpers mkdir RepairSystem.Tests\Helpers
(
echo using System;
echo using System.Data;
echo using Moq;
echo using RepairSystem.API.Data;
echo 
echo namespace RepairSystem.Tests.Helpers
echo {
echo     /// ^<summary^>
echo     /// 用於測試的 DapperContext 模擬類
echo     /// ^</summary^>
echo     public class MockDapperContext 
echo     {
echo         private readonly Mock^<DapperContext^> _mockContext;
echo         private readonly Mock^<IDbConnection^> _mockConnection;
echo         private readonly Mock^<IDbTransaction^> _mockTransaction;
echo 
echo         /// ^<summary^>
echo         /// 建立一個新的模擬 DapperContext
echo         /// ^</summary^>
echo         public MockDapperContext^(^)
echo         {
echo             _mockContext = new Mock^<DapperContext^>^(null^);
echo             _mockConnection = new Mock^<IDbConnection^>^(^);
echo             _mockTransaction = new Mock^<IDbTransaction^>^(^);
echo 
echo             // 設置連接返回事務
echo             _mockConnection
echo                 .Setup^(c =^> c.BeginTransaction^(It.IsAny^<IsolationLevel^>^(^)^)^)
echo                 .Returns^(_mockTransaction.Object^);
echo 
echo             // 設置上下文創建連接
echo             _mockContext
echo                 .Setup^(c =^> c.CreateConnection^(^)^)
echo                 .Returns^(_mockConnection.Object^);
echo         }
echo 
echo         /// ^<summary^>
echo         /// 獲取模擬的 DapperContext
echo         /// ^</summary^>
echo         public DapperContext Object =^> _mockContext.Object;
echo 
echo         /// ^<summary^>
echo         /// 獲取模擬的數據庫連接
echo         /// ^</summary^>
echo         public Mock^<IDbConnection^> MockConnection =^> _mockConnection;
echo 
echo         /// ^<summary^>
echo         /// 獲取模擬的事務
echo         /// ^</summary^>
echo         public Mock^<IDbTransaction^> MockTransaction =^> _mockTransaction;
echo     }
echo } 
) > RepairSystem.Tests\Helpers\MockDapperContext.cs

echo 創建測試數據生成器類...
(
echo using System;
echo using System.Collections.Generic;
echo using RepairSystem.API.Models;
echo 
echo namespace RepairSystem.Tests.Helpers
echo {
echo     /// ^<summary^>
echo     /// 測試數據生成器
echo     /// ^</summary^>
echo     public static class TestDataGenerator
echo     {
echo         /// ^<summary^>
echo         /// 生成測試用的報修單數據
echo         /// ^</summary^>
echo         public static List^<RepairTicket^> GenerateTestTickets^(int count = 5^)
echo         {
echo             var tickets = new List^<RepairTicket^>^(^);
echo             for ^(int i = 1; i ^<= count; i++^)
echo             {
echo                 tickets.Add^(new RepairTicket
echo                 {
echo                     Id = i,
echo                     Title = $"測試報修單 {i}",
echo                     Description = $"這是測試報修單描述 {i}",
echo                     Status = i % 3 == 0 ? "已完成" : ^(i % 2 == 0 ? "處理中" : "未處理"^),
echo                     Priority = i % 3 == 0 ? "低" : ^(i % 2 == 0 ? "中" : "高"^),
echo                     CreatedAt = DateTime.Now.AddDays^(-i^),
echo                     UpdatedAt = DateTime.Now.AddHours^(-i^),
echo                     UserId = i,
echo                     HandledBy = i % 2 == 0 ? i + 10 : null
echo                 }^);
echo             }
echo             return tickets;
echo         }
echo         
echo         /// ^<summary^>
echo         /// 生成測試用的設備數據
echo         /// ^</summary^>
echo         public static List^<Equipment^> GenerateTestEquipment^(int count = 5^)
echo         {
echo             var equipment = new List^<Equipment^>^(^);
echo             for ^(int i = 1; i ^<= count; i++^)
echo             {
echo                 equipment.Add^(new Equipment
echo                 {
echo                     EquipmentId = i,
echo                     Name = $"測試設備 {i}",
echo                     DeviceType = i % 3 == 0 ? "電腦" : ^(i % 2 == 0 ? "打印機" : "伺服器"^),
echo                     SerialNumber = $"SN{i:D6}",
echo                     Status = i % 2 == 0 ? "正常" : "停用",
echo                     Department = i % 3 == 0 ? "行政部" : ^(i % 2 == 0 ? "技術部" : "財務部"^),
echo                     Location = $"辦公室 {i:D3}",
echo                     PurchaseDate = DateTime.Now.AddYears^(-i^),
echo                     LastMaintenanceDate = DateTime.Now.AddMonths^(-i^)
echo                 }^);
echo             }
echo             return equipment;
echo         }
echo         
echo         /// ^<summary^>
echo         /// 生成測試用的用戶數據
echo         /// ^</summary^>
echo         public static List^<User^> GenerateTestUsers^(int count = 5^)
echo         {
echo             var users = new List^<User^>^(^);
echo             for ^(int i = 1; i ^<= count; i++^)
echo             {
echo                 users.Add^(new User
echo                 {
echo                     Id = i,
echo                     Username = $"user{i}",
echo                     Name = $"測試用戶 {i}",
echo                     Email = $"user{i}@example.com",
echo                     Role = i % 3 == 0 ? "Admin" : ^(i % 2 == 0 ? "Technician" : "User"^),
echo                     Phone = $"1234567{i:D3}",
echo                     Department = i % 3 == 0 ? "行政部" : ^(i % 2 == 0 ? "技術部" : "財務部"^)
echo                 }^);
echo             }
echo             return users;
echo         }
echo         
echo         /// ^<summary^>
echo         /// 生成測試用的附件數據
echo         /// ^</summary^>
echo         public static List^<AttachmentFile^> GenerateTestAttachments^(int ticketId, int count = 2^)
echo         {
echo             var attachments = new List^<AttachmentFile^>^(^);
echo             for ^(int i = 1; i ^<= count; i++^)
echo             {
echo                 attachments.Add^(new AttachmentFile
echo                 {
echo                     Id = i + ^(ticketId * 10^),
echo                     FileName = $"file{i}.txt",
echo                     FilePath = $"/uploads/ticket_{ticketId}/file{i}.txt",
echo                     FileSize = i * 1024,
echo                     TicketId = ticketId,
echo                     UploadTime = DateTime.Now.AddHours^(-i^),
echo                     UploadedBy = ticketId
echo                 }^);
echo             }
echo             return attachments;
echo         }
echo     }
echo } 
) > RepairSystem.Tests\Helpers\TestDataGenerator.cs

echo 測試輔助類創建完成。
echo 請確保 full-repair.cmd 已經執行過，然後重新構建測試專案。
pause 