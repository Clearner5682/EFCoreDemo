using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Web.Middlewares
{
    public class CustomMiddleware
    {
        private readonly IAuthorizationPolicyProvider policyProvider;
        private readonly RequestDelegate next;
        private Dictionary<string, int> dic = new Dictionary<string, int>();

        public CustomMiddleware(IAuthorizationPolicyProvider policyProvider,RequestDelegate next)
        {
            this.policyProvider = policyProvider;
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            if (!dic.ContainsKey(endpoint.DisplayName))
            {
                dic.Add(endpoint.DisplayName, 1);
            }
            else
            {
                dic[endpoint.DisplayName]++;
            }

            await this.next.Invoke(context);
        }
    }
}
