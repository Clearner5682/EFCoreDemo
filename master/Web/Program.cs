using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Autofac;
using Autofac.Extensions.DependencyInjection;

namespace Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    //webBuilder.UseKestrel(options => {
                    //    //监听https的默认端口443
                    //    //options.ListenAnyIP(443, listenOptions => {
                    //    //    //listenOptions.UseHttps("server.pfx", "123456");//使用指定的https证书
                    //    //    //listenOptions.UseHttps();//使用默认的https证书
                    //    //});
                    //    //监听http端口5000和6000
                    //    options.ListenAnyIP(5000);
                    //    options.ListenAnyIP(6000);
                    //});
                });
    }
}
