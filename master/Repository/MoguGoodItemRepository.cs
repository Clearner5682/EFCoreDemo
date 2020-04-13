using System;
using System.Collections.Generic;
using System.Text;
using Model;
using IRepository;

namespace Repository
{
    public class MoguGoodItemRepository:BaseRepository<MoguGoodItem>,IMoguGoodItemRepository
    {
        public MoguGoodItemRepository(IUnitOfWork unitOfWork):base(unitOfWork)
        {

        }
    }
}
