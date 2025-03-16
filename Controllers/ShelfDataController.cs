using Cassandra;
using greatreads.Models;
using Microsoft.AspNetCore.Mvc;

namespace greatreads.Controllers
{
    public class ShelfDataController : Controller
    {
        Cassandra.ISession session = null;

        public ShelfDataController()
        {
            try
            {
                var cluster = Cluster.Builder().AddContactPoint("127.0.0.1").Build();
                session = cluster.Connect("greatreads");
            }
            catch (Exception ex) { }
        }

        public IActionResult DisplayShelfData(int id_user)
        {
            ShelfData sd = null;
            if (id_user == 0)
                sd = new ShelfData();
            else 
                sd = new ShelfData(id_user);
                return View();
        }

        public IActionResult DisplayShelf(int id_shelf)
        {
            if (id_shelf == 0)
                return RedirectToAction("DisplayShelfData");
            else
            {
                ShelfBooksData sb = new ShelfBooksData(id_shelf);
                return View(sb);
            }
        }

        public IActionResult GoToCreateShelf(int id_shelf)
        {
            if (id_shelf != 0)
            {
                Shelf tmpObj = null;

                try
                {
                    var results = session.Execute("select * from shelves where " +
                                    "id_shelf =" + id_shelf + " ; ");

                    foreach (var result in results)
                    {
                        tmpObj = new Shelf();

                        tmpObj.ID_shelf = result.GetValue<int>("id_shelf");
                        tmpObj.Name = result.GetValue<String>("name");
                        tmpObj.Number_of_books = result.GetValue<int>("number_of_books");
                    }
                }
                catch (Exception) { }

                return View(tmpObj);
            }
            else
                return View();
        }

        private Boolean CheckShelf(Shelf shelf)
        {
            if (shelf.ID_shelf == 0)
                return false;
            return true;
        }

        public IActionResult SaveShelf(int id_shelf, String name, int id_user)
        {
            int number_of_books = 0;
            try
            {
                var result = session.Execute("select * from shelves where id_shelf=" + id_shelf + ";");
                if (result.Count() != 0)
                    number_of_books = result.First().GetValue<int>("number_of_books");
            }
            catch (Exception) { }

            Shelf tmpObj = new Shelf();

            tmpObj.ID_shelf = id_shelf;
            tmpObj.Name = name;
            tmpObj.Number_of_books = number_of_books;

            if (!CheckShelf(tmpObj))
                return RedirectToAction("DisplayShelfData");

            var us = session.Execute("select * from usershelves where id_shelf=" +
                                        id_shelf + " and id_user=" + id_user + ";");

            if (us.Count() != 0)
                return RedirectToAction("DisplayShelfData");

            try
            {
                session.Execute("insert into shelves (id_shelf, name, number_of_books)" +
                                "values (" + tmpObj.ID_shelf +
                                ", \'" + tmpObj.Name + "\'" +
                                ", " + tmpObj.Number_of_books + ");");

                session.Execute("Insert into policekorisnika (id_user, id_shelf) " +
                                "values (" + id_user + ", " + id_shelf + ");");
            }
            catch (Exception ex) { }
            return RedirectToAction("DisplayShelfData");
        }

        public IActionResult DeleteShelf(int id_shelf)
        {
            try
            {
                session.Execute("delete from shelfbooks where id_shelf=" + id_shelf + ";");
                var ids = session.Execute("select * from usershelves where id_shelf=" + id_shelf + ";");

                foreach (var id in ids)
                {
                    int id_user = id.GetValue<int>("id_user");

                    session.Execute("delete from usershelves where id_shelf = " + id_shelf +
                                    " and id_user=" + id_user + "; ");
                }

                session.Execute("delete from shelves where " +
                                "id_shelf =" + id_shelf + " ; ");
            }
            catch (Exception ex) { }

            return RedirectToAction("DisplayShelfData");
        }

        public IActionResult UpdateShelf(int id_shelf)
        {
            return RedirectToAction("CreateShelf", new { id_shelf = id_shelf });
        }

        public IActionResult GoToAddToShelf(int id_book)
        {
            ShelfData sd = new ShelfData();
            sd.ID_book = id_book;
            return View(sd);
        }

        public IActionResult AddToShelf(int id_book, int id_shelf)
        {
            if (id_book != 0 && id_shelf != 0)
            {
                var sb = session.Execute("select * from shelfbooks where id_shelf=" + id_shelf +
                                        " and id_book=" + id_book + ";");

                if (sb.Count() != 0)
                    return RedirectToAction("DisplayShelfData");
                try
                {
                    session.Execute("Insert into shelfbooks (id_shelf, id_book)" +
                                    "values (" + id_shelf + ", " + id_book + ");");
                    var result = session.Execute("select * from shelf where id_shelf=" + id_shelf + ";").First();
                    Shelf sh = new Shelf();

                    sh.Number_of_books = result.GetValue<int>("number_of_books");
                    sh.Number_of_books++;

                    session.Execute("Update polica set number_of_books=" + sh.Number_of_books + " where id_shelf=" + id_shelf + ";");
                }
                catch (Exception ex)
                { }
            }
            return RedirectToAction("DisplayPolicaData");
        }

        public IActionResult RemoveFromShelf(int id_shelf, int id_book)
        {
            int id_user = 0;
            try
            {
                session.Execute("delete from shelfbooks where id_shelf=" + id_shelf + " and id_book=" + id_book + ";");
                var shelf = session.Execute("select * from shelf where id_shelf=" + id_shelf + ";").First();

                Shelf sh = new Shelf();

                sh.Number_of_books = shelf.GetValue<int>("number_of_books");
                sh.Number_of_books--;

                session.Execute("update shelves set number_of_books=" + sh.Number_of_books + " where id_shelf=" + id_shelf + ";");

                var user = session.Execute("select id_user from userbooks where id_shelf=" + id_shelf + ";").First();

                id_user = user.GetValue<int>(0);
            }
            catch (Exception ex)
            { }
            return RedirectToAction("DisplayShelfData", new { id_user = id_user });
        }
    }
}
