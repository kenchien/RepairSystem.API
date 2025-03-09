@echo off
echo 重建解決方案...

echo 1. 建立分離的測試專案...
dotnet new xunit -o RepairSystem.Tests
dotnet sln RepairSystem.sln add RepairSystem.Tests

echo 2. 安裝測試套件...
cd RepairSystem.Tests
dotnet add package Moq --version 4.20.69
dotnet add package Microsoft.NET.Test.Sdk --version 17.7.2
dotnet add package xunit --version 2.5.1
dotnet add package xunit.runner.visualstudio --version 2.5.1
dotnet add package coverlet.collector --version 6.0.0
cd ..

echo 3. 設置專案引用...
dotnet add RepairSystem.Tests/RepairSystem.Tests.csproj reference RepairSystem.API.csproj

echo 4. 重建解決方案...
dotnet build RepairSystem.sln

echo 解決方案重建完成。
echo 請執行 run-tests.cmd 運行測試。
pause 