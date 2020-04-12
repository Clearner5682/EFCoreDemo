using System;
using System.Collections.Generic;
using System.Text;
using Model;
using IRepository;

namespace Repository
{
    public class RecommendItemRepository:BaseRepository<RecommendItem>,IRecommendItemRepository
    {
        public RecommendItemRepository(IUnitOfWork unitOfWork):base(unitOfWork)
        {

        }
    }
}
