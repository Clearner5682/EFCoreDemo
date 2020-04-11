using System;
using System.Collections.Generic;
using System.Text;
using IRepository;
using IServices;
using Model;

namespace Services
{
    public class SwiperItemService:BaseService<SwiperItem>,ISwiperItemService
    {
        ISwiperItemRepository _repository;
        public SwiperItemService(ISwiperItemRepository repository) :base(repository)
        {
            _repository = repository;
        }
    }
}
