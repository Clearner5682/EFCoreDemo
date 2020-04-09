using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Proxies;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using System.IO;
using Autofac;
using Autofac.Extras.DynamicProxy;
using Database;

namespace Web
{
    public class Startup
    {
        IConfiguration Configuration;
        IWebHostEnvironment Env;
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Env = env;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<MyContext>(options =>
            {
                options
                .UseSqlServer(Configuration.GetConnectionString("SqlServer"), sqlServerOptions =>
                {
                    sqlServerOptions.MaxBatchSize(1_0000);// 批量执行SQL语句的最大数量
                })
                .UseLazyLoadingProxies();// 启用延迟加载

                options.EnableSensitiveDataLogging();
            });
            services.AddControllers(options =>
            {
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{Controller=Home}/{Action=Index}/{id?}");
            });
        }

        // 这是不带环境变量的方法
        // 如果环境变量为Development则会先查找ConfigureDevelopmentContainer
        // 如果环境变量为Production则会先查找ConfigureProductionContainer
        // 带环境变量的方法都找不到，则会找ConfigureContainer
        public void ConfigureContainer(ContainerBuilder builder)// 在Startup内部动态获取该方法和参数类型，然后调用该方法
        {
            #region 测试用的代码，注释掉

            //// 通过反射注入
            //builder.RegisterType<DependencyService>()
            //    .As<IDependencyService>()
            //    .InstancePerDependency();// 瞬时模式

            //// 通过委托注入服务，比反射好的地方在于除了构造函数，还可以做一些其他的事情
            //builder.Register<GreetService>(
            //    (c, p) =>
            //    {
            //        return new GreetService(p.Named<string>("name"));
            //    })
            //    .As<IGreetService>()
            //    .SingleInstance();// 单例模式

            //// 通过实例注入，生存周期只能是单例模式
            //var output = new StringWriter();
            //builder.RegisterInstance(output)
            //    .As<TextWriter>()
            //    .SingleInstance();

            //// 泛型实例注入
            //builder.RegisterGeneric(typeof(Repository<>))
            //    .As(typeof(IRepository<>))
            //    .InstancePerLifetimeScope();// 同一个LifeScope内的实例是相同的

            //// *这样是错误的，因为只注入了类型，没有实现
            ////builder.RegisterType<IDependencyService>();
            //// *这样是正确的，因为自己既是类型，也是实现
            //builder.RegisterType<DependencyService>()
            //    .AsSelf()
            //    .SingleInstance();

            #endregion

            
            Assembly assemblyRepository = Assembly.LoadFrom(Path.Combine(AppContext.BaseDirectory, "Repository.dll"));
            string test1 = Path.Combine(AppContext.BaseDirectory, "Repository.dll");
            Assembly assemblyServices = Assembly.LoadFrom(Path.Combine(AppContext.BaseDirectory, "Services.dll"));
            string test2= Path.Combine(AppContext.BaseDirectory, "Services.dll");

            builder.RegisterAssemblyTypes(assemblyRepository)
                .AsImplementedInterfaces()
                .InstancePerDependency();// 单例模式
            

            builder.RegisterAssemblyTypes(assemblyServices)
                .AsImplementedInterfaces()
                .InstancePerDependency();// 单例模式
        }
    }
}
