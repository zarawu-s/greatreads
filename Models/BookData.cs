using Cassandra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace greatreads.Models
{
    public class Book
    {
        public int ID_book;
        public String Title;
        public String Original_title;
        public String Author;
        public String Format;
        public String Language;
        public String Genre;
        public int Number_of_pages;
        public float Rating;
        public LocalDate Date_published;
        public String ISBN;
    }

    public class BookData
    {
        public List<Book> Books = new List<Book>();

        public BookData() 
        {
            try
            {
                var cluster = Cluster.Builder().AddContactPoint("127.0.0.1").Build();
                using var session = cluster.Connect("greatreads");
                var results = session.Execute("select * from books");
                Book tmpObj = null;

                foreach (var result in results)
                {
                    tmpObj = new Book();

                    tmpObj.ID_book = result.GetValue<int>("id_book");
                    tmpObj.Title = result.GetValue<String>("title");
                    tmpObj.Original_title = result.GetValue<String>("original_title");
                    tmpObj.Author = result.GetValue<String>("author");
                    tmpObj.Number_of_pages = result.GetValue<int>("number_of_pages");
                    tmpObj.Format = result.GetValue<String>("format");
                    tmpObj.Date_published = result.GetValue<LocalDate>("date_published");
                    tmpObj.Language = result.GetValue<String>("language");
                    tmpObj.Rating = result.GetValue<float>("rating");
                    tmpObj.Genre = result.GetValue<String>("genre");
                    tmpObj.ISBN = result.GetValue<String>("isbn");

                    Books.Add(tmpObj);
                }
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

    }
}
