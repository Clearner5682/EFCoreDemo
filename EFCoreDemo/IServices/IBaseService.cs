using System;
using System.Collections.Generic;
using System.Text;

namespace IServices
{
    public interface IBaseService<Entity> where Entity:class
    {
        void Add(Entity entity);
        void Delete(Entity entity);
        void DeleteById(object id);
        void Update(Entity entity);
        Entity GetById(object id);
    }
}
