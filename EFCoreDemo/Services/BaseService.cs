using System;
using System.Collections.Generic;
using System.Text;
using IRepository;
using IServices;

namespace Services
{
    public class BaseService<Entity> : IBaseService<Entity> where Entity : class
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

        public void Update(Entity entity)
        {
            BaseRepository.Update(entity);
        }
    }
}
