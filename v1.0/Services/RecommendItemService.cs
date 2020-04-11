using System;
using System.Collections.Generic;
using System.Text;
using Model;
using IRepository;
using IServices;
using Database;

namespace Services
{
    public class RecommendItemService:BaseService<RecommendItem>,IRecommendItemService
    {
        IRecommendItemRepository _recommendItemRepository;

        public RecommendItemService(IRecommendItemRepository recommendItemRepository):base(recommendItemRepository)
        {
            _recommendItemRepository = recommendItemRepository;
        }
    }
}
