using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
using Model;
using IRepository;
using IServices;

namespace Services
{
    public class BaseService<Entity> : IBaseService<Entity> where Entity : Base
    {
        protected IBaseRepository<Entity> BaseRepository;
        public BaseService(IBaseRepository<Entity> baseRepository)
        {
            BaseRepository = baseRepository;
        }

        public void Add(Entity entity)
        {
            BaseRepository.Add(entity);
        }

        public void AddRange(ICollection<Entity> entities)
        {
            BaseRepository.AddRange(entities);
        }

        public void Delete(Entity entity)
        {
            BaseRepository.Delete(entity);
        }

        public void DeleteById(object id)
        {
            BaseRepository.DeleteById(id);
        }

        public Entity GetById(object id)
        {
            return BaseRepository.GetById(id);
        }

        public IList<Entity> GetAll()
        {
            return BaseRepository.GetAll();
        }

        public void Update(Entity entity)
        {
            BaseRepository.Update(entity);
        }

        public void UpdateRange(ICollection<Entity> entities)
        {
            BaseRepository.UpdateRange(entities);
        }

        public IList<Entity> SearchTop<EntityKey>(int count, Expression<Func<Entity, EntityKey>> keySelector,bool isAscending, Expression<Func<Entity, bool>> predicate=null)
        {
            return BaseRepository.SearchTop(count, keySelector,isAscending,predicate);
        }

        public IList<Entity> SearchPage(string sort, bool isAscending, int page, int pageSize,out int total, Expression<Func<Entity, bool>> predicate = null)
        {
            return BaseRepository.SearchPage(sort, isAscending, page, pageSize, out total,predicate);
        }
    }
}
