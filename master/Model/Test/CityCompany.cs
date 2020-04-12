using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    public class CityCompany
    {
        public int Id { get; set; }
        public int CityId { get; set; }
        public virtual City City { get; set; }
        public int CompanyId { get; set; }
        public virtual Company Company { get; set; }
    }
}
