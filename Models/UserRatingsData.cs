using Cassandra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace greatreads.Models
{
    public class UserRatings
    {
        public int ID_book;
        public int ID_user;
        public int Rating;
    }

    public class UserRatingsData
    {
        public List<UserRatings> Ratings = new List<UserRatings>();

        public UserRatingsData()
        {
            try
            {
                var cluster = Cluster.Builder().AddContactPoint("127.0.0.1").Build();
                using var session = cluster.Connect("greatreads");
                var results = session.Execute("select * from userratings");
                UserRatings tmpObj = null;

                results.OrderBy(x => x.GetColumn("id_book"));
                foreach (var result in results)
                {
                    tmpObj = new UserRatings();

                    tmpObj.ID_book = result.GetValue<int>("id_book");
                    tmpObj.ID_user = result.GetValue<int>("id_user");
                    tmpObj.Rating = result.GetValue<int>("rating");

                    Ratings.Add(tmpObj);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
