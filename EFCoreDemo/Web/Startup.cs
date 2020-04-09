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
                    sqlServerOptions.MaxBatchSize(1_0000);// ����ִ��SQL�����������
                })
                .UseLazyLoadingProxies();// �����ӳټ���

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

        // ���ǲ������������ķ���
        // �����������ΪDevelopment����Ȳ���ConfigureDevelopmentContainer
        // �����������ΪProduction����Ȳ���ConfigureProductionContainer
        // �����������ķ������Ҳ����������ConfigureContainer
        public void ConfigureContainer(ContainerBuilder builder)// ��Startup�ڲ���̬��ȡ�÷����Ͳ������ͣ�Ȼ����ø÷���
        {
            #region �����õĴ��룬ע�͵�

            //// ͨ������ע��
            //builder.RegisterType<DependencyService>()
            //    .As<IDependencyService>()
            //    .InstancePerDependency();// ˲ʱģʽ

            //// ͨ��ί��ע����񣬱ȷ���õĵط����ڳ��˹��캯������������һЩ����������
            //builder.Register<GreetService>(
            //    (c, p) =>
            //    {
            //        return new GreetService(p.Named<string>("name"));
            //    })
            //    .As<IGreetService>()
            //    .SingleInstance();// ����ģʽ

            //// ͨ��ʵ��ע�룬��������ֻ���ǵ���ģʽ
            //var output = new StringWriter();
            //builder.RegisterInstance(output)
            //    .As<TextWriter>()
            //    .SingleInstance();

            //// ����ʵ��ע��
            //builder.RegisterGeneric(typeof(Repository<>))
            //    .As(typeof(IRepository<>))
            //    .InstancePerLifetimeScope();// ͬһ��LifeScope�ڵ�ʵ������ͬ��

            //// *�����Ǵ���ģ���Ϊֻע�������ͣ�û��ʵ��
            ////builder.RegisterType<IDependencyService>();
            //// *��������ȷ�ģ���Ϊ�Լ��������ͣ�Ҳ��ʵ��
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
                .InstancePerDependency();// ����ģʽ
            

            builder.RegisterAssemblyTypes(assemblyServices)
                .AsImplementedInterfaces()
                .InstancePerDependency();// ����ģʽ
        }
    }
}
