using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Model;
using IServices;
using Database;

namespace Web.Controllers
{
    public class RoleController : Controller
    {
        IRoleService _roleService;
        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpPost]
        public IActionResult AddRole(string RoleName)
        {
            Role model = new Role();
            model.Id = Guid.NewGuid();
            model.RoleName = RoleName;
            model.CreateTime = DateTime.Now;
            model.LastUpdateTime = DateTime.Now;
            _roleService.Add(model);

            return Ok(new { ErrorCode="0"});
        }

        public IActionResult GetRoleList()
        {
            var roleList = _roleService.GetRoleList();

            return Ok(new { ErrorCode="0",RoleList=roleList});
        }
    }
}
