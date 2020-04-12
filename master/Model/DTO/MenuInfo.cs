using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Model.DTO
{
    public class MenuInfo
    {
        /// <summary>
        /// 菜单分组
        /// </summary>
        public string GroupName { get; set; }
        /// <summary>
        /// 二级分组
        /// </summary>
        public string ClsName { get; set; }
        /// <summary>
        /// 菜单编号
        /// </summary>
        public int MenuNum { get; set; }
        /// <summary>
        /// 菜单名称
        /// </summary>
        public string MenuName { get; set; }
        /// <summary>
        /// 菜单URL
        /// </summary>
        public string MenuUrl{ get; set; }
        /// <summary>
        /// 操作
        /// </summary>
        public IList<ActionInfo> Actions { get; set; }
    }
}
