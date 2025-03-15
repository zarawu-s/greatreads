using Cassandra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace greatreads.Models
{
    public class Shelf
    {
        public int ID_shelf;
        public String Name;
        public int Number_of_books;
    }

    public class ShelfData
    {
        public List<Shelf> Shelves = new List<Shelf>();
        public int ID_book;
        public ShelfData()
        {
            try
            {
                var cluster = Cluster.Builder().AddContactPoint("127.0.0.1").Build();
                using var session = cluster.Connect("greatreads");
                var results = session.Execute("select * from shelves");
                Shelf tmpObj = null;

                foreach (var result in results)
                {
                    tmpObj = new Shelf();

                    tmpObj.ID_shelf = result.GetValue<int>("id_shelf");
                    tmpObj.Name = result.GetValue<String>("name");
                    tmpObj.Number_of_books = result.GetValue<int>("number_of_books");

                    Shelves.Add(tmpObj);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }

}
