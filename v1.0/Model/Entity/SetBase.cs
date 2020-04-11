using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    public class SetBase:Base
    {
        public DateTime CreateTime { get; set; }
        public DateTime LastUpdateTime { get; set; }
    }
}
