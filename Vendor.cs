using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EntityFramework
{

    [Table("Vendors")]
    public class Vendor
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int VendorId { get; set; }
        public string VendorName { get; set; }
        public string VendorCode { get; set; }
        public string VendorPhoneNumber { get; set; }
        public string VendorEmail { get; set; }
        public DateTime VendorCreatedOn { get; set; }
        public bool IsActive { get; set; }

        

    }
}
