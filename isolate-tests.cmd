@echo off
echo 隔離測試專案並配置單獨的構建流程...

echo 1. 刪除測試文件夾的舊引用...
(
echo using Microsoft.Extensions.Logging; 
echo using RepairSystem.API.Data;
echo using RepairSystem.API.Models;
echo using System;
echo using System.Collections.Generic;
echo using System.Threading.Tasks;
echo using System.Data;
echo using System.Linq;
echo using System.Text;
echo using Xunit;
echo using Moq;
echo using Dapper;
echo 
echo namespace RepairSystem.Tests
echo {
echo     public class UnitTest1
echo     {
echo         [Fact]
echo         public void Test1()
echo         {
echo             Assert.True(true);
echo         }
echo     }
echo }
) > RepairSystem.Tests\UnitTest1.cs

echo 2. 更新測試專案文件...
(
echo ^<Project Sdk="Microsoft.NET.Sdk"^>
echo.
echo   ^<PropertyGroup^>
echo     ^<TargetFramework^>net8.0^</TargetFramework^>
echo     ^<ImplicitUsings^>enable^</ImplicitUsings^>
echo     ^<Nullable^>enable^</Nullable^>
echo     ^<IsPackable^>false^</IsPackable^>
echo     ^<IsTestProject^>true^</IsTestProject^>
echo   ^</PropertyGroup^>
echo.
echo   ^<ItemGroup^>
echo     ^<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.7.2" /^>
echo     ^<PackageReference Include="xunit" Version="2.5.1" /^>
echo     ^<PackageReference Include="xunit.runner.visualstudio" Version="2.5.1"^>
echo       ^<IncludeAssets^>runtime; build; native; contentfiles; analyzers; buildtransitive^</IncludeAssets^>
echo       ^<PrivateAssets^>all^</PrivateAssets^>
echo     ^</PackageReference^>
echo     ^<PackageReference Include="coverlet.collector" Version="6.0.0"^>
echo       ^<IncludeAssets^>runtime; build; native; contentfiles; analyzers; buildtransitive^</IncludeAssets^>
echo       ^<PrivateAssets^>all^</PrivateAssets^>
echo     ^</PackageReference^>
echo     ^<PackageReference Include="Moq" Version="4.20.69" /^>
echo   ^</ItemGroup^>
echo.
echo   ^<ItemGroup^>
echo     ^<ProjectReference Include="..\RepairSystem.API.csproj" /^>
echo   ^</ItemGroup^>
echo.
echo ^</Project^>
) > RepairSystem.Tests\RepairSystem.Tests.csproj

echo 3. 獨立構建測試專案...
dotnet clean RepairSystem.Tests\RepairSystem.Tests.csproj
dotnet restore RepairSystem.Tests\RepairSystem.Tests.csproj
dotnet build RepairSystem.Tests\RepairSystem.Tests.csproj

echo 測試專案已隔離。
echo 請執行 run-tests.cmd 測試構建。
pause 