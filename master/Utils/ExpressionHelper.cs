using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;
using System.Linq;

namespace Utils
{
    public static class ExpressionHelper
    {
        

        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source,string sortField,bool isAscending)
        {
            Type tType = typeof(T);
            PropertyInfo propertyInfo = tType.GetProperty(sortField);
            if (propertyInfo == null)
            {
                throw new Exception("排序字段错误");
            }
            string methodName = "OrderBy";
            if (!isAscending)
            {
                methodName = "OrderByDescending";
            }
            MethodInfo orderby = (typeof(Queryable)).GetMethods().FirstOrDefault(m => m.Name == methodName && m.GetParameters().Length == 2);
            if (orderby == null)
            {
                throw new Exception("找不到排序方法");
            }
            MethodInfo genericMethod = orderby.MakeGenericMethod(tType, propertyInfo.PropertyType);

            return (IOrderedQueryable<T>)genericMethod.Invoke(null, new object[] { source, GetLambdaExpression<T>(sortField) });
        }

        private static LambdaExpression GetLambdaExpression<T>(string sortField)
        {
            Type type = typeof(T);
            PropertyInfo propertyInfo = type.GetProperty(sortField);
            if (propertyInfo == null)
            {
                throw new Exception("排序字段错误");
            }
            ParameterExpression parameter = Expression.Parameter(type);
            Expression propertySelector = Expression.Property(parameter, propertyInfo);

            return Expression.Lambda(propertySelector, parameter);
        }
    }
}
