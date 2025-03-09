@echo off
echo 清理專案...

dotnet clean RepairSystem.API.csproj
rd /s /q obj
rd /s /q bin
cd RepairSystem.Tests
rd /s /q obj
rd /s /q bin
cd ..

echo 專案已清理完成。
echo 請執行 rebuild-solution.cmd 重建解決方案。
pause 