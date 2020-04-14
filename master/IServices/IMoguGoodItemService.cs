using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace IServices
{
    public interface IMoguGoodItemService:IBaseService<MoguGoodItem>
    {
        Task CollectGoodItemImage();
        IList<MoguGoodItem> GetPagedData(int type, string sort,bool isAscending, int page, int pageSize, out int total);
    }
}
