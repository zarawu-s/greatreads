using Cassandra;
using greatreads.Models;
using Microsoft.AspNetCore.Mvc;

namespace greatreads.Controllers
{
    public class BookDataController : Controller
    {
        Cassandra.ISession session = null;

        public BookDataController()
        {
            try
            {
                var cluster = Cluster.Builder().AddContactPoint("127.0.0.1").Build();
                session = cluster.Connect("greatreads");
            }
            catch (Exception) { }
        }

        public IActionResult DisplayBookData()
        {
            BookData bd = new BookData();
            return View(bd);
        }

        public IActionResult GoToCreateBook(int id_book)
        {
            if (id_book != 0)
            {
                Book tmpObj = null;

                try
                {
                    var results = session.Execute("select * from books where " +
                                    "id_book =" + id_book + " ; ");

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
                    }
                }
                catch (Exception) { }

                return View(tmpObj);
            }
            else
                return View();
        }

        private Boolean CheckBook(Book book)
        {
            if (book.ID_book == 0)
                return false;
            return true;
        }

        public IActionResult SaveBook(int id_book, String title, String original_title, String author, DateTime date_published, String format, String language, String genre, int number_of_pages, float rating, String isbn)
        {
            Book tmpObj = new Book();

            tmpObj.ID_book = id_book;
            tmpObj.Title = title;
            tmpObj.Original_title = original_title;
            tmpObj.Author = author;
            tmpObj.Date_published = new LocalDate(date_published.Year, date_published.Month, date_published.Day);
            tmpObj.Format = format;
            tmpObj.Language= language;
            tmpObj.Genre = genre;
            tmpObj.Number_of_pages = number_of_pages;
            tmpObj.Rating = rating;
            tmpObj.ISBN = isbn;

            if (!CheckBook(tmpObj))
                return RedirectToAction("DisplayBookData");

            LocalDate ld = null;
            LocalDate minld = new LocalDate(1, 1, 1);
            if (tmpObj.Date_published== minld)
                ld = new LocalDate(1000, 10, 10);
            else
                ld = new LocalDate(tmpObj.Date_published.Year, tmpObj.Date_published.Month, tmpObj.Date_published.Day);

            try
            {
                session.Execute("insert into books (id_book, title, original_title, author, date_published," +
                                "format_knjige, jezik, zanr, broj_stranica, rating, isbn)" +
                                "VALUES (" + tmpObj.ID_book +
                                ", \'" + tmpObj.Title + "\'" +
                                ", \'" + tmpObj.Original_title + "\'" +
                                ", \'" + tmpObj.Author + "\'" +
                                ", \'" + ld.ToString() + "\'" +
                                ", \'" + tmpObj.Format + "\'" +
                                ", \'" + tmpObj.Language + "\'" +
                                ", \'" + tmpObj.Genre + "\'" +
                                ", " + tmpObj.Number_of_pages +
                                ", " + tmpObj.Rating + 
                                ", \'" + tmpObj.ISBN + "\');");
            }
            catch (Exception) { }
            return RedirectToAction("DisplayBookData");
        }

        public IActionResult DeleteBook(int id_book)
        {
            try
            {
                session.Execute("delete from books where " +
                                "id_book =" + id_book + " ; ");
            }
            catch (Exception) { }

            return RedirectToAction("DisplayKnjigaData");
        }

        public IActionResult UpdateBook(int id_book)
        {
            return RedirectToAction("GoToCreateBook", new { id_book = id_book });
        }
    }
}
