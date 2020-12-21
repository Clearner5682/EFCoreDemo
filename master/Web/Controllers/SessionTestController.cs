using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Session;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Controllers
{
    public class SessionTestController : Controller
    {
        private readonly IDistributedCache _cache;

        public SessionTestController(IHttpContextAccessor httpContextAccessor,IDistributedCache cache,ISessionStore sessionStore)
        {
            var httpContext = httpContextAccessor.HttpContext;
            this._cache = cache;
            var sessionFeature = httpContext.Features.Get<ISessionFeature>();
            if (sessionFeature != null && sessionFeature.Session != null)
            {
                var session = sessionFeature.Session;
                var keys = session.Keys;
                foreach (var key in keys)
                {
                    byte[] data = this._cache.Get(key);
                }
            }
        }

        public IActionResult AddCount()
        {
            var count = this.HttpContext.Session.GetInt32("count")??0;
            count++;
            this.HttpContext.Session.SetInt32("count", count);

            return Ok();
        }

        public IActionResult GetCount()
        {
            var count = this.HttpContext.Session.GetInt32("count") ?? 0;

            return Json(new { Count = count });
        }

        //public IActionResult GetCount2()
        //{
        //    var count = this.sessionStore.("count") ?? 0;

        //    return Json(new { Count = count });
        //}
    }
}
