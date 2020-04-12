using System;
using System.Collections.Generic;
using System.Text;

namespace Model.DTO
{
    public class WXLoginResult
    {
        public string openid { get; set; }
        public string session_key { get; set; }
        public string unionid { get; set; }
        public int errcode { get; set; }
        public string errmsg { get; set; }
    }
}
