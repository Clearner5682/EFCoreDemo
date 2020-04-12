using System;
using System.Collections.Generic;
using System.Text;
using IRepository;
using Model;

namespace Repository
{
    public class SwiperItemRepository:BaseRepository<SwiperItem>,ISwiperItemRepository
    {
        public SwiperItemRepository(IUnitOfWork unitOfWork):base(unitOfWork)
        {

        }
    }
}
