using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using Model;
using Database;
using IRepository;
using Utils;

namespace Repository
{
    public class BaseRepository<Entity> : IBaseRepository<Entity> where Entity : Base
    {
        protected IUnitOfWork UnitOfWork;
        protected MyContext Context;

        public BaseRepository(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
            Context = unitOfWork.GetContext();
        }

        public IUnitOfWork GetUnitOfWork()
        {
            return UnitOfWork;
        }

        public void Add(Entity entity)
        {
            SetDefault(entity);
            Context.Add(entity).SaveChanges();
        }

        public void AddRange(ICollection<Entity> entities)
        {
            foreach(var entity in entities)
            {
                SetDefault(entity);
            }
            Context.AddRange(entities);
            Context.SaveChanges();
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

        public IList<Entity> GetAll()
        {
            return Context.Set<Entity>().ToList();
        }

        public void Update(Entity entity)
        {
            UpdateDefault(entity);
            Context.Update(entity).SaveChanges();
        }

        public void UpdateRange(ICollection<Entity> entities)
        {
            foreach(var entity in entities)
            {
                UpdateDefault(entity);
            }
            Context.UpdateRange(entities);
            Context.SaveChanges();
        }

        private void SetDefault(Entity entity)
        {
            var now = DateTime.Now;
            if (entity is SetBase)
            {
                SetBase setBase = entity as SetBase;
                setBase.CreateTime = now;
                setBase.LastUpdateTime = now;
            }
            else if (entity is LogBase)
            {
                LogBase logBase = entity as LogBase;
                logBase.LogTime = now;
            }
            entity.Id = Guid.NewGuid();
        }

        private void UpdateDefault(Entity entity)
        {
            var now = DateTime.Now;
            if (entity is SetBase)
            {
                SetBase setBase = entity as SetBase;
                setBase.LastUpdateTime = now;
            }
        }

        public IList<Entity> SearchTop<EntityKey>(int count, Expression<Func<Entity, EntityKey>> keySelector, bool isAscending, Expression<Func<Entity, bool>> predicate = null)
        {
            IQueryable<Entity> query = Context.Set<Entity>();
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            if (isAscending)
            {
                query = query.OrderBy(keySelector);
            }
            else
            {
                query = query.OrderByDescending(keySelector);
            }

            return query.Take(count).ToList();
        }

        public IList<Entity> SearchPage(string sort,bool isAscending, int page, int pageSize,out int total, Expression<Func<Entity, bool>> predicate = null)
        {
            total = 0;
            IQueryable<Entity> queryCount = Context.Set<Entity>();
            IQueryable<Entity> query = Context.Set<Entity>();
            if (predicate != null)
            {
                queryCount = queryCount.Where(predicate);
                query = query.Where(predicate);
            }
            if (!string.IsNullOrWhiteSpace(sort))
            {
                query = query.OrderBy<Entity>(sort, isAscending);
            }
            total = queryCount.Count();

            return query.Skip(page*pageSize).Take(pageSize).ToList();
        }
    }
}
