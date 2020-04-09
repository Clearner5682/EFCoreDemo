using System;
using System.Collections.Generic;
using System.Text;
using Database;
using IRepository;

namespace Repository
{
    public class BaseRepository<Entity> : IBaseRepository<Entity> where Entity : class
    {
        protected IUnitOfWork UnitOfWork;
        protected MyContext Context;

        public BaseRepository(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
            Context = unitOfWork.GetContext();
        }

        public void Add(Entity entity)
        {
            Context.Add(entity).SaveChanges();
        }

        public void Delete(Entity entity)
        {
            Context.Remove(entity).SaveChanges();
        }

        public void DeleteById(object id)
        {
            Entity entity = Context.Find<Entity>(id);
            if (entity == null)
            {
                throw new Exception($"根据ID{id}获取实体失败");
            }
            Delete(entity);
        }

        public Entity GetById(object id)
        {
            return Context.Find<Entity>(id);
        }

        public void Update(Entity entity)
        {
            Context.Update(entity).SaveChanges();
        }
    }
}
