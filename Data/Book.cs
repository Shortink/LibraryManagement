//Book class for managing book details
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Data
{
    public class Book
    {
        int bookId;
        string authorName;
        string title;
        string isbn;
        int categoryId;
        string genre;
        DateTime publishedDate;
        int copies;
        int availableCopies;
        

        public int BookId { get => bookId; set => bookId = value; }
        public string AuthorName { get => authorName; set => authorName = value; }
        public string Title { get => title; set => title = value; }
        public string Isbn { get => isbn; set => isbn = value; }
        public int CategoryId { get => categoryId; set => categoryId = value; }
        public string Genre { get => genre; set => genre = value; }
        public DateTime PublishedDate { get => publishedDate; set => publishedDate = value; }

        public int Copies { get => copies; set => copies = value; }
        public int AvailableCopies { get => availableCopies; set => availableCopies = value; }
        

        public Book(int bookId, string authorName, string title, string isbn, int categoryId, string genre, DateTime publishedDate, int copies, int availableCopies)
        {
            this.bookId = bookId;
            this.authorName = authorName;
            this.title = title;
            this.isbn = isbn;
            this.categoryId = categoryId;
            this.genre = genre;
            this.PublishedDate = publishedDate;
            this.copies = copies;
            this.availableCopies = availableCopies;
        }

        public Book()
        {

        }
    }
}
