@echo off 
cd  %~dp0/../../../Client/
echo 路径 : %cd% 

git update-index --no-assume-unchanged Packages/manifest.json
git update-index --no-assume-unchanged Packages/packages-lock.json

git update-index --no-assume-unchanged ProjectSettings/EditorBuildSettings.asset
git update-index --no-assume-unchanged ProjectSettings/ProjectSettings.asset
git update-index --assume-unchanged ProjectSettings/ProjectVersion.txt

git update-index --no-assume-unchanged UserSettings/EditorUserSettings.asset

@REM git update-index --no-assume-unchanged 
echo 忽略取消成功
pause >nul


