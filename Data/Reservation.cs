//Reservation class to store details about a book reservation
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Data
{
    public class Reservation
    {
        int reservationID;
        int bookID;
        int memberID;
        string memberName;
        string bookName;
        DateTime reservationDate;

        public Reservation(int reservationID, int bookID, int memberID, string memberName, string bookName, DateTime reservationDate)
        {
            this.reservationID = reservationID;
            this.bookID = bookID;
            this.memberID = memberID;
            this.memberName = memberName;
            this.bookName = bookName;
            this.reservationDate = reservationDate;
        }

        public Reservation(int bookId, int memberId, DateTime reservationDate)
        {
            this.bookID = bookId;
            this.memberID = memberId;
            this.reservationDate = reservationDate;
        }

        public int ReservationID { get => reservationID; set => reservationID = value; }
        public int BookID { get => bookID; set => bookID = value; }
        public int MemberID { get => memberID; set => memberID = value; }
        public string MemberName { get => memberName; set => memberName = value; }
        public string BookName { get => bookName; set => bookName = value; }
        public DateTime ReservationDate { get => reservationDate; set => reservationDate = value; }
    }
}
