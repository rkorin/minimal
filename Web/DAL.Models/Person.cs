using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Models
{ 
    public class Person
    { 
        public int Id { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public String Address { get; set; }

        public String Note { get; set; }

        public int? ImageId { get; set; }
    }

}
