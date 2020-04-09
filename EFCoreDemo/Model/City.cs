using System;
using System.Collections.Generic;

namespace Model
{
    public class City
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string AreaCode { get; set; }
        public int ProvinceId { get; set; }
        public virtual Province Province { get; set; }
        public virtual Mayor Mayor { get; set; }
        public virtual List<CityCompany> CityCompanies { get; set; }
    }
}
