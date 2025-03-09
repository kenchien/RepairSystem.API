@echo off
echo 更新解決方案文件，確保測試專案被正確包含...

dotnet sln RepairSystem.sln add RepairSystem.Tests/RepairSystem.Tests.csproj

echo 解決方案已更新。
pause 