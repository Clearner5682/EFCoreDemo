using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Model;
using IRepository;
using IServices;
using Utils;
using Microsoft.Data.SqlClient;
using System.Linq;

namespace Services
{
    public class MoguGoodItemService:BaseService<MoguGoodItem>,IMoguGoodItemService
    {
        IMoguGoodItemRepository _baseRepository;
        public MoguGoodItemService(IMoguGoodItemRepository baseRepository):base(baseRepository)
        {
            _baseRepository = baseRepository;
        }

        public async Task CollectGoodItemImage()// 采集商品的图片数据，每次取10个
        {
            var toCollectGoodItems = _baseRepository.SearchTop(10, o => o.CreateTime, true, o => !string.IsNullOrWhiteSpace(o.img) && o.Image == null);
            if (toCollectGoodItems.Count > 0)
            {
                foreach (var item in toCollectGoodItems)
                {
                    using (var httpClient = new HttpClient())
                    {
                        byte[] bytes = await httpClient.GetByteArrayAsync(item.img);
                        if (bytes != null && bytes.Length > 0)
                        {
                            item.Image = bytes;
                        }
                    }
                    Thread.Sleep(1000);
                }

                _baseRepository.UpdateRange(toCollectGoodItems);
            }
        }

        public IList<MoguGoodItem> GetPagedData(int type, string sort,bool isAscending, int page,int pageSize,out int total)
        {
            string countSql = @$"select count(1) from MoguGoodItems a where a.Type={type}";
            total = _baseRepository.SqlQuery<int>(countSql).Sum(o=>o);
            string orderby = "Order By " + sort;
            if (isAscending)
            {
                orderby += " asc";
            }
            else
            {
                orderby += " desc";
            }
            string sql = @$"select Id,title,price from
                        (
                        select ROW_NUMBER() OVER({orderby}) as RowIndex,
                        a.Id,
                        a.title,
                        a.price
                        from MoguGoodItems a
                        where a.Type=@Type
                        )T
                        where T.RowIndex>@StartRowIndex and T.RowIndex<=@EndRowIndex";
            SqlParameter[] parameters = new SqlParameter[] 
            { 
                new SqlParameter { ParameterName="Type",Value=type },
                new SqlParameter { ParameterName="StartRowIndex",Value=(page-1)*pageSize },
                new SqlParameter { ParameterName = "EndRowIndex", Value = page * pageSize } 
            };

            return _baseRepository.SqlQuery<MoguGoodItem>(sql,parameters);
        }
    }
}
