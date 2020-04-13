﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
using Model;

namespace IRepository
{
    public interface IBaseRepository<Entity> where Entity:Base
    {
        IUnitOfWork GetUnitOfWork();
        void Add(Entity entity);
        void AddRange(ICollection<Entity> entities);
        void Delete(Entity entity);
        void DeleteById(object id);
        void Update(Entity entity);
        Entity GetById(object id);
        IList<Entity> GetAll();
        IList<Entity> SearchTop<EntityKey>(int count, Expression<Func<Entity, EntityKey>> keySelector,bool isAscending, Expression<Func<Entity, bool>> predicate=null);
    }
}
