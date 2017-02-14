using DAL.Interfaces;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL
{
    public class PersonRepository : IRepository<Person>
    {
        public PersonRepository()
        {

        }
        PersonContext context = new PersonContext();
        public void delete(int id)
        {
            var first = context.Persons.FirstOrDefault(fe => fe.Id == id);
            if (first != null)
            {
                context.Persons.Remove(first);
                context.SaveChanges();
            }
        }

        public IEnumerable<Person> get()
        {
            return context.Persons.Select(s =>
                new Person
                {
                    Id = s.Id,
                    Address = s.Address,
                    DateOfBirth = s.DateOfBirth,
                    FirstName = s.FirstName,
                    //Image = null, // TO DO
                    LastName = s.LastName,
                    Note = s.Note
                });
        }

        public Person get(int id)
        {
            var s = context.Persons.FirstOrDefault(fe => fe.Id == id);
            if (s != null)
                return new Person
                {
                    Id = s.Id,
                    Address = s.Address,
                    DateOfBirth = s.DateOfBirth,
                    FirstName = s.FirstName,
                    //        Image = null, // TO DO
                    LastName = s.LastName,
                    Note = s.Note
                };
            return null;
        }

        public void save(Person entity)
        {
            var s = context.Persons.FirstOrDefault(fe => fe.Id == entity.Id);
            if (s != null)
            {
                s.FirstName = entity.FirstName;
                s.Address = entity.Address;
                s.DateOfBirth = entity.DateOfBirth;
                s.LastName = entity.LastName;
                s.Note = entity.Note;
                context.SaveChanges();
            }
            else
            {
                if (string.IsNullOrWhiteSpace(entity.FirstName) ||
                    string.IsNullOrWhiteSpace(entity.LastName)) return;
                context.Persons.Add(new Person
                {
                    LastName = entity.LastName,
                    Address = entity.Address,
                    DateOfBirth = entity.DateOfBirth,
                    FirstName = entity.FirstName,
                    ImageId = 0,
                    Note = entity.Note
                });
                context.SaveChanges();
            }
        }
    }
}

