@echo off

echo 1. 清理專案...
dotnet clean
rd /s /q bin 2>nul
rd /s /q obj 2>nul

echo 2. 刪除舊的測試專案...
rd /s /q RepairSystem.Tests 2>nul

echo 3. 創建新的測試專案...
mkdir RepairSystem.Tests
mkdir RepairSystem.Tests\Helpers
mkdir RepairSystem.Tests\Data

echo 4. 創建基本測試文件...
(
echo using Microsoft.Extensions.Logging;
echo using Moq;
echo using Xunit;
echo using System;
echo 
echo namespace RepairSystem.Tests
echo {
echo     public class BasicTests
echo     {
echo         [Fact]
echo         public void SampleTest()
echo         {
echo             // 簡單測試確保測試框架正常運作
echo             Assert.True(true);
echo         }
echo     }
echo }
) > RepairSystem.Tests\BasicTests.cs

echo 5. 創建測試專案文件...
(
echo ^<Project Sdk="Microsoft.NET.Sdk"^>
echo 
echo   ^<PropertyGroup^>
echo     ^<TargetFramework^>net8.0^</TargetFramework^>
echo     ^<ImplicitUsings^>enable^</ImplicitUsings^>
echo     ^<Nullable^>enable^</Nullable^>
echo     ^<IsPackable^>false^</IsPackable^>
echo     ^<IsTestProject^>true^</IsTestProject^>
echo   ^</PropertyGroup^>
echo 
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
echo     ^<PackageReference Include="Dapper" Version="2.1.15" /^>
echo   ^</ItemGroup^>
echo 
echo   ^<ItemGroup^>
echo     ^<ProjectReference Include="..\RepairSystem.API.csproj" /^>
echo   ^</ItemGroup^>
echo 
echo ^</Project^>
) > RepairSystem.Tests\RepairSystem.Tests.csproj

echo 6. 更新解決方案文件...
(
echo Microsoft Visual Studio Solution File, Format Version 12.00
echo # Visual Studio Version 17
echo VisualStudioVersion = 17.0.31903.59
echo MinimumVisualStudioVersion = 10.0.40219.1
echo Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "RepairSystem.API", "RepairSystem.API.csproj", "{54DF7D77-E5AA-4A59-A42B-E764A1D3A1AD}"
echo EndProject
echo Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "RepairSystem.Tests", "RepairSystem.Tests\RepairSystem.Tests.csproj", "{60C0A8B2-F27B-4E0C-92F3-38E1C00A5AB5}"
echo EndProject
echo Global
echo 	GlobalSection(SolutionConfigurationPlatforms) = preSolution
echo 		Debug|Any CPU = Debug|Any CPU
echo 		Release|Any CPU = Release|Any CPU
echo 	EndGlobalSection
echo 	GlobalSection(ProjectConfigurationPlatforms) = postSolution
echo 		{54DF7D77-E5AA-4A59-A42B-E764A1D3A1AD}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
echo 		{54DF7D77-E5AA-4A59-A42B-E764A1D3A1AD}.Debug|Any CPU.Build.0 = Debug|Any CPU
echo 		{54DF7D77-E5AA-4A59-A42B-E764A1D3A1AD}.Release|Any CPU.ActiveCfg = Release|Any CPU
echo 		{54DF7D77-E5AA-4A59-A42B-E764A1D3A1AD}.Release|Any CPU.Build.0 = Release|Any CPU
echo 		{60C0A8B2-F27B-4E0C-92F3-38E1C00A5AB5}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
echo 		{60C0A8B2-F27B-4E0C-92F3-38E1C00A5AB5}.Debug|Any CPU.Build.0 = Debug|Any CPU
echo 		{60C0A8B2-F27B-4E0C-92F3-38E1C00A5AB5}.Release|Any CPU.ActiveCfg = Release|Any CPU
echo 		{60C0A8B2-F27B-4E0C-92F3-38E1C00A5AB5}.Release|Any CPU.Build.0 = Release|Any CPU
echo 	EndGlobalSection
echo 	GlobalSection(SolutionProperties) = preSolution
echo 		HideSolutionNode = FALSE
echo 	EndGlobalSection
echo 	GlobalSection(ExtensibilityGlobals) = postSolution
echo 		SolutionGuid = {D39F17CA-6A05-4E4F-9C52-6F12B8E4E7D3}
echo 	EndGlobalSection
echo EndGlobal
) > RepairSystem.sln

echo 7. 構建測試專案...
dotnet restore RepairSystem.Tests\RepairSystem.Tests.csproj
dotnet build RepairSystem.Tests\RepairSystem.Tests.csproj

echo 8. 運行基本測試確認測試框架正常...
dotnet test RepairSystem.Tests\RepairSystem.Tests.csproj

echo 修復完成！您現在可以運行 create-mock-dapper-context.cmd 和 create-dapper-tests.cmd 添加完整測試。
pause 