using System;
using System.Collections.Generic;
using System.Text;

namespace IRepository
{
    public interface IBaseRepository<Entity> where Entity:class
    {
        void Add(Entity entity);
        void Delete(Entity entity);
        void DeleteById(object id);
        void Update(Entity entity);
        Entity GetById(object id);
    }
}
