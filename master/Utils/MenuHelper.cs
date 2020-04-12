using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System.Collections.Generic;
using Model.DTO;

namespace Utils
{
    public class MenuHelper
    {
        private static IConfiguration _configuration;
        private static IList<MenuInfo> _menuInfoList;
        private static IList<ActionInfo> _actionInfoList;

        static MenuHelper()// 最先执行静态构造函数
        {
            if (_menuInfoList == null)
            {
                _menuInfoList = new List<MenuInfo>();
            }
            if (_actionInfoList == null)
            {
                _actionInfoList = new List<ActionInfo>();
            }
        }

        public MenuHelper(string menuPath)
        {
            string fileName = "Menu.json";
            _configuration = new ConfigurationBuilder()
                .SetBasePath(menuPath)
                .Add(new JsonConfigurationSource { Path=fileName,Optional=false,ReloadOnChange=true})
                .Build();
            LoadMenu();
        }

        private void LoadMenu()
        {
            var groups = _configuration.GetSection("Groups").GetChildren();
            foreach(IConfigurationSection groupConfig in groups)
            {
                var groupName = groupConfig["GroupName"];
                var menus = groupConfig.GetSection("Menus").GetChildren();
                foreach(IConfigurationSection menuConfig in menus)
                {
                    MenuInfo menuInfo = new MenuInfo();
                    menuInfo.Actions = new List<ActionInfo>();
                    menuInfo.ClsName = menuConfig["ClsName"];
                    menuInfo.MenuNum = Convert.ToInt32(menuConfig["MenuNum"]);
                    menuInfo.MenuName = menuConfig["MenuName"];
                    menuInfo.MenuUrl = menuConfig["MenuUrl"];
                    IEnumerable<IConfigurationSection> actions = menuConfig.GetSection("Actions").GetChildren();
                    if (actions != null)
                    {
                        foreach(IConfigurationSection actionConfig in actions)
                        {
                            ActionInfo actionInfo = new ActionInfo();
                            actionInfo.ActionNum =menuInfo.MenuNum+ Convert.ToInt32(actionConfig["ActionIndex"]);
                            actionInfo.ActionName = actionConfig["ActionName"];
                            actionInfo.ActionUrl = actionConfig["ActionUrl"];
                            

                            menuInfo.Actions.Add(actionInfo);
                            _actionInfoList.Add(actionInfo);
                        }
                    }

                    _menuInfoList.Add(menuInfo);
                }
            }
        }

        public static IList<MenuInfo> GetMenuInfos()
        {
            return _menuInfoList;
        }

        public static IList<ActionInfo> GetActionInfos()
        {
            return _actionInfoList;
        }
    }
}
