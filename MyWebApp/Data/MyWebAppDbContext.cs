using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyWebApp.Models;

namespace MyWebApp.Data
{
    public class MyWebAppDbContext : DbContext
    {
        public MyWebAppDbContext (DbContextOptions<MyWebAppDbContext> options)
            : base(options)
        {
        }

        public DbSet<MyWebApp.Models.Joke> Joke { get; set; }

        public DbSet<MyWebApp.Models.Member> Member { get; set; }

        public DbSet<MyWebApp.Models.Product>? Product { get; set; }

        public DbSet<MyWebApp.Models.Wishlist> Wishlist { get; set; }

    }
}
