using Cassandra;
using greatreads.Models;
using Microsoft.AspNetCore.Mvc;

namespace greatreads.Controllers
{
    public class AuthorBooksDataController : Controller
    {
        Cassandra.ISession session = null;

        public AuthorBooksDataController()
        {
            try
            {
                var cluster = Cluster.Builder().AddContactPoint("127.0.0.1").Build();
                session = cluster.Connect("greatreads");
            }
            catch (Exception) { }
        }

        public IActionResult DisplayAuthorBooksData()
        {
            AuthorBooksData abd = new AuthorBooksData();
            return View(abd);
        }

        public Boolean CheckBook(AuthorBooks authorbook)
        {
            if (authorbook.ID_book == 0 || authorbook.ID_author == 0)
                return false;
            return true;
        }

        public IActionResult DeleteAuthorBook(int id_book, int id_author)
        {
            try
            {
                session.Execute("delete from authorbooks where " +
                                "id_book =" + id_book + " and " +
                                "id_author =" + id_author + ";");
            }
            catch (Exception) { }

            int number_of_books;

            var nob = session.Execute("select number_of_books from authors where id_author=" + id_author + ";");
            number_of_books = nob.First().GetValue<int>(0);
            number_of_books--;

            session.Execute("update authors set number_of_books=" + number_of_books + " where id_author=" + id_author + ";");

            return RedirectToAction("DisplayAuthorBooksData");
        }

        public IActionResult GoToCreateAuthorBook(int id_book, int id_author)
        {
            if (id_book != 0 && id_author != 0)
            {
                AuthorBooks tmpObj = null;

                try
                {
                    var results = session.Execute("select * from authorbooks where " +
                                  "id_book =" + id_book + " and " +
                                  "id_author = " + id_author + ";");

                    foreach (var result in results)
                    {
                        tmpObj = new AuthorBooks();

                        tmpObj.ID_book = result.GetValue<int>("id_book");
                        tmpObj.ID_author = result.GetValue<int>("id_author");
                        tmpObj.Title = result.GetValue<String>("title");
                    }
                }
                catch (Exception) { }

                return View(tmpObj);
            }

            return View();
        }

        public IActionResult SaveAuthorBook(int id_book, int id_author)
        {
            AuthorBooks tmpObj = new AuthorBooks();

            tmpObj.ID_book = id_book;
            tmpObj.ID_author = id_author;

            if (!CheckBook(tmpObj))
                return RedirectToAction("DisplayAuthorBooksData");

            var book = session.Execute("select title from books where id_book=" + id_book + ";").First();

            tmpObj.Title = book.GetValue<String>(0);

            try
            {
                session.Execute("insert into authorbooks (id_book, id_author, title) " +
                    "values (" + tmpObj.ID_book +
                    ", " + tmpObj.ID_author +
                    ", \'" + tmpObj.Title + "\');");
            }
            catch (Exception) { }

            int number_of_books;

            var nob = session.Execute("select number_of_books from author where id_author=" + id_author + ";");
            number_of_books = nob.First().GetValue<int>(0);
            number_of_books++;

            session.Execute("update authors set number_of_books=" + number_of_books+ " where id_author=" + id_author + ";");

            return RedirectToAction("DisplayAuthorBooksData");
        }
    }
}
