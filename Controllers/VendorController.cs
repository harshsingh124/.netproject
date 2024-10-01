using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace EntityFramework.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendorController : ControllerBase
    {
        private readonly AddDbContext _dbContext;
        public VendorController(AddDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        [Route("List")]
        public async Task<IActionResult> AllVendor()
        {
            var vendors = await _dbContext.Vendors.ToListAsync();

            if (!vendors.Any())
            {
                return NotFound("No Vendor found.");
            }

            return Ok(vendors);
        }




        [HttpGet]
        [Route("getbyid/{code}")]
        public async Task<IActionResult> GetVendorByCode(string code)
        {
            var vendor = await _dbContext.Vendors.SingleOrDefaultAsync(v => v.VendorCode == code);

            if (vendor == null)
            {
                return NotFound($"Vendor with code {code} not found.");
            }

            return Ok(vendor);
        }

        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> AddNewVendor([FromBody] Vendor v)
        {
            //if (string.IsNullOrWhiteSpace(v.VendorCode) || v.VendorCode.Length != 3)
            //{
            //    return BadRequest("Vendor Code must be exactly 3 characters long.");
            //}

            if (string.IsNullOrWhiteSpace(v.VendorName))
            {
                return BadRequest("Vendor Name cannot be empty.");
            }

            if (await _dbContext.Vendors.AnyAsync(existingVendor => existingVendor.VendorCode == v.VendorCode))
            {
                return Conflict("Vendor Code already exists in the database.");
            }

            await _dbContext.Vendors.AddAsync(v);
            await _dbContext.SaveChangesAsync();

            return Created( "" , new {});
        }

        [HttpDelete]
        [Route("delete/{id}")]

        public async Task<IActionResult> RemoveVendor(int id)
        {
            var vendor = await _dbContext.Vendors.SingleOrDefaultAsync(v => v.VendorId == id);

            if (vendor == null)
            {
                return NotFound($"Vendor with id {id} not found.");
            }

            var isVendorInUse = await _dbContext.Invoices.AnyAsync(i => i.VendorId == vendor.VendorId);

            if (isVendorInUse)
            {
                return BadRequest($"Vendor id {id} is present in Invoices ");
            }

            _dbContext.Vendors.Remove(vendor);
            await _dbContext.SaveChangesAsync();

            return Created("" , new { V = $"Vendor {id} deleted successfully." });
        }

        [HttpPut]
        [Route("edit/")]
        public async Task<IActionResult> EditVendor([FromBody] Vendor updatedVendor)
        {
            var existingVendor = await _dbContext.Vendors.SingleOrDefaultAsync(v => v.VendorId == updatedVendor.VendorId);

            if (existingVendor != null)
            {
                existingVendor.VendorName = updatedVendor.VendorName;
                existingVendor.VendorCode = updatedVendor.VendorCode;
                existingVendor.VendorPhoneNumber = updatedVendor.VendorPhoneNumber;
                existingVendor.VendorEmail = updatedVendor.VendorEmail;
                existingVendor.VendorCreatedOn = updatedVendor.VendorCreatedOn;
                existingVendor.IsActive = updatedVendor.IsActive;

                await _dbContext.SaveChangesAsync();

                return Created("", new { V = $"Vendor updated successfully." });
            }

            return NotFound("Vendor not found.");
        }


        [HttpGet]
        [Route("count/")]
        public int VendorCount()
        {
            return _dbContext.Vendors.Count();
        }

        [HttpGet]
        [Route("pagination/{pageNumber}")]
        public async Task<IActionResult> Pagination(int pageNumber = 1, int pageSize = 3)
        {

            if (pageNumber <= 0 || pageSize <= 0)
            {
                return BadRequest("Invalid page number or page size.");
            }


            var totalVendors = await _dbContext.Vendors.CountAsync();


            var vendor = await _dbContext.Vendors
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            if (!vendor.Any())
            {
                return NotFound("No Vendor found.");
            }

            return Ok(new
            {
                TotalVendors = totalVendors,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Vendor = vendor
            });
        }

    }
}
