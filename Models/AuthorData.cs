using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cassandra;
using greatreads.Controllers;

namespace greatreads.Models
{
    public class Author
    {
        public int ID_author;
        public String Name_Surname;
        public LocalDate Date_of_birth;
        public LocalDate Date_of_death;
        public String Country;
        public String Biography;
        public int Number_of_books;
        public float Rating;
    }

    public class AuthorData
    {
        public List<Author> Authors = new List<Author>();

        public AuthorData()
        {
            try
            {
                var cluster = Cluster.Builder().AddContactPoint("127.0.0.1").Build();
                using var session = cluster.Connect("greatreads");
                var results = session.Execute("select * from authors");
                Author tmpObj = null;

                foreach (var result in results)
                {
                    tmpObj = new Author();
                    tmpObj.ID_author = result.GetValue<int>("id_author");
                    tmpObj.Name_Surname = result.GetValue<String>("name_surname");
                    tmpObj.Date_of_birth = result.GetValue<LocalDate>("date_of_birth");
                    tmpObj.Date_of_death = result.GetValue<LocalDate>("date_of_death");
                    tmpObj.Country = result.GetValue<String>("country");
                    tmpObj.Biography = result.GetValue<String>("biography");
                    tmpObj.Number_of_books = result.GetValue<int>("number_of_books");
                    tmpObj.Rating = result.GetValue<float>("rating");
             
                    Authors.Add(tmpObj);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}