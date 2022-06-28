# MyFramework


[ignore file _ unity package or setting]  作用于项目 
设置 [unity Packages] [ProjectSettings] [UserSettings] 的忽略 设置的



[ignore file _ sparse checkout]  作用稀疏检出
设置 外部使用单个 MyFramework 里面的框架 需要忽略提交的到github的

[ignore file _ sparse checkout/git Framework]  有检出完整工程流程



新工程执行下面忽略命令  设置自己的本地忽略   这里是提示
------------------------------------------------------------------------------------------------------------------
git update-index --assume-unchanged Assets/LHFramework/AppConfig/AppInfo/AppInfoConfig.asset
------------------------------------------------------------------------------------------------------------------

场景：
开发的时候，我们可能需要的开发环境部分配置跟测试及生产环境不一样，如数据库配置，需要手动修改，又不想把修改过的配置文件提交，这时候只需要用到下边的命令。
git update-index --assume-unchanged [file-path]

git update-index --no-assume-unchanged [file-path]


三、恢复跟踪
恢复单一文件

git update-index --no-assume-unchanged <路径名>
1
查看所有已忽略文件

git ls-files -v | grep '^h\ '


取消忽略所有文件

提取文件路径

git ls-files -v | grep '^h\ ' | awk '{print $2}'
1


取消忽略

git ls-files -v | grep '^h' | awk '{print $2}' |xargs git update-index --no-assume-unchanged 


链接
https://blog.csdn.net/Old_D7/article/details/121395313