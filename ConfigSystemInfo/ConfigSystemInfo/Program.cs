using ConfigSystemInfo.Common;
using ConfigSystemInfo.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ConfigSystemInfo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("zh-CN", true) { DateTimeFormat = { ShortTimePattern = "HH:mm:ss", ShortDatePattern = "yyyy-MM-dd", FullDateTimePattern = "yyyy-MM-dd HH:mm:ss", LongDatePattern = "yyyy-MM-dd", LongTimePattern = "HH:mm:ss" } };
            Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            Log.Info("服务准备启动");
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseSystemd()
            .ConfigureServices((hostContext, services) =>
            {
                services.AddHostedService<Worker>();
            });
    }
}
