using System.ComponentModel.DataAnnotations;

namespace SancScan.Models
{
    public class Login
    {
        [Key]
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }    
    }
}
