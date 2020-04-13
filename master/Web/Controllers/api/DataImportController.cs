using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using Utils;
using Microsoft.AspNetCore.Http;
using System.IO;
using Model;
using IServices;

namespace Web.Controllers.api
{
    public class DataImportController : Controller
    {
        IMoguGoodItemService _moguGoodItemService;
        public DataImportController(IMoguGoodItemService moguGoodItemService)
        {
            _moguGoodItemService = moguGoodItemService;
        }

        /// <summary>
        /// 上传蘑菇街商品列表数据
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult ImportMoguGoodItems([FromForm]IFormFile file)
        {
            if (file == null)
            {
                return Ok(new { ErrorCode = "9999", Message = "没有上传文件" });
            }
            if (file.ContentType != "application/json")
            {
                return Ok(new { ErrorCode = "9999", Message = "不是JSON文件" });
            }
            string json = "";
            using (var stream = file.OpenReadStream())
            {
                using (var sr = new StreamReader(stream))
                {
                    json = sr.ReadToEnd();
                }
            }
            if (string.IsNullOrWhiteSpace(json))
            {
                return Ok(new { ErrorCode = "9999", Message = "内容为空" });
            }
            IList<MoguGoodItem> list = JsonHelper.GetEntity<IList<MoguGoodItem>>(json, "result.wall.docs");
            _moguGoodItemService.AddRange(list);

            return Ok(new { ErrorCode="0"});
        }
    }
}