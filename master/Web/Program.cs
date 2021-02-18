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
                    //    //����https��Ĭ�϶˿�443
                    //    //options.ListenAnyIP(443, listenOptions => {
                    //    //    //listenOptions.UseHttps("server.pfx", "123456");//ʹ��ָ����https֤��
                    //    //    //listenOptions.UseHttps();//ʹ��Ĭ�ϵ�https֤��
                    //    //});
                    //    //����http�˿�5000��6000
                    //    options.ListenAnyIP(5000);
                    //    options.ListenAnyIP(6000);
                    //});
                });
    }
}
