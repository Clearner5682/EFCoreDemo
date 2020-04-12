using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    public class Province
    {
        public Province()
        {
            Cities = new List<City>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int Population { get; set; }
        public virtual List<City> Cities { get; set; }
    }
}
