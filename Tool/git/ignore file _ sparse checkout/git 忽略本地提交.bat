@echo off 
cd  %~dp0/../../../Client/
echo 路径 : %cd% 

git update-index --assume-unchanged Assets/LHFramework/AppConfig/AppInfo/AppInfoConfig.asset
git update-index --assume-unchanged Assets/LHFramework/Manager/Helper/GameEventType.cs


echo 忽略成功 
pause >nul