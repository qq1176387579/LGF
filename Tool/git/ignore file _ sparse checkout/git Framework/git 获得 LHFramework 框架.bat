@echo off 
cd  %~dp0
::稀疏检出工具 获得 Assets/LHFramework 文件 放跟目录

git init

git remote add origin https://gitee.com/lh1176387579/MyFramework.git       

git config core.sparsecheckout true

echo Assets/LHFramework/ >> .git/info/sparse-checkout  
echo Tool/git/ignore file _ sparse checkout >> .git/info/sparse-checkout  

git pull origin master



