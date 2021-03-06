﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace IServices
{
    public interface IBaseService<Entity> where Entity:class
    {
        void Add(Entity entity);
        void AddRange(ICollection<Entity> entities);
        void Delete(Entity entity);
        void DeleteById(object id);
        void Update(Entity entity);
        void UpdateRange(ICollection<Entity> entities);
        Entity GetById(object id);
        IList<Entity> GetAll();
        IList<Entity> SearchTop<EntityKey>(int count, Expression<Func<Entity, EntityKey>> keySelector, bool isAscending, Expression<Func<Entity, bool>> predicate = null);
        IList<Entity> SearchPage(string sort, bool isAscending, int page, int pageSize,out int total, Expression<Func<Entity, bool>> predicate = null);
    }
}
