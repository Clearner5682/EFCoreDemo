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
using System.Text;
using Database;
using Utils;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using System.Text.Json.Serialization;

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
            services.AddSingleton(new MenuHelper(Env.ContentRootPath));
            // ���IHttpContextAccessor
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            // �����֤����
            services.AddAuthentication("Bearer")
                .AddJwtBearer(configOptions => {
                    // 3+2
                    configOptions.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetValue<string>("SecretKey"))),
                        ValidateIssuer = true,
                        ValidIssuer = Configuration.GetValue<string>("Issuer"),
                        RequireAudience = true,
                        ValidAudience = Configuration.GetValue<string>("Audience"),


                        RequireExpirationTime = true,
                        ValidateLifetime = true
                    };
                });
            // �����Ȩ����
            services.AddAuthorization(options => {
                options.AddPolicy("AdminOrUser", builder => {
                    builder.RequireRole("Admin", "User").Build();
                });
                options.AddPolicy("AdminAndUser", builder => {
                    builder.RequireRole("Admin").RequireRole("User").Build();
                });

                // �Զ�����Ȩ
                options.AddPolicy("MyPolicy", builder => {
                    builder.Requirements.Add(new PermissionRequirement());
                });
            });
            // ����Զ�����Ȩ�Ĵ������
            services.AddScoped<IAuthorizationHandler, PermissionHandler>();
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
                
            })
            .AddJsonOptions(options=> {
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
                //options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
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
                endpoints.MapControllerRoute("api", "api/{Controller}/{Action}/{id?}");
            });
        }

        // ���ǲ������������ķ���
        // �����������ΪDevelopment����Ȳ���ConfigureDevelopmentContainer
        // �����������ΪProduction����Ȳ���ConfigureProductionContainer
        // �����������ķ������Ҳ����������ConfigureContainer
        public void ConfigureContainer(ContainerBuilder builder)// ��Startup�ڲ���̬��ȡ�÷����Ͳ������ͣ�Ȼ����ø÷���
        {
            #region �����õĴ��룬ע�͵�

            // ͨ������ע��
            builder.RegisterType<DependencyService>()
                .As<IDependencyService>()
                .InstancePerDependency();// ˲ʱģʽ

            // ͨ��ί��ע����񣬱ȷ���õĵط����ڳ��˹��캯������������һЩ����������
            builder.Register<GreetService>(
                (c, p) =>
                {
                    return new GreetService(p.Named<string>("name"));
                })
                .As<IGreetService>()
                .SingleInstance();// ����ģʽ

            // ͨ��ʵ��ע�룬��������ֻ���ǵ���ģʽ
            var output = new StringWriter();
            builder.RegisterInstance(output)
                .As<TextWriter>()
                .SingleInstance();

            // ����ʵ��ע��
            builder.RegisterGeneric(typeof(Repository<>))
                .As(typeof(IRepository<>))
                .InstancePerLifetimeScope();// ͬһ��LifeScope�ڵ�ʵ������ͬ��

            // *�����Ǵ���ģ���Ϊֻע�������ͣ�û��ʵ��
            //builder.RegisterType<IDependencyService>();
            // *��������ȷ�ģ���Ϊ�Լ��������ͣ�Ҳ��ʵ��
            builder.RegisterType<DependencyService>()
                .AsSelf()
                .SingleInstance();

            #endregion


            Assembly assemblyRepository = Assembly.LoadFrom(Path.Combine(AppContext.BaseDirectory, "Repository.dll"));
            Assembly assemblyServices = Assembly.LoadFrom(Path.Combine(AppContext.BaseDirectory, "Services.dll"));


            builder.RegisterAssemblyTypes(assemblyRepository)
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();// ���������˲ʱģʽ����Service�еĹ�����Ԫ��Repository�еĹ�����Ԫ����ͬһ��ʵ��
            

            builder.RegisterAssemblyTypes(assemblyServices)
                .AsImplementedInterfaces()
                .InstancePerDependency();
        }
    }
}
