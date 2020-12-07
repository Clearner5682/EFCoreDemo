using Microsoft.AspNetCore.Authorization;
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
        public CustomAuthorizeFilter():base(policy:"MyPolicy")
        {

        }

        //如果用下面这个无参数的构造函数，则没有指定授权策略，当用户没有登录时会直接返回401
        //此时相当于加了一个全局的AuthorizeAttribute特性
        //public CustomAuthorizeFilter():base()
        //{

        //}
    }
}
