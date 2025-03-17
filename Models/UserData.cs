using Cassandra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace greatreads.Models
{
    public class User
    {
        public int ID_user;
        public String Email;
        public LocalDate Date_of_birth;
        public String Name_Surname;
        public String Password;
        public String Gender;
        public String Username;
    }

    public class UserData
    {
        public List<User> Users = new List<User>();

        public UserData()
        {
            try
            {
                var cluster = Cluster.Builder().AddContactPoint("127.0.0.1").Build();
                using var session = cluster.Connect("greatreads");
                var results = session.Execute("select * from users");
                User tmpObj = null;

                foreach (var result in results)
                {
                    tmpObj = new User();

                    tmpObj.ID_user = result.GetValue<int>("id_user");
                    tmpObj.Email = result.GetValue<string>("email");
                    tmpObj.Date_of_birth = result.GetValue<LocalDate>("date_of_birth");
                    tmpObj.Name_Surname = result.GetValue<string>("name_surname");
                    tmpObj.Password = result.GetValue<string>("password");
                    tmpObj.Gender = result.GetValue<string>("gender");
                    tmpObj.Username = result.GetValue<string>("username");

                    Users.Add(tmpObj);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
