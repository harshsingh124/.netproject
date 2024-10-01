using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EntityFramework.NewFolder
{
    public class InvoiceDtocs
    {
        public int InvoiceId { get; set; }
        public int InvoiceNumber { get; set; }


        [Required]
        [ForeignKey("Currencies")]
        public int CurrencyId { get; set; }


        [Required]
        [ForeignKey("Vendors")]
        public int VendorId { get; set; }
        public decimal InvoiceAmount { get; set; }
        public DateTime ReceivedDate { get; set; }
        public DateTime InvoiceDueDate { get; set; }
        public bool IsActive { get; set; }

    }
}
