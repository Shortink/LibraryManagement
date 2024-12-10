using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Data
{
    public class BorrowedBook
    {
        int id;
        int bookId;
        int memberId;
        string memberName;
        DateTime borrowedDate;
        DateTime dueDate;

        public BorrowedBook(int bookId, int memberId, string memberName, DateTime borrowedDate, DateTime dueDate)
        {
            this.bookId = bookId;
            this.memberName = memberName;
            this.borrowedDate = borrowedDate;
            this.dueDate = dueDate;
            this.memberId = memberId;
        }

        public BorrowedBook(int bookId, int memberId, DateTime borrowedDate, DateTime dueDate)
        {
            this.bookId = bookId;
            this.borrowedDate = borrowedDate;
            this.dueDate = dueDate;
            this.memberId = memberId;
        }

        public BorrowedBook(int id, int bookId, int memberId, string memberName, DateTime borrowedDate, DateTime dueDate)
        {
            this.id = id;
            this.bookId = bookId;
            this.memberId = memberId;
            this.memberName = memberName;
            this.borrowedDate = borrowedDate;
            this.dueDate = dueDate;
        }

        public int Id { get => id; set => id = value; }
        public string MemberName { get => memberName; set => memberName = value; }
        public int BookId { get => bookId; set => bookId = value; }
        public DateTime DueDate { get => dueDate; set => dueDate = value; }
        public DateTime BorrowedDate { get => borrowedDate; set => borrowedDate = value; }
        public int MemberId { get => memberId; set => memberId = value; }
    }
}
