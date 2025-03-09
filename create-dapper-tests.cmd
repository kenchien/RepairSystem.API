@echo off
echo 創建 DapperRepairRepository 測試...

if not exist RepairSystem.Tests\Data mkdir RepairSystem.Tests\Data

echo 創建 DapperRepairRepositoryTests...
(
echo using Dapper;
echo using Microsoft.Extensions.Logging;
echo using Moq;
echo using RepairSystem.API.Data;
echo using RepairSystem.API.Models;
echo using RepairSystem.Tests.Helpers;
echo using System.Collections.Generic;
echo using System.Data;
echo using System.Linq;
echo using System.Threading.Tasks;
echo using Xunit;
echo 
echo namespace RepairSystem.Tests.Data
echo {
echo     /// ^<summary^>
echo     /// DapperRepairRepository 單元測試
echo     /// ^</summary^>
echo     public class DapperRepairRepositoryTests
echo     {
echo         private readonly Mock^<ILogger^<DapperRepairRepository^>^> _mockLogger;
echo 
echo         public DapperRepairRepositoryTests^(^)
echo         {
echo             _mockLogger = new Mock^<ILogger^<DapperRepairRepository^>^>^(^);
echo         }
echo 
echo         [Fact]
echo         public async Task GetAllTicketsAsync_ShouldReturnAllTickets^(^)
echo         {
echo             // Arrange
echo             var mockContext = new MockDapperContext^(^);
echo 
echo             var expectedTickets = TestDataGenerator.GenerateTestTickets^(^);
echo 
echo             mockContext.MockConnection
echo                 .Setup^(c =^> c.QueryAsync^<RepairTicket, User, User, Equipment, RepairTicket^>^(
echo                     It.IsAny^<string^>^(^), 
echo                     It.IsAny^<Func^<RepairTicket, User, User, Equipment, RepairTicket^>^>^(^), 
echo                     It.IsAny^<object^>^(^), 
echo                     It.IsAny^<IDbTransaction^>^(^), 
echo                     It.IsAny^<bool^>^(^), 
echo                     It.IsAny^<string^>^(^), 
echo                     It.IsAny^<int?^>^(^), 
echo                     It.IsAny^<CommandType?^>^(^)^)^)
echo                 .ReturnsAsync^(expectedTickets^);
echo 
echo             var repository = new DapperRepairRepository^(mockContext.Object, _mockLogger.Object^);
echo 
echo             // Act
echo             var result = await repository.GetAllTicketsAsync^(^);
echo 
echo             // Assert
echo             Assert.NotNull^(result^);
echo             Assert.Equal^(expectedTickets.Count, result.Count^(^)^);
echo         }
echo 
echo         [Fact]
echo         public void SampleTest^(^)
echo         {
echo             // 簡單測試以確保測試框架正常運作
echo             Assert.True^(true^);
echo         }
echo     }
echo }
) > RepairSystem.Tests\Data\DapperRepairRepositoryTests.cs

echo 創建 EquipmentRepositoryTests...
(
echo using Dapper;
echo using Microsoft.Extensions.Logging;
echo using Moq;
echo using RepairSystem.API.Data;
echo using RepairSystem.API.Models;
echo using RepairSystem.Tests.Helpers;
echo using System.Collections.Generic;
echo using System.Data;
echo using System.Linq;
echo using System.Threading.Tasks;
echo using Xunit;
echo 
echo namespace RepairSystem.Tests.Data
echo {
echo     /// ^<summary^>
echo     /// 設備倉儲測試
echo     /// ^</summary^>
echo     public class EquipmentRepositoryTests
echo     {
echo         private readonly Mock^<ILogger^<DapperRepairRepository^>^> _mockLogger;
echo 
echo         public EquipmentRepositoryTests^(^)
echo         {
echo             _mockLogger = new Mock^<ILogger^<DapperRepairRepository^>^>^(^);
echo         }
echo 
echo         [Fact]
echo         public void SampleTest^(^)
echo         {
echo             // 簡單測試以確保測試框架正常運作
echo             Assert.True^(true^);
echo         }
echo     }
echo }
) > RepairSystem.Tests\Data\EquipmentRepositoryTests.cs

echo 創建 UserRepositoryTests...
(
echo using Dapper;
echo using Microsoft.Extensions.Logging;
echo using Moq;
echo using RepairSystem.API.Data;
echo using RepairSystem.API.Models;
echo using RepairSystem.Tests.Helpers;
echo using System.Collections.Generic;
echo using System.Data;
echo using System.Linq;
echo using System.Threading.Tasks;
echo using Xunit;
echo 
echo namespace RepairSystem.Tests.Data
echo {
echo     /// ^<summary^>
echo     /// 用戶倉儲測試
echo     /// ^</summary^>
echo     public class UserRepositoryTests
echo     {
echo         private readonly Mock^<ILogger^<DapperRepairRepository^>^> _mockLogger;
echo 
echo         public UserRepositoryTests^(^)
echo         {
echo             _mockLogger = new Mock^<ILogger^<DapperRepairRepository^>^>^(^);
echo         }
echo 
echo         [Fact]
echo         public void SampleTest^(^)
echo         {
echo             // 簡單測試以確保測試框架正常運作
echo             Assert.True^(true^);
echo         }
echo     }
echo }
) > RepairSystem.Tests\Data\UserRepositoryTests.cs

echo 測試用例創建完成。
echo 請執行 dotnet test RepairSystem.Tests\RepairSystem.Tests.csproj 來運行測試。
pause 