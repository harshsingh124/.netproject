using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework.NewFolder
{
    [Table("InvoiceView")]
    public class InvoiceViewDto
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InvoiceId { get; set; }
        public int InvoiceNumber { get; set; }
        public decimal InvoiceAmount { get; set; }
        public string VendorCode { get; set; }
        public string CurrencyCode { get; set; }
        public bool IsActive { get; set; }
        public DateTime ReceivedDate { get; set; }

        public DateTime InvoiceDueDate { get; set; }
    }
}
