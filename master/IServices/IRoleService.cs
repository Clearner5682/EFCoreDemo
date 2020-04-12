using System;
using System.Collections.Generic;
using System.Text;
using Model;

namespace IServices
{
    public interface IRoleService:IBaseService<Role>
    {
        IList<Role> GetRoleList();
    }
}
