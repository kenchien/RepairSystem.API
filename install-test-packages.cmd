@echo off
echo 正在為測試專案安裝必要的NuGet套件...

dotnet add RepairSystem.Tests/RepairSystem.Tests.csproj package xunit --version 2.5.1
dotnet add RepairSystem.Tests/RepairSystem.Tests.csproj package xunit.runner.visualstudio --version 2.5.1
dotnet add RepairSystem.Tests/RepairSystem.Tests.csproj package Moq --version 4.20.69
dotnet add RepairSystem.Tests/RepairSystem.Tests.csproj package Microsoft.NET.Test.Sdk --version 17.7.2
dotnet add RepairSystem.Tests/RepairSystem.Tests.csproj package coverlet.collector --version 6.0.0

echo 套件安裝完成。請運行 run-tests.cmd 執行測試。
pause 