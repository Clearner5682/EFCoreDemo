using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    public class MoguGoodItem:SetBase
    {
        public string tradeItemId { get; set; }
        public int itemType { get; set; }
        /// <summary>
        /// 列表图片链接
        /// </summary>
        public string img { get; set; }
        /// <summary>
        /// 详情链接
        /// </summary>
        public string clientUrl { get; set; }
        /// <summary>
        /// 详情链接
        /// </summary>
        public string link { get; set; }
        public string itemMarks { get; set; }
        public string acm { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string title { get; set; }
        public int type { get; set; }
        public string cparam { get; set; }
        public decimal orgPrice { get; set; }
        public bool useTitle { get; set; }
        /// <summary>
        /// 价格
        /// </summary>
        public decimal price { get; set; }

    }
}
