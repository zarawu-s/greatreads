using Cassandra;
using greatreads.Models;
using Microsoft.AspNetCore.Mvc;

namespace greatreads.Controllers
{
    public class UserRatingsDataController : Controller
    {
        Cassandra.ISession session = null;

        public UserRatingsDataController()
        {
            try
            {
                var cluster = Cluster.Builder().AddContactPoint("127.0.0.1").Build();
                session = cluster.Connect("greatreads");
            }
            catch (Exception)
            { }
        }

        public IActionResult DisplayUserRatingsData()
        {
            UserRatings ur = new UserRatings();
            return View(ur);
        }

        public Boolean CheckRating(UserRatings rating)
        {
            if (rating.ID_book == 0 || rating.ID_user == 0)
                return false;
            return true;
        }

        public IActionResult DeleteRating(int id_book, int id_user)
        {
            try
            {
                session.Execute("delete from userratings where " +
                                "id_book =" + id_book + " and " +
                                "id_user =" + id_user + ";");
            }
            catch (Exception) { }

            CalculateRating(id_book);
            CalculateAuthorRating(id_book);
            return RedirectToAction("DisplayUserRatingsData");
        }

        public IActionResult GoToCreateRating(int id_book, int id_user, int rating)
        {
            UserRatings tmpObj = null;

            tmpObj = new UserRatings();

            tmpObj.ID_book = id_book;
            tmpObj.ID_user = id_user;
            tmpObj.Rating = rating;

            return View(tmpObj);
        }

        public IActionResult SaveRating(int id_book, int id_user, int rating)
        {
            UserRatings tmpObj = new UserRatings();

            tmpObj.ID_book = id_book;
            tmpObj.ID_user = id_user;
            tmpObj.Rating = rating;

            if (!CheckRating(tmpObj))
                return RedirectToAction("DisplayUserRatingsData");

            try
            {
                session.Execute("insert into userratings (id_book, id_user, rating) " +
                    "values (" + tmpObj.ID_book +
                    ", " + tmpObj.ID_user +
                    ", " + tmpObj.Rating + ");");
            }
            catch (Exception) { }

            CalculateRating(id_book);
            CalculateAuthorRating(id_book);;

            return RedirectToAction("DisplayUserRatingsData");
        }

        private void CalculateRating(int id_book)
        {
            float sum = 0, n = 0;

            try
            {
                var ratings = session.Execute("select rating from userratings where id_book=" + id_book + ";");

                foreach (var r in ratings)
                {
                    int rating = r.GetValue<int>(0);
                    sum += rating;
                    n++;
                }
                float fin_rating = sum / n;

                session.Execute("update books set rating=" +
                                fin_rating.ToString("0.000", System.Globalization.CultureInfo.InvariantCulture) +
                                " where id_book=" + id_book + ";");
            }
            catch (Exception) { }
        }

        public void CalculateAuthorRating(int id_book)
        {
            float sum = 0, n = 0;
            try
            {
                var author = session.Execute("select id_author from authorbooks where id_book=" + id_book + ";");

                int id_author = author.First().GetValue<int>("id_author");

                var books = session.Execute("select id_book from authorbooks where id_author=" + id_author + ";");

                foreach (var book in books)
                {
                    int id = book.GetValue<int>(0);

                    var result = session.Execute("select rating from books where id_book=" + id + ";").FirstOrDefault();

                    float rating = result.GetValue<float>(0);

                    sum += rating;
                    n++;
                }

                float fin_rating = sum / n;

                session.Execute("update authors set rating=" +
                                fin_rating.ToString("0.000", System.Globalization.CultureInfo.InvariantCulture) +
                                " where id_author=" +
                                id_author + ";");
            }
            catch (Exception) { }
        }
    }
}
