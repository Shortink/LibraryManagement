using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Data
{
    public class Member
    {
        int? memberID;
        string firstName;
        string lastName;

        

        public int? MemberID { get => memberID; set => memberID = value; }
        public string FirstName { get => firstName; set => firstName = value; }
        public string LastName { get => lastName; set => lastName = value; }

        public Member(int memberID, string firstName, string lastName)
        {
            this.memberID = memberID;
            this.firstName = firstName;
            this.lastName = lastName;
        }

        public Member(string firstName, string lastName)
        {
            this.firstName = firstName;
            this.lastName = lastName;
        }
    }
}
