using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime EstablishDate { get; set; }
        public virtual List<CityCompany> CityCompanies { get; set; }
    }
}
