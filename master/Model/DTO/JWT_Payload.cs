using System;
using System.Collections.Generic;
using System.Text;

namespace Model.DTO
{
    public class JWT_Payload
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Phone { get; set; }
        public long nbf { get; set; }
        public long exp { get; set; }
        public string iss { get; set; }
        public string aud { get; set; }
    }
}
