using Cassandra;
using greatreads.Models;
using Microsoft.AspNetCore.Mvc;

namespace greatreads.Controllers
{
    public class AuthorDataController : Controller
    {
        Cassandra.ISession session = null;

        public AuthorDataController()
        {
            try
            {
                var cluster = Cluster.Builder().AddContactPoint("127.0.0.1").Build();
                session = cluster.Connect("greatreads");
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public IActionResult DisplayAuthorData()
        {
            AuthorData ad = new AuthorData();
            return View(ad);
        }

        public IActionResult CreateAuthor(int id_author)
        {
            if (id_author != 0)
            {
                Author tmpObj = null;

                try
                {
                    var results = session.Execute("select * from author where id_author =" + id_author + ";");
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
                    }
                }
                catch (Exception e)
                { }

                return View(tmpObj);
            }
            else return View();
        }

        private Boolean CheckAuthor(Author author)
        {            
            if (author.ID_author == 0)
                return false;
            return true;
        }

        public IActionResult SaveAuthor(int id_author, String name_surname, DateTime date_of_birth, DateTime date_of_death, String country, int number_of_books, float rating, String biography)
        {
            Author tmpObj = new Author();

            tmpObj.ID_author = id_author;
            tmpObj.Name_Surname = name_surname;
            tmpObj.Date_of_birth = new LocalDate(date_of_birth.Year, date_of_birth.Month, date_of_birth.Day);
            tmpObj.Date_of_death = new LocalDate(date_of_death.Year, date_of_death.Month, date_of_death.Day);
            tmpObj.Country = country;
            tmpObj.Number_of_books = number_of_books;
            tmpObj.Biography = biography;
            tmpObj.Rating = rating;

            if (!CheckAuthor(tmpObj))
                return RedirectToAction("DisplayAuthorData");

            LocalDate ld1 = null;
            LocalDate minld1 = new LocalDate(1, 1, 1);
            if (tmpObj.Date_of_death == minld1)
                ld1 = new LocalDate(9999, 12, 31);
            else
                ld1 = new LocalDate(tmpObj.Date_of_death.Year, tmpObj.Date_of_death.Month, tmpObj.Date_of_death.Day);

            LocalDate ld2 = null;
            LocalDate minld2 = new LocalDate(1, 1, 1);
            if (tmpObj.Date_of_birth == minld2)
                ld2 = new LocalDate(1000, 10, 10);
            else
                ld2 = new LocalDate(tmpObj.Date_of_birth.Year, tmpObj.Date_of_birth.Month, tmpObj.Date_of_birth.Day);

            try
            {
                session.Execute("insert into autor (id_author, name_surname, date_of_birth, date_of_death, country," +
                                "number_of_books, rating, biography)" +
                                "values (" + tmpObj.ID_author +
                                ", \'" + tmpObj.Name_Surname + "\'" +
                                ", \'" + ld2.ToString() + "\'" +
                                ", \'" + ld1.ToString() + "\'" +
                                ", \'" + tmpObj.Country + "\'" +
                                ", " + tmpObj.Number_of_books +
                                ", " + tmpObj.Rating +
                                ", \'" + tmpObj.Biography + "\');");
            }
            catch (Exception e)
            { }

            return RedirectToAction("DisplayAuthorData");
        }

        public IActionResult UpdateAuthor(int id_author)
        {
            return RedirectToAction("CreateAuthor", new { id_author = id_author });
        }

        public IActionResult AddAuthorBook(int id_author)
        {
            return View(id_author);
        }
    }
}
