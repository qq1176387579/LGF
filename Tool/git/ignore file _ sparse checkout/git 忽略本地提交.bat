@echo off 
cd  %~dp0/../../..

git update-index --assume-unchanged Assets/LHFramework/AppConfig/AppInfo/AppInfoConfig.asset
git update-index --assume-unchanged Assets/LHFramework/Manager/Helper/GameEventType.cs
