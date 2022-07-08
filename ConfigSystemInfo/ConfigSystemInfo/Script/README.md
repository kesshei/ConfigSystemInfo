# 配置成系统服务 (ConfigSystemInfo) 
需要把服务配置成系统Syscmd服务，才可以开机后启动。

# 第一步
修改  configsysteminfo.service 里面的
```csharp
ExecStart=/usr/bin/dotnet  /home/kesshei/Desktop/net6.0/ConfigSystemInfo.dll
```
为指定路径即可
# 第二步 (初始化服务)
在脚本目录下执行

```
sudo ./initserver.sh
```

输出以下内容就正常

```
copy success
Created symlink /etc/systemd/system/multi-user.target.wants/configsysteminfo.service → /etc/systemd/system/configsysteminfo.service.
Server start Success
Server Init Success
```

然后，查看 /Logs 文件下的日志，是否写入，即可。

即可

# 其他
可以通过
1. start.sh 开启服务，默认初始化已开启
2. stop.sh 停止服务
3. uninstallService.sh 卸载服务

