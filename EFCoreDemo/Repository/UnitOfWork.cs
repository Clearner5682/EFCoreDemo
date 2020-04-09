using System;
using System.Collections.Generic;
using System.Text;
using Database;
using IRepository;

namespace Repository
{
    public class UnitOfWork:IUnitOfWork
    {
        private MyContext _context;
        public UnitOfWork(MyContext context)
        {
            _context = context;
        }

        public void BeginTran()
        {
            _context.Database.BeginTransaction();
        }

        public void CommitTran()
        {
            _context.Database.CommitTransaction();
        }

        public MyContext GetContext()
        {
            return _context;
        }

        public void RollbackTran()
        {
            _context.Database.RollbackTransaction();
        }
    }
}
