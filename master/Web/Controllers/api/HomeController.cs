using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using IServices;
using Model;
using Utils;
using Web.ViewModel;
using Newtonsoft.Json.Linq;
using Microsoft.CSharp.RuntimeBinder;
using System.Text.Json;

namespace Web.Controllers.api
{
    public class HomeController : Controller
    {
        ISwiperItemService _swiperItemService;
        IRecommendItemService _recommendItemService;
        public HomeController(ISwiperItemService swiperItemService,IRecommendItemService recommendItemService)
        {
            _swiperItemService = swiperItemService;
            _recommendItemService = recommendItemService;
        }

        [HttpPost]
        public IActionResult AddSwiperItem(IFormCollection formCollection)// 上传图片
        {
            if (formCollection.Files.Count == 0)
            {
                return Ok(new { ErrorCode = "9999", Message = "请选择文件" });
            }
            using (Stream stream=formCollection.Files[0].OpenReadStream())
            {
                SwiperItem model = new SwiperItem();
                model.Image = stream.ToByteArray();
                _swiperItemService.Add(model);
            }

            return Ok(new { ErrorCode="0"});
        }

        [HttpGet]
        public IActionResult GetSwiperItemImage(Guid Id)// 下载图片
        {
            var model = _swiperItemService.GetById(Id);
            if (model == null)
            {
                return Ok(new { ErrorCode = "9999", Message = "获取轮播项目失败" });
            }

            return File(model.Image, "image/png");
        }

        [HttpGet]
        public IActionResult GetSwiperItemList()// 获取轮播图集合
        {
            string hostUrl = Request.Host.Value;
            if (!hostUrl.StartsWith("http://"))
            {
                hostUrl = "http://" + hostUrl;
            }
            if (!hostUrl.EndsWith("/"))
            {
                hostUrl += "/";
            }
            string GetImageUrl(Guid Id)
            {
                return hostUrl + "api/Home/GetSwiperItemImage/" + Id.ToString();
            }

            return Ok(
                _swiperItemService
                .GetAll()
                .OrderBy(o=>o.CreateTime)
                .Select(o=>new { Id=o.Id,ImageUrl=GetImageUrl(o.Id),LinkUrl=o.LinkUrl})
                );
        }

        [HttpPost]
        public IActionResult AddRecommendItem(RecommendItemViewModel viewModel)
        {
            if (viewModel == null)
            {
                return Ok(new { ErrorCode = "9999", Message = "推荐项目不能为空" });
            }
            if (viewModel.Image == null)
            {
                return Ok(new { ErrorCode = "9999", Message = "推荐项目的图片不能为空" });
            }
            if (viewModel.RecommendType != 0 && viewModel.RecommendType != 1)
            {
                return Ok(new { ErrorCode = "9999", Message = "推荐类型错误" });
            }
            var model = new RecommendItem();
            model.Name = viewModel.Name;
            model.RecommendType = (EnumRecommendType)viewModel.RecommendType;
            using (Stream stream = viewModel.Image.OpenReadStream())
            {
                model.Image = stream.ToByteArray();
                model.ImageType = viewModel.Image.ContentType;
            }
            _recommendItemService.Add(model);

            return Ok(new { ErrorCode="0"});
        }

        [HttpGet]
        public IActionResult GetRecommendItemList(EnumRecommendType recommendType,int count)
        {
            //[FromBody]JsonElement data
            //int count = data.GetProperty("count").GetInt32();
            string hostUrl = Request.Host.Value;
            if (!hostUrl.StartsWith("http://"))
            {
                hostUrl = "http://" + hostUrl;
            }
            if (!hostUrl.EndsWith("/"))
            {
                hostUrl += "/";
            }
            string GetImageUrl(Guid Id)
            {
                return hostUrl + "api/Home/GetRecommendItemImage/" + Id.ToString();
            }
            var list = _recommendItemService.SearchTop(count, o => o.Sort,true,o=>o.RecommendType==recommendType);

            return Ok(list.Select(o=>new { Id=o.Id,Name=o.Name,ImageUrl=GetImageUrl(o.Id),LinkUrl=o.LinkUrl}));
        }

        [HttpGet]
        public IActionResult GetRecommendItemImage(Guid id)
        {
            var model = _recommendItemService.GetById(id);
            if (model == null)
            {
                return Ok(new { ErrorCode = "9999", Message = "获取推荐项目失败" });
            }

            return File(model.Image, model.ImageType);
        }
    }
}