using Cassandra;
using greatreads.Models;
using Microsoft.AspNetCore.Mvc;

namespace greatreads.Controllers
{
    public class UserDataController : Controller
    {
        Cassandra.ISession session = null;

        public UserDataController()
        {
            try
            {
                var cluster = Cluster.Builder().AddContactPoint("127.0.0.1").Build();
                session = cluster.Connect("greatreads");
            }
            catch (Exception)
            { }
        }

        public IActionResult DisplayUserData()
        {
            UserData kor = new UserData();
            return View(kor);
        }

        public IActionResult GoToCreateUser(int id_user)
        {
            if (id_user != 0)
            {
                User user = null;
                try
                {
                    var results = session.Execute("select * from users where " +
                                    "id_user =" + id_user + " ; ");

                    foreach (var result in results)
                    {
                        user = new User();

                        user.ID_user = result.GetValue<int>("id_user");
                        user.Email = result.GetValue<string>("email");
                        user.Date_of_birth = result.GetValue<LocalDate>("date_of_birth");
                        user.Name_Surname = result.GetValue<string>("name_surname");
                        user.Password = result.GetValue<string>("password");
                        user.Gender = result.GetValue<string>("gender");
                        user.Username = result.GetValue<string>("username");
                    }
                }
                catch (Exception) { }
                return View(user);
            }
            else
                return View();
        }

        public Boolean CheckUser(User user)
        {
            if (user.ID_user == 0)
                return false;
            return true;
        }

        public IActionResult SaveUser(int id_user, String email, DateTime date_of_birth, String name_surname, String password, String gender, String username)
        {
            User user = new User();
            user.ID_user = id_user;
            user.Email = email;
            user.Date_of_birth= new LocalDate(date_of_birth.Year, date_of_birth.Month, date_of_birth.Day);
            user.Name_Surname = name_surname;
            user.Password = password;
            user.Gender = gender;
            user.Username = username;

            if (!CheckUser(user))
                return RedirectToAction("DisplayUserData");

            LocalDate ld = null;
            LocalDate minld = new LocalDate(1, 1, 1);
            if (user.Date_of_birth == minld)
                ld = new LocalDate(1000, 10, 10);
            else
                ld = new LocalDate(user.Date_of_birth.Year, user.Date_of_birth.Month, user.Date_of_birth.Day);

            try
            {
                session.Execute("insert into users(id_user, email, date_of_birth, name_surname, password, gender, username)" +
                                "values (" + user.ID_user +
                                ", \'" + user.Email + "\'" +
                                ", \'" + ld.ToString() + "\'" +
                                ", \'" + user.Name_Surname + "\'" +
                                ", \'" + user.Password + "\'" +
                                ", \'" + user.Gender + "\'" +
                                ",\' " + user.Username + "\');");
            }
            catch (Exception) { }
            return RedirectToAction("DisplayUserData");
        }

        public IActionResult DeleteUser(int id_user)
        {
            try
            {
                session.Execute("delete from users where " +
                                "id_user = " + id_user + " ; ");
            }
            catch (Exception) { }

            return RedirectToAction("DisplayUserData");
        }

        public IActionResult UpdateUser(int id_user)
        {
            return RedirectToAction("GoToCreateUser", new { id_user = id_user });
        }

        public IActionResult ShowShelves(int id_user)
        {
            return RedirectToAction("DisplayShelfData", "Shelf", new { id_user = id_user });
        }
    }
}
