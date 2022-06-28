
cd  %~dp0/../../../Client/
echo 路径 : %cd% 

git update-index --no-assume-unchanged Assets/LHFramework/AppConfig/AppInfo/AppInfoConfig.asset
git update-index --no-assume-unchanged Assets/LHFramework/Manager/Helper/GameEventType.cs
::git update-index --no-assume-unchanged 


echo 忽略成功 
pause >nul


