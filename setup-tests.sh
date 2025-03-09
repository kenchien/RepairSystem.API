#!/bin/bash
echo "正在為測試專案安裝必要的NuGet套件..."

cd RepairSystem.Tests

echo "安裝xUnit測試框架..."
dotnet add package xunit --version 2.5.1
dotnet add package xunit.runner.visualstudio --version 2.5.1

echo "安裝Moq模擬框架..."
dotnet add package Moq --version 4.20.69

echo "安裝其他測試相關套件..."
dotnet add package Microsoft.NET.Test.Sdk --version 17.7.2
dotnet add package coverlet.collector --version 6.0.0

echo "測試相關套件安裝完成。"
echo "請執行 run-tests.sh 運行測試。"

cd ..
echo "按 Enter 鍵繼續..."
read 