using Cassandra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace greatreads.Models
{
    public class UserRating
    {
        public int ID_book;
        public int ID_user;
        public int Rating;
    }

    public class UserRatingData
    {
        public List<UserRating> Ocene = new List<UserRating>();

        public UserRatingData()
        {
            try
            {
                var cluster = Cluster.Builder().AddContactPoint("127.0.0.1").Build();
                using var session = cluster.Connect("greatreads");
                var results = session.Execute("select * from userratings");
                UserRating tmpObj = null;

                results.OrderBy(x => x.GetColumn("id_book"));
                foreach (var result in results)
                {
                    tmpObj = new UserRating();

                    tmpObj.ID_book = result.GetValue<int>("id_book");
                    tmpObj.ID_user = result.GetValue<int>("id_user");
                    tmpObj.Rating = result.GetValue<int>("rating");

                    Ocene.Add(tmpObj);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
