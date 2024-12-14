//Main database class that is responsible for all database queries and operations
using Microsoft.Data.SqlClient;
using Microsoft.Maui.Controls;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace LibraryManagement.Data
{
    public class DbManager
    {
        string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Library;Integrated Security=True";
        //string connectionString = @"data source=DESKTOP-450AHJT\LOCALDB#F7F10ADA;initial catalog=Library;trusted_connection=true";
        static List<Book> books = new();
        static List<Member> members = new();
        static List<BorrowedBook> borrowedBooks = new();
        static List<BorrowedBook> borrowedBooksRecord = new();
        static List<Reservation> reservedBooks = new();
        List<string> categories = new List<string> { "Fiction", "Non-Fiction", "Science Fiction", "Mystery", "Romance", "Thriller", "Horror", "Biography", "Fantasy", "Self-Help" };
        List<string> admins = new();
        public DbManager() 
        {
            LoadBooks();
            LoadMembers();
            LoadBorrowedBook();
            LoadReservedBook();

        }

        //Load all book categories from the database and store them in the categories list
        public void LoadCategories()
        {
            //clear the list before loading new data to prevent duplicates
            categories.Clear();
            SqlConnection connection = new SqlConnection(connectionString);
            string query = @"SELECT * FROM lms_category;";
            connection.Open();
            SqlCommand command = new SqlCommand(query, connection);

            using SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                string data = $"{reader.GetInt32(0)}:{reader.GetString(1)}";  //stores category id and name in the format "id:name"
                categories.Add(data);
            }
        }

        //Load all reserved books from the database and store them in the reservedBooks list. 
        public void LoadReservedBook()
        {
            reservedBooks.Clear(); //clear the list before loading new data to prevent duplicates
            SqlConnection connection = new SqlConnection(connectionString);
            string query = @"
            SELECT 
                r.HoldId,
                r.BookId,
                r.PatreonId,
                CONCAT(p.FirstName, ' ', p.LastName) AS PatreonName,
                b.Title,
                r.ReservationDate
            FROM
                lms_reservation r
            JOIN
                lms_patreon p ON r.PatreonId = p.PatreonId
            JOIN
                lms_book b ON r.BookId = b.BookId
;";
            connection.Open();
            SqlCommand command = new SqlCommand(query, connection);

            using SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                //add a new reservation object to the reservedBooks list for every record retrieved from the database
                reservedBooks.Add(new Reservation(reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2), reader.GetString(3), reader.GetString(4), reader.GetDateTime(5)));
            }
        }

        //Load all borrowed books from the database and store them in the borrowedBooks list.
        public void LoadBorrowedBooksRecord()
        {
            //clear the list before loading new data to prevent duplicates
            borrowedBooks.Clear(); 
            SqlConnection connection = new SqlConnection(connectionString);
            string query = @"
            SELECT 
                c.CheckoutId,
                c.BookId,
                c.PatreonId,
                CONCAT(p.FirstName, ' ', p.LastName) AS PatreonName,
                b.Title,
                c.CheckoutDate,
                c.DueDate
            FROM 
                lms_checkout c
            JOIN 
                lms_patreon p ON c.PatreonId = p.PatreonId
            JOIN 
                lms_book b ON c.BookId = b.BookId
            --WHERE
            --    c.ReturnDate IS NULL;";

            connection.Open();
            SqlCommand command = new SqlCommand(query, connection);

            using SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                //add a new borrowed book object to the borrowedBooks list for every record retrieved from the database
                borrowedBooksRecord.Add(new BorrowedBook(reader.GetInt32(0),reader.GetInt32(1), reader.GetInt32(2), reader.GetString(3), reader.GetString(4), reader.GetDateTime(5), reader.GetDateTime(6)));
            }
        }

        //Load all borrowed books from the database and store them in the borrowedBooks list.
        public void LoadBorrowedBook()
        {
            //clear the list before loading new data to prevent duplicates
            borrowedBooks.Clear();
            SqlConnection connection = new SqlConnection(connectionString);
            string query = @"
            SELECT 
                c.CheckoutId,
                c.BookId,
                c.PatreonId,
                CONCAT(p.FirstName, ' ', p.LastName) AS PatreonName,
                b.Title,
                c.CheckoutDate,
                c.DueDate
            FROM 
                lms_checkout c
            JOIN 
                lms_patreon p ON c.PatreonId = p.PatreonId
            JOIN 
                lms_book b ON c.BookId = b.BookId
            WHERE
                c.ReturnDate IS NULL;";

            connection.Open();
            SqlCommand command = new SqlCommand(query, connection);

            //read a record at a time
            using SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                //add a new borrowed book object to the borrowedBooks list for every record retrieved from the database
                borrowedBooks.Add(new BorrowedBook(reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2), reader.GetString(3), reader.GetString(4), reader.GetDateTime(5), reader.GetDateTime(6)));
            }
        }

        //Reserve a book by adding a new record to the lms_reservation table in the database
        public async void ReserveBook(Reservation book)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            string query = @"INSERT INTO lms_reservation (BookId, PatreonId, ReservationDate, Status) VALUES (@BookId, @PatreonId, @ReservationDate, @Status);";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@BookId", book.BookID);
            command.Parameters.AddWithValue("@PatreonId", book.MemberID);
            command.Parameters.AddWithValue("@ReservationDate", book.ReservationDate);
            command.Parameters.AddWithValue("@Status", "On Hold");
            command.ExecuteNonQuery();
            await Application.Current.MainPage.DisplayAlert("Success", "Book reserved successfully", "OK");
        }

        //Cancel a book reservation by deleting the reservation record in the database
        public async void CancelReservation(Reservation book)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            //string query = @"
            //    UPDATE lms_reservation
            //    SET Status = 'Cancelled'
            //    WHERE HoldId = @HoldId;     
            //    ";
            string query = @"
                DELETE FROM lms_reservation
                WHERE HoldId = @HoldId;     
                ";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@HoldId", book.ReservationID);
            command.ExecuteNonQuery();
            await Application.Current.MainPage.DisplayAlert("Success", "Reservation cancelled successfully", "OK");
        }

        //Remove a book from the database
        public async void RemoveBook(Book book)
        {
            //check if the book is currently checked out or reserved before deleting it
            LoadBorrowedBooksRecord();
            //loads the borrowed books record to see if the book id to be removed matches with any id in the borrowed books record
            var borrowedBook = borrowedBooksRecord.Find(b => b.BookId == book.BookId);
            var reservedBook = reservedBooks.Find(b => b.BookID == book.BookId);
            //returns error message if the book exists in the borrowed or reserved books record
            if (borrowedBook != null)
            {
                // return "The book is currently checked out and cannot be deleted.";
                await Application.Current.MainPage.DisplayAlert("Error", "The book currently has a checkout record and cannot be deleted.", "OK");
                return;

            } else if(reservedBook != null) {
                await Application.Current.MainPage.DisplayAlert("Error", "The book is currently on hold and cannot be deleted.", "OK");
                return;
            }

            //runs the query if the book is not checked out or reserved
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            string query = @"DELETE FROM lms_book WHERE BookId = @BookId;";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@BookId", book.BookId);
            command.ExecuteNonQuery();
            await Application.Current.MainPage.DisplayAlert("Success", "Book removed successfully", "OK");
        }


        //makes a new checkout record in the database when a book is borrowed
        public async void BorrowBook(BorrowedBook book)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            string borrowBookQuery = @"INSERT INTO lms_checkout (BookId, PatreonId, CheckoutDate, DueDate) VALUES (@BookId, @PatreonId, @CheckoutDate, @DueDate);";
            SqlCommand command = new SqlCommand(borrowBookQuery, connection);
            command.Parameters.AddWithValue("@BookId", book.BookId);
            command.Parameters.AddWithValue("@PatreonId", book.MemberId);
            command.Parameters.AddWithValue("@CheckoutDate", book.BorrowedDate);
            command.Parameters.AddWithValue("@DueDate", book.DueDate);
            command.ExecuteNonQuery();
            await Application.Current.MainPage.DisplayAlert("Success", "Book borrowed successfully", "OK");
        }

        //returns a book by updating the return date in the lms_checkout table
        public async void ReturnBook(BorrowedBook book)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            string returnBookQuery = @"
                UPDATE lms_checkout
                SET ReturnDate = GETDATE()
                WHERE BookId = @BookId
                AND PatreonId = @PatreonId
                AND ReturnDate IS NULL;";
            SqlCommand command = new SqlCommand(returnBookQuery, connection);
            command.Parameters.AddWithValue("@BookId", book.BookId);
            command.Parameters.AddWithValue("@PatreonId", book.MemberId);
            command.ExecuteNonQuery();
            await Application.Current.MainPage.DisplayAlert("Success", "Book returned successfully", "OK");
        }

        //Add a new member to the database
        public void LoadMembers()
        {
            //clear the list before loading new data to prevent duplicates
            members.Clear();
            SqlConnection connection = new SqlConnection(connectionString);
            string query = @"SELECT * FROM lms_patreon;";
            connection.Open();
            SqlCommand command = new SqlCommand(query, connection);

            using SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                //add a new member object to the members list for every record retrieved from the database
                members.Add(new Member(reader.GetInt32(0), reader.GetString(1), reader.GetString(2)));
            }
        }

        //Add a new member to the database
        public async void AddNewMember(Member member)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            //insert new member record into the lms_patreon table
            string insertMemberQuery = @"INSERT INTO lms_patreon (FirstName, LastName) VALUES (@FirstName, @LastName);";
            SqlCommand command = new SqlCommand(insertMemberQuery, connection);
            command.Parameters.AddWithValue("@FirstName", member.FirstName);
            command.Parameters.AddWithValue("@LastName", member.LastName);
            command.ExecuteNonQuery();
            await Application.Current.MainPage.DisplayAlert("Success", "Member added successfully", "OK");
        }

        //Load all admin names from the database and store them in the admins list
        public void LoadAdmin()
        {
            admins.Clear();
            SqlConnection connection = new SqlConnection(connectionString);
            string query = @"SELECT * FROM lms_admin;";
            connection.Open();
            SqlCommand command = new SqlCommand(query, connection);

            using SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                admins.Add(reader.GetString(1));
            }
        }

        public async void AddNewAdmin(string name)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            string insertMemberQuery = @"INSERT INTO lms_admin (Name) VALUES (@Name);";
            SqlCommand command = new SqlCommand(insertMemberQuery, connection);
            command.Parameters.AddWithValue("@Name", name);
            command.ExecuteNonQuery();
            await Application.Current.MainPage.DisplayAlert("Success", "Admin added successfully", "OK");
        }

        public async void RemoveAdmin(string name)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            string query = @"DELETE FROM lms_admin WHERE Name = @Name;";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Name", name);
            command.ExecuteNonQuery();
            await Application.Current.MainPage.DisplayAlert("Success", "Admin removed successfully", "OK");
        }

        //Load all books from the database and store them in the books list
        public void LoadBooks()
        {
            books.Clear();
            SqlConnection connection = new SqlConnection(connectionString);
            //uses a join query to retrieve book details along with the author name and category name for easy access
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

        //Add a new book to the database
        public async void AddNewBook(Book book)
        {
            string firstName = book.AuthorName.Split(' ')[0];
            string lastName = book.AuthorName.Split(' ')[1];
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            //calls function to retrieve author id from the database if it already exixts, otherwise it adds a new author record
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
            command.Parameters.AddWithValue("@Genre", book.Genre);
            command.Parameters.AddWithValue("@PubDate", "");
            command.Parameters.AddWithValue("@Copies", book.Copies);
            command.Parameters.AddWithValue("@AvailableCopies", book.Copies);


            command.ExecuteNonQuery();
            await Application.Current.MainPage.DisplayAlert("Success", "Book Successfully Added", "OK");
        }

 
        int GetAuthor(SqlConnection connection, string firstName, string lastName)
        {
            int authorId = 0;
            using (SqlCommand command = new SqlCommand(@"SELECT AuthorId FROM lms_author WHERE FirstName = @FirstName AND LastName = @LastName;", connection))
            {
                command.Parameters.AddWithValue("@FirstName", firstName);
                command.Parameters.AddWithValue("@LastName", lastName);

                //if author already exists in database it retrieves the id, otherwise it adds a new author row to the table and returns the id
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
            LoadBooks();
            return books;
        }

        public List<Member> GetMembers()
        {
            LoadMembers();
            return members;
        }

        public List<BorrowedBook> GetBorrowedBooks()
        {
            LoadBorrowedBook();
            return borrowedBooks;
        }

        public List<Reservation> GetReservedBooks()
        {
            LoadReservedBook();
            return reservedBooks;
        }

        public List<string> GetCategories()
        {
            LoadCategories();
            return categories;
        }

        public List<string> GetAdmin()
        {
            LoadAdmin();
            return admins;
        }
    }
}
