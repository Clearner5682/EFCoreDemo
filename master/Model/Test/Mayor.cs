using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    public class Mayor
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public EnumGender Gender { get; set; }
        public DateTime Birthday { get; set; }

        public int CityId { get; set; }
        public virtual City City { get; set; }
    }
}
