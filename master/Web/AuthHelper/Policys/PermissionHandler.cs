using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Utils;
using Model.DTO;

namespace Web
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PermissionHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            if (context.User == null)
            {
                throw new Exception("请先登录");
            }
            var userId = context.User.Claims.First(o => o.Type == "UserId").Value;
            // 可以根据userId去数据库找该userId对应的角色
            // 可以根据角色去找该角色对应的菜单权限
            // 这里只是简单的模拟一下允许特定的UserId访问该Policy保护的资源
            // 从httpcontext获取路由，反射出对应的Action，判断该UseId能否访问该Action
            string requestUrl = _httpContextAccessor.HttpContext.Request.Path.Value;
            IList<MenuInfo> menuInfos = MenuHelper.GetMenuInfos();
            IList<ActionInfo> actionInfos = MenuHelper.GetActionInfos();
            if (menuInfos.Any(o => o.MenuUrl == requestUrl) || actionInfos.Any(o => o.ActionUrl == requestUrl))
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }

            return Task.CompletedTask;
        }
    }
}
