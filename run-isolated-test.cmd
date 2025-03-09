@echo off
echo 運行隔離測試...

dotnet test RepairSystem.Tests/RepairSystem.Tests.csproj --logger:console;verbosity=detailed

echo 測試完成。
pause 