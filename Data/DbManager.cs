using Microsoft.Data.SqlClient;
using Microsoft.Maui.Controls;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Data
{
    public class DbManager
    {
        static List<Book> books = new();
        public DbManager() 
        {
            LoadBooks();
        }

        

        public async void LoadBooks()
        {
            string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Library;Integrated Security=True";
            SqlConnection connection = new SqlConnection(connectionString);
            string query = @"
            SELECT 
                b.BookId,
                CONCAT(a.FirstName, ' ', a.LastName) AS AuthorName,
                b.Title,
                b.ISBN,
                b.CategoryId,
                b.Genre,
                b.PubDate AS PublishedDate,
                b.Copies,
                b.AvailableCopies
            FROM 
                lms_book b
            JOIN 
                lms_author a ON b.AuthorId = a.AuthorId
            JOIN 
                lms_category c ON b.CategoryId = c.CategoryId;";
            connection.Open();
            SqlCommand command = new SqlCommand(query, connection);

            using SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())//read a record at a time
            {
                books.Add(new Book(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetInt32(4), reader.GetString(5), reader.GetDateTime(6), reader.GetInt32(7), reader.GetInt32(8)) );
            }
        }

        public async void AddNewBook(Book book)
        {
            string firstName = book.AuthorName.Split(' ')[0];
            string lastName = book.AuthorName.Split(' ')[1];
            string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Library;Integrated Security=True";
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            int authorId = GetAuthor(connection, firstName, lastName);

            string insertBookQuery = @"
                INSERT INTO lms_book (AuthorId, Title, ISBN, CategoryId, Genre, PubDate, Copies, AvailableCopies)
                VALUES (@AuthorId, @Title, @ISBN, @CategoryId, @Genre, @PubDate, @Copies, @AvailableCopies);
            ";
            SqlCommand command = new SqlCommand(insertBookQuery, connection);
            command.Parameters.AddWithValue("@AuthorId", authorId);
            command.Parameters.AddWithValue("@Title", book.Title);
            command.Parameters.AddWithValue("@ISBN", book.Isbn);
            command.Parameters.AddWithValue("@CategoryId", book.CategoryId);
            command.Parameters.AddWithValue("@Genre", "");
            command.Parameters.AddWithValue("@PubDate", "");
            command.Parameters.AddWithValue("@Copies", book.Copies);
            command.Parameters.AddWithValue("@AvailableCopies", book.Copies);


            command.ExecuteNonQuery();
        }

        int GetAuthor(SqlConnection connection, string firstName, string lastName)
        {
            int authorId = 0;
            using (SqlCommand command = new SqlCommand(@"SELECT AuthorId FROM lms_author WHERE FirstName = @FirstName AND LastName = @LastName;", connection))
            {
                command.Parameters.AddWithValue("@FirstName", firstName);
                command.Parameters.AddWithValue("@LastName", lastName);

                //if author already exists in database it retries the id, otherwise it adds a new author row to the table and returns the id
                var result = command.ExecuteScalar();
                if (result != null)
                {
                    authorId = Convert.ToInt32(result);
                }
                else
                {
                    string insertAuthorQuery = @"
                    INSERT INTO lms_author (FirstName, LastName)
                    VALUES (@FirstName, @LastName);
                    SELECT SCOPE_IDENTITY(); -- Get the last inserted AuthorId";

                    using (SqlCommand insertCommand = new SqlCommand(insertAuthorQuery, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@FirstName", firstName);
                        insertCommand.Parameters.AddWithValue("@LastName", lastName);

                        // Retrieve the AuthorId of the newly inserted author
                        authorId = Convert.ToInt32(insertCommand.ExecuteScalar());
                    }
                }
            }
            return authorId;
        }

        public List<Book> GetBooks()
        {
            return books;
        }
    }
}
