@echo off
echo 修復測試專案...

echo 1. 清理專案...
call clean-project.cmd

echo 2. 刪除舊的測試目錄...
rd /s /q RepairSystem.Tests

echo 3. 重建解決方案...
call rebuild-solution.cmd

echo 測試專案修復完成。
echo 請運行 run-tests.cmd 執行測試。
pause 