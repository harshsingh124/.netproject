using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework
{
    [Table("Invoices")]
    public class Invoice
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InvoiceId { get; set; }
        public int InvoiceNumber { get; set; }


        [Required]
        [ForeignKey("Currencies")]
        public int CurrencyId { get; set; }
        public virtual Currency Currency { get; set; }


        [Required]
        [ForeignKey("Vendors")]
        public int VendorId { get; set; }

        public virtual Vendor Vendor { get; set; }
        public decimal InvoiceAmount { get; set; }
        public DateTime ReceivedDate { get; set; }
        public DateTime InvoiceDueDate { get; set; }
        public bool IsActive { get; set; }

    }
}
