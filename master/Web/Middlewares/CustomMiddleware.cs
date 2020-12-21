using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Session;
using Microsoft.Extensions.Logging;

namespace Web.Middlewares
{
    public class CustomMiddleware
    {
        private readonly IAuthorizationPolicyProvider policyProvider;
        private readonly RequestDelegate next;
        private readonly ILogger<CustomMiddleware> _logger;
        private readonly IDataProtector _dataProtector;
        private readonly SessionOptions _options;
        private Dictionary<string, int> dic = new Dictionary<string, int>();

        public CustomMiddleware(IAuthorizationPolicyProvider policyProvider,RequestDelegate next,IDataProtectionProvider dataProtectionProvider, IOptions<SessionOptions> options,ILogger<CustomMiddleware> logger)
        {
            this.policyProvider = policyProvider;
            this.next = next;
            this._logger = logger;
            this._dataProtector = dataProtectionProvider.CreateProtector(nameof(SessionMiddleware));
            this._options = options.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var cookieValue = context.Request.Cookies[_options.Cookie.Name];
            var sessionKey = CookieProtection.Unprotect(_dataProtector, cookieValue, _logger);
            var endpoint = context.GetEndpoint();
            if (endpoint != null)
            {
                if (!dic.ContainsKey(endpoint.DisplayName))
                {
                    dic.Add(endpoint.DisplayName, 1);
                }
                else
                {
                    dic[endpoint.DisplayName]++;
                }
            }

            await this.next.Invoke(context);
        }
    }
}
