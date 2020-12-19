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
            // ���IHttpContextAccessor
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            #region JWT��֤

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

            #endregion

            #region ���ڲ��Ե���Ȩ

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

            #endregion

            #region EFCore

            services.AddDbContext<MyContext>(options =>
            {
                options
                .UseSqlServer(Configuration.GetConnectionString("SqlServer"), sqlServerOptions =>
                {
                    sqlServerOptions.MaxBatchSize(1_0000);// ����ִ��SQL�����������
                    // ʹ��ROWNUM��ҳ����ΪSQLSERVER 2012���²�֧��Fetch Next
                    // ��3.0��ʼ�Ѿ��Ƴ�
                    //sqlServerOptions.UseRowNumberForPaging();
                })
                .UseLazyLoadingProxies();// �����ӳټ���

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
                options.JsonSerializerOptions.PropertyNamingPolicy = null;// ���л����ı���������
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

            services.AddHangfireServer();// ע��IHostedService

            #endregion

            services.AddSingleton<TaskManager>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,TaskManager taskManager)
        {
            #region �쳣�����м��
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

            taskManager.RegisterTasks();// ע��Ҫִ�е�����ͼƻ�
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
