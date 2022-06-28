@echo off 
cd  %~dp0/../../..

git update-index --assume-unchanged Packages/manifest.json
git update-index --assume-unchanged Packages/packages-lock.json

git update-index --assume-unchanged ProjectSettings/EditorBuildSettings.asset
git update-index --assume-unchanged ProjectSettings/ProjectSettings.asset
git update-index --assume-unchanged ProjectSettings/ProjectVersion.txt

git update-index --assume-unchanged UserSettings/EditorUserSettings.asset
@REM git update-index --assume-unchanged 

