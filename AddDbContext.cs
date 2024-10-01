using EntityFramework.NewFolder;
using Microsoft.EntityFrameworkCore;
using System.Numerics;
using System.Reflection.Emit;

namespace EntityFramework
{
    public class AddDbContext : DbContext
    {

        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<Invoice> Invoices { get; set; }

        public DbSet<InvoiceViewDto> InvoiceView { get; set; }

        public AddDbContext(DbContextOptions<AddDbContext> options)
            : base(options)
        {
            
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer("DefaultConnection");
        //}

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<InvoiceViewDto>()
        //        //.HasNoKey() // Specify that this is a keyless entity type
        //    .ToView("InvoiceView");

        //    // Additional configurations for other entities can go here
        //}

    }
}
