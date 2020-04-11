using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using IRepository;
using Model;

namespace Repository
{
    public class RoleRepository : BaseRepository<Role>,IRoleRepository
    {
        public RoleRepository(IUnitOfWork unitOfWork):base(unitOfWork)
        {
            
        }

        public IList<Role> GetRoleList()
        {
            return Context.Set<Role>().ToList();
        }
    }
}
