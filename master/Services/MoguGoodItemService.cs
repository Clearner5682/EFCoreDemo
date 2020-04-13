using System;
using System.Collections.Generic;
using System.Text;
using Model;
using IRepository;
using IServices;

namespace Services
{
    public class MoguGoodItemService:BaseService<MoguGoodItem>,IMoguGoodItemService
    {
        public MoguGoodItemService(IMoguGoodItemRepository baseRepository):base(baseRepository)
        {

        }
    }
}
