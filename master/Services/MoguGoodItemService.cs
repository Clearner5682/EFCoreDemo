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
            return _baseRepository.SearchPage(sort
                , true
                , page
                , pageSize
                , out total
                , o => o.type == type);
        }
    }
}
