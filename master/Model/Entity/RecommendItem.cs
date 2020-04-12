using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    public class RecommendItem:SetBase
    {
        public int Sort { get; set; }
        public string Name { get; set; }
        public byte[] Image { get; set; }
        /// <summary>
        /// 图片类型
        /// </summary>
        public string ImageType { get; set; }
        public string LinkUrl { get; set; }
    }
}
