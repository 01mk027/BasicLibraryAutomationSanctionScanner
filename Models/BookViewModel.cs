namespace SancScan.Models
{
    public class BookViewModel
    {
        public int BookId { get; set; }
        public string BookName { get; set; }
        public string AuthorName { get; set; }
        public bool DoesExist { get; set; }
        public bool IsBorrowed { get; set; }

        public string ImageFullPath { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? BorrowerName { get; set; }

        public DateTime? BringBackDate { get; set; }
    }
}