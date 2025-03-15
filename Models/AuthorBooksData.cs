using Cassandra;

namespace greatreads.Models
{
    public class AuthorBooks
    {
        public int ID_book;
        public int ID_author;
        public String Title;
    }

    public class AuthorBooksData
    {
        public List<AuthorBooks> AuthorBooks = new List<AuthorBooks>();

        public AuthorBooksData()
        {
            try
            {
                var cluster = Cluster.Builder().AddContactPoint("127.0.0.1").Build();
                using var session = cluster.Connect("greatreads");
                var results = session.Execute("select * from authorbooks;");
                AuthorBooks tmpObj = null;

                foreach (var result in results)
                {
                    tmpObj = new AuthorBooks();

                    tmpObj.ID_book = result.GetValue<int>("id_book");
                    tmpObj.ID_author = result.GetValue<int>("id_author");
                    tmpObj.Title = result.GetValue<String>("title");

                    AuthorBooks.Add(tmpObj);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
