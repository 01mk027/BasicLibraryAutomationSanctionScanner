using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SancScan.Models;

    public class DatabaseContext : DbContext
    {
        public DatabaseContext (DbContextOptions<DatabaseContext> options)
            : base(options)
        {
        }

        public DbSet<SancScan.Models.Book> Book { get; set; } = default!;

        public DbSet<SancScan.Models.Borrow>? Borrow { get; set; }

        public DbSet<SancScan.Models.Login>? Login { get; set; }

        
}
