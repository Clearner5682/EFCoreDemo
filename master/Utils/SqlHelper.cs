using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Reflection;

namespace Utils
{
    public static class SqlHelper
    {
        public static IList<T> ToList<T>(this DataTable tb)
        {
            var list = new List<T>();
            Type type = typeof(T);
            PropertyInfo[] propertyInfos = type.GetProperties();
            foreach(DataRow row in tb.Rows)
            {
                var obj = type.Assembly.CreateInstance(type.FullName);
                foreach(DataColumn column in tb.Columns)
                {
                    if (row[column] != DBNull.Value) 
                    {
                        if (type.IsValueType)// 值类型int,string,datetime等
                        {
                            list.Add((T)(row[column]));
                        }
                        else
                        {
                            PropertyInfo currProperty = propertyInfos.FirstOrDefault(o => o.Name == column.ColumnName);
                            if (currProperty != null)
                            {
                                currProperty.SetValue(obj, row[column]);
                            }
                        }
                    }
                }

                list.Add((T)obj);
            }

            return list;
        }
    }
}
