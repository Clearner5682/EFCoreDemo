using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
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
using Hangfire;
using Hangfire.AspNetCore;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Mvc.Filters;
using Web.Filters;
using Microsoft.AspNetCore.Diagnostics;
using Web.Exceptions;
using Microsoft.AspNetCore.Http.Features;
using Web.Middlewares;
using Newtonsoft.Json;

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
            // 添加IHttpContextAccessor
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            #region JWT认证

            // 添加认证服务
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

            #endregion

            #region 基于策略的授权

            // 添加授权服务
            services.AddAuthorization(options => {
                options.AddPolicy("AdminOrUser", builder => {
                    builder.RequireRole("Admin", "User").Build();
                });
                options.AddPolicy("AdminAndUser", builder => {
                    builder.RequireRole("Admin").RequireRole("User").Build();
                });

                // 自定义授权
                options.AddPolicy("MyPolicy", builder => {
                    builder.Requirements.Add(new PermissionRequirement());
                });
            });
            // 添加自定义授权的处理服务
            services.AddScoped<IAuthorizationHandler, PermissionHandler>();

            #endregion

            #region EFCore

            services.AddDbContext<MyContext>(options =>
            {
                options
                .UseSqlServer(Configuration.GetConnectionString("SqlServer"), sqlServerOptions =>
                {
                    sqlServerOptions.MaxBatchSize(1_0000);// 批量执行SQL语句的最大数量
                    // 使用ROWNUM分页，因为SQLSERVER 2012以下不支持Fetch Next
                    // 从3.0开始已经移除
                    //sqlServerOptions.UseRowNumberForPaging();
                })
                .UseLazyLoadingProxies();// 启用延迟加载

                options.EnableSensitiveDataLogging();
            });

            #endregion

            // MVC
            services.AddControllers(options =>
            {
                //options.Filters.Add(typeof(CustomAuthorizeFilter));
                //options.Filters.Add(new CustomAuthorizeFilter());
                //options.Filters.Add<CustomAuthorizeFilter>();
                //options.Filters.Add<CustomAuthorizeFilter2>();
            })
            .AddJsonOptions(options=> {
                options.JsonSerializerOptions.PropertyNamingPolicy = null;// 序列化不改变属性名称
                //options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });

            #region Hangfire

            services.AddHangfire(configuration => {
                configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(Configuration.GetConnectionString("Hangfire"), new SqlServerStorageOptions 
                {
                    CommandBatchMaxTimeout=TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout=TimeSpan.FromMinutes(5),
                    QueuePollInterval=TimeSpan.Zero,
                    UseRecommendedIsolationLevel=true,
                    UsePageLocksOnDequeue=true,
                    DisableGlobalLocks=true
                });
            });

            services.AddHangfireServer();// 注入IHostedService

            #endregion

            services.AddSingleton<TaskManager>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,TaskManager taskManager)
        {
            #region 异常处理中间件
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}
            app.UseExceptionHandler(new ExceptionHandlerOptions
            {
                ExceptionHandler = async (context) => {
                    await Task.Run(() => {
                        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                        if (exceptionHandlerPathFeature.Error is UnAuthorizedException)
                        {
                            context.Response.StatusCode = 401;
                            return;
                        }

                        context.Response.WriteAsync(exceptionHandlerPathFeature.Error.Message);
                    });
                }
            });
            #endregion

            #region MiddlewareTest

            app.Use(async (context, next) =>
            {
                if (!context.Response.HasStarted)
                {
                    context.Response.ContentType = "text/html;charset=utf-8";
                    context.Response.OnStarting(async () => {
                        await Task.Run(() => {
                            if (!context.Response.HasStarted)
                            {
                                context.Response.Headers.Add("Test1", "Test1");
                            }
                        });
                    });
                }
                await context.Response.WriteAsync("Middleware1Start\r\n");
                await next.Invoke();
                await context.Response.WriteAsync("Middleware1End\r\n");
            });

            app.Use(next => {
                return async context => {
                    await context.Response.WriteAsync("Middleware2Start\r\n");
                    await next(context);
                    await context.Response.WriteAsync("Middleware2End\r\n");
                };
            });

            //app.Run(async context =>
            //{
            //    await context.Response.WriteAsync(JsonConvert.SerializeObject(new { Result = "Middleware3End" }));
            //});

            #endregion

            app.UseStaticFiles();

            app.UseHangfireDashboard();

            app.UseRouting();

            app.UseAuthentication();
            app.UseMiddleware<CustomMiddleware>();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{Controller=Home}/{Action=Index}/{id?}");
                endpoints.MapControllerRoute("api", "api/{Controller}/{Action}/{id?}");
            });

            taskManager.RegisterTasks();// 注入要执行的任务和计划
        }

        // 这是不带环境变量的方法
        // 如果环境变量为Development则会先查找ConfigureDevelopmentContainer
        // 如果环境变量为Production则会先查找ConfigureProductionContainer
        // 带环境变量的方法都找不到，则会找ConfigureContainer
        public void ConfigureContainer(ContainerBuilder builder)// 在Startup内部动态获取该方法和参数类型，然后调用该方法
        {
            #region 测试用的代码，注释掉

            // 通过反射注入
            builder.RegisterType<DependencyService>()
                .As<IDependencyService>()
                .InstancePerDependency();// 瞬时模式

            // 通过委托注入服务，比反射好的地方在于除了构造函数，还可以做一些其他的事情
            builder.Register<GreetService>(
                (c, p) =>
                {
                    return new GreetService(p.Named<string>("name"));
                })
                .As<IGreetService>()
                .SingleInstance();// 单例模式

            // 通过实例注入，生存周期只能是单例模式
            var output = new StringWriter();
            builder.RegisterInstance(output)
                .As<TextWriter>()
                .SingleInstance();

            // 泛型实例注入
            builder.RegisterGeneric(typeof(Repository<>))
                .As(typeof(IRepository<>))
                .InstancePerLifetimeScope();// 同一个LifeScope内的实例是相同的

            // *这样是错误的，因为只注入了类型，没有实现
            //builder.RegisterType<IDependencyService>();
            // *这样是正确的，因为自己既是类型，也是实现
            builder.RegisterType<DependencyService>()
                .AsSelf()
                .SingleInstance();

            #endregion


            Assembly assemblyRepository = Assembly.LoadFrom(Path.Combine(AppContext.BaseDirectory, "Repository.dll"));
            Assembly assemblyServices = Assembly.LoadFrom(Path.Combine(AppContext.BaseDirectory, "Services.dll"));


            builder.RegisterAssemblyTypes(assemblyRepository)
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();// 这里如果用瞬时模式，则Service中的工作单元和Repository中的工作单元不是同一个实例
            

            builder.RegisterAssemblyTypes(assemblyServices)
                .AsImplementedInterfaces()
                .InstancePerDependency();
        }
    }
}
