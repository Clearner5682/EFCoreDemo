using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Exceptions
{
    public class UnAuthorizedException:Exception
    {
        public int ErrorCode { get; set; } = 401;
    }
}
