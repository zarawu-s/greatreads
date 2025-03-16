using Cassandra;

namespace greatreads.Models
{
    public class ShelfBooks
    {
        public Shelf Shelf;
        public List<Book> Books;
    }

    public class ShelfBooksData
    {
        public List<ShelfBooks> Shelves = new List<ShelfBooks>();

        public ShelfBooksData()
        {
            try
            {
                var cluster = Cluster.Builder().AddContactPoint("127.0.0.1").Build();
                using var session = cluster.Connect("greatreads");
                var results = session.Execute("select distinct id_shelf from shelfbooks;").Distinct();
                ShelfBooks tmpObj = null;

                Shelf tmpShelf = null;
                List<Book> tmpBooks = null;
                Book tmpBook = null;

                foreach (var result in results)
                {
                    tmpObj = new ShelfBooks();
                    tmpShelf = new Shelf();

                    int id_shelf = result.GetValue<int>("id_shelf");
                    var sp = session.Execute("select * from shelf where id_shelf=" + id_shelf + ";").First();

                    tmpShelf.ID_shelf = sp.GetValue<int>("id_shelf");
                    tmpShelf.Name = sp.GetValue<String>("name");
                    tmpShelf.Number_of_books = sp.GetValue<int>("number_of_books");

                    tmpObj.Shelf = tmpShelf;

                    var skp = session.Execute("select id_book from shelfbooks where id_shelf=" + id_shelf + ";");

                    tmpBooks = new List<Book>();

                    foreach (var sk in skp)
                    {
                        tmpBook = new Book();
                        int id_book = sk.GetValue<int>("id_book");

                        var book = session.Execute("select * from books where id_book=" + id_book + ";").FirstOrDefault();
                        tmpBook.ID_book = id_book;
                        tmpBook.Title = book.GetValue<String>("title");
                        tmpBook.Original_title = book.GetValue<String>("original_title");
                        tmpBook.Author = book.GetValue<String>("author");
                        tmpBook.Number_of_pages = book.GetValue<int>("number_of_pages");
                        tmpBook.Format = book.GetValue<String>("format");
                        tmpBook.Date_published = book.GetValue<LocalDate>("date_published");
                        tmpBook.Language = book.GetValue<String>("language");
                        tmpBook.Rating = book.GetValue<float>("rating");
                        tmpBook.Genre = book.GetValue<String>("genre");
                        tmpBook.ISBN = book.GetValue<String>("isbn");

                        tmpBooks.Add(tmpBook);
                    }

                    tmpObj.Books = tmpBooks;
                    Shelves.Add(tmpObj);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public ShelfBooksData(int id_shelf)
        {
            try
            {
                var cluster = Cluster.Builder().AddContactPoint("127.0.0.1").Build();
                using var session = cluster.Connect("greatreads");
                var results = session.Execute("select id_book from shelfbooks where id_shelf=" + id_shelf + ";");

                ShelfBooks tmpObj = new ShelfBooks();
                Shelf tmpShelf = new Shelf();
                List<Book> tmpBooks = new List<Book>();
                Book tmpBook = null;

                var sp = session.Execute("select * from shelves where id_shelf=" + id_shelf + ";").First();

                tmpShelf.ID_shelf = sp.GetValue<int>("id_shelf");
                tmpShelf.Name = sp.GetValue<String>("name");
                tmpShelf.Number_of_books = sp.GetValue<int>("number_of_books");

                tmpObj.Shelf = tmpShelf;

                foreach (var result in results)
                {
                    tmpBook = new Book();
                    int id_book = result.GetValue<int>("id_book");

                    var book = session.Execute("select * from books where id_book=" + id_book + ";").FirstOrDefault();

                    tmpBook.ID_book = id_book;
                    tmpBook.Title = book.GetValue<String>("title");
                    tmpBook.Original_title = book.GetValue<String>("original_title");
                    tmpBook.Author = book.GetValue<String>("author");
                    tmpBook.Number_of_pages = book.GetValue<int>("number_of_pages");
                    tmpBook.Format = book.GetValue<String>("format");
                    tmpBook.Date_published = book.GetValue<LocalDate>("date_published");
                    tmpBook.Language = book.GetValue<String>("language");
                    tmpBook.Rating = book.GetValue<float>("rating");
                    tmpBook.Genre = book.GetValue<String>("genre");
                    tmpBook.ISBN = book.GetValue<String>("isbn");

                    tmpBooks.Add(tmpBook);
                }

                tmpObj.Books = tmpBooks;

                Shelves.Add(tmpObj);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}

