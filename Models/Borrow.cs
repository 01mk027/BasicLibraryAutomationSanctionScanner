using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace SancScan.Models
{
    public class Borrow
    {
        public int BorrowId { get; set; }
        public String BorrowerName { get; set; }
        
        public DateTime BorrowDateTime { get; set; }
        public Nullable<DateTime> BringBackDate { get; set; }
        public Nullable<DateTime> ReceivedBackTime { get; set; }
        public DateTime DateToBeReturned { get; set; }
        public bool IsBroughtBack { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; }
    }
}
