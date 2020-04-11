using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Web.ViewModel
{
    public class RecommendItemViewModel
    {
        public string Name { get; set; }
        public IFormFile Image { get; set; }
    }
}
