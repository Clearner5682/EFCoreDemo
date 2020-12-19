using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Filters
{
    public class CustomAuthorizeFilter : AuthorizeFilter
    {
        //public CustomAuthorizeFilter():base(policy: "MyPolicy")
        //{

        //}

        //如果用下面这个无参数的构造函数，则没有指定授权策略，当用户没有登录时会直接返回401
        //此时相当于加了一个全局的AuthorizeAttribute特性
        public CustomAuthorizeFilter():base()
        {

        }

        //public override async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        //{
        //    await base.OnAuthorizationAsync(context);
        //}

        public override async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (string.IsNullOrEmpty(context.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "userid")?.Value))
            {
                context.Result = new JsonResult(new { StatusCode = 401, Message = "请登录" });
            }
            await base.OnAuthorizationAsync(context);
        }
    }

    public class CustomAuthorizeFilter2 : IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            await Task.Run(() => {
                var endpoint = context.HttpContext.Features.Get<IEndpointFeature>().Endpoint;
                context.Result = new UnauthorizedResult();
            });
        }
    }
}
