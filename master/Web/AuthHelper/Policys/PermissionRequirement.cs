using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Utils;
using Model.DTO;

namespace Web
{
    public class PermissionRequirement:IAuthorizationRequirement
    {
        public IList<MenuInfo> MenuInfos { get; set;}

        public PermissionRequirement()
        {

        }
    }
}
