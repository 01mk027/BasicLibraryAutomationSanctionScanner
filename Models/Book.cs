using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace SancScan.Models
{
    public class Book
    {
        [Key]
        public int BookId { get; set; }    
        public string BookName { get; set; }
        public string AuthorName { get; set; }
        public bool DoesExist { get; set; }
        public byte[] Image { get; set; }
        public string ImageName { get; set; }
        public long ImageSize { get; set; }
        public string ImageFullPath { get; set; }
        public bool IsBorrowed { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<Borrow> Borrow { get; set; }
    }
}
