# 配置系统服务 (ConfigSystemInfo)
主要功能有

1. 通信，使用Socket
- 1.1 协议，JSON
2. 功能
- 2.1 修改系统时间
- 2.2 修改网络信息(IP地址,子网掩码,默认网关,DNS)
3. 查询
- 3.1 查询IP信息 (IP地址,子网掩码,默认网关,DNS)

# 获取IP
发送

```csharp
{"type","getIP"}
```

返回

```csharp
[{"Key":"IP","Value":"192.168.198.134"},{"Key":"ZWYM","Value":"255.255.254.0"},{"Key":"MRWG","Value":"192.168.198.2"},{"Key":"DNS","Value":"192.168.198.2"}]
```

# 设置时间
```csharp
{"type":"setSysTime","value":"2021-07-12 18:40:50"}
```

返回

```csharp
{"type":"set","value":"OK"}
```

# 设置IP信息
```csharp
{"type":"configIP","value":[{"type":"setIP","value":"192.168.198.134"},{"type":"setZWYM","value":"255.255.254.0"},{"type":"setMRWG","value":"192.168.198.2"},{"type":"setDNS","value":"192.168.198.2"}]}
```

返回(如果设置错误，可能也不会返回)

```csharp
{"type":"set","value":"OK"}
```

# 需要修改的配置
配置路径: appsettings.json
```csharp
{
  "NetworkServer": {
    "ServerPort": 1235,
    "NetworkCard": "ens33"
  }
}
```

1. ServerPort 服务监听的端口
2. 需要指定修改的网卡卡名

# Ubantu 18.4 部署.NET 6 环境
## 包签名
```csharp
wget https://packages.microsoft.com/config/ubuntu/18.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb
```
## 安装SDK
```csharp
sudo apt-get update; \
  sudo apt-get install -y apt-transport-https && \
  sudo apt-get update && \
  sudo apt-get install -y dotnet-sdk-6.0
```

## 安装运行时
```csharp
sudo apt-get install -y dotnet-runtime-6.0
```

```csharp
sudo apt-get update; \
  sudo apt-get install -y apt-transport-https && \
  sudo apt-get update && \
  sudo apt-get install -y aspnetcore-runtime-6.0
```


# 服务后台服务启动。

可以使用以下相关脚本，管理服务(Script/configsysteminfo.service)
```csharp
sudo systemctl daemon-reload
sudo systemctl status configsysteminfo
sudo systemctl stop configsysteminfo.service
sudo systemctl enable configsysteminfo.service
sudo systemctl disabled configsysteminfo.service
查看日志
sudo journalctl -u configsysteminfo
应用服务目录Logs文件
```

也可以使用我自己创建的脚本管理服务
参考Script/README.md