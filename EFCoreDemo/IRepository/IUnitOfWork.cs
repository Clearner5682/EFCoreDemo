using System;
using System.Collections.Generic;
using System.Text;
using Database;

namespace IRepository
{
    public interface IUnitOfWork
    {
        public MyContext GetContext();

        public void BeginTran();

        public void CommitTran();

        public void RollbackTran();
    }
}
