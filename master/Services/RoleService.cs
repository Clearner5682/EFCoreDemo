using System;
using System.Collections.Generic;
using System.Text;
using Database;
using IRepository;
using IServices;
using Model;

namespace Services
{
    public class RoleService : BaseService<Role>,IRoleService
    {
        private IRoleRepository _roleRepository;
        public RoleService(IUnitOfWork unitOfWork,IRoleRepository roleRepository):base(roleRepository)
        {
            _roleRepository = roleRepository;
            bool isEqual = _roleRepository == BaseRepository;
            var unitOfWork2 = roleRepository.GetUnitOfWork();
            bool isEqual2 = unitOfWork == unitOfWork2;
        }

        public IList<Role> GetRoleList()
        {
            return _roleRepository.GetRoleList();
        }
    }
}
