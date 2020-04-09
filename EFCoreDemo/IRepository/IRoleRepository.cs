using System;
using System.Collections.Generic;
using System.Text;
using Model;

namespace IRepository
{
    public interface IRoleRepository:IBaseRepository<Role>
    {
        IList<Role> GetRoleList();
    }
}
