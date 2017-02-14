using DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL
{
    public class PersonContext : DbContext
    {
        public PersonContext() : base()
        {
        }

        public DbSet<Person> Persons { get; set; }
        //public DbSet<PersonImage> PersonImages { get; set; }
    }

}
