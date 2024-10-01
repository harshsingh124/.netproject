using EntityFramework;
using EntityFramework.NewFolder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

namespace EntityFramework.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly AddDbContext _dbContext;
        public InvoiceController(AddDbContext dbContext)
        {

            _dbContext = dbContext;
        }


        [HttpGet]
        [Route("List")]
        public async Task<IActionResult> AllInvoice()
        {
            var invoice = await _dbContext.Invoices.ToListAsync();

            if (!invoice.Any())
            {
                return NotFound("No Invoice found.");
            }

            return Ok(invoice);

        }

        [HttpGet("filterinvoice/{cId}/{vId}")]
        public async Task<ActionResult<Invoice>> filterinvoice(int cId, int vId)
        {
            try
            {
                var iList = new List<Invoice>();

                iList = await _dbContext.Invoices.Where(i => i.VendorId == (vId == 0 ? i.VendorId : vId) && i.CurrencyId == (cId == 0 ? i.CurrencyId : cId)).ToListAsync();
                return Ok(iList);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }



        [HttpGet]
        [Route("getbyid/{id}")]
        public async Task<IActionResult> GetInvoiceByCode(int id)
        {
            var invoice = await _dbContext.Invoices.SingleOrDefaultAsync(i => i.InvoiceId == id);

            if (invoice == null)
            {
                return NotFound($"Invoice with code {id} not found.");
            }

            return Ok(invoice);
        }

        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> AddNewInvoices([FromBody] InvoiceDtocs dtocs)
        {

            var currencyExists = await _dbContext.Currencies.AnyAsync(c => c.CurrencyId == dtocs.CurrencyId);
            if (!currencyExists)
            {
                return NotFound($"Currency with ID {dtocs.CurrencyId} does not exist.");
            }


            if (dtocs.InvoiceDueDate <= dtocs.ReceivedDate)
            {
                return BadRequest("Invoice Due Date must be later than Invoice Received Date.");
            }

            var invoice = new Invoice()
            {
                InvoiceNumber = dtocs.InvoiceNumber,
                CurrencyId = dtocs.CurrencyId,
                VendorId = dtocs.VendorId,
                InvoiceAmount = dtocs.InvoiceAmount,
                ReceivedDate = dtocs.ReceivedDate,
                InvoiceDueDate = dtocs.InvoiceDueDate,
                IsActive = true,
            };


            await _dbContext.Invoices.AddAsync(invoice);
            await _dbContext.SaveChangesAsync();

            return Created("", new { message = "Invoice added successfully." });
        }



        [HttpDelete]
        [Route("delete/{id}")]
        public async Task<IActionResult> RemoveInvoice(int id)
        {
            var invoice = await _dbContext.Invoices.SingleOrDefaultAsync(i => i.InvoiceId == id);

            if (invoice == null)
            {
                return NotFound($"Invocie with id {id} not found.");
            }


            _dbContext.Invoices.Remove(invoice);
            await _dbContext.SaveChangesAsync();

            return Ok($"Invoice {id} deleted successfully.");
        }


        [HttpPut]
        [Route("edit/")]
        public async Task<IActionResult> EditInvoice([FromBody] InvoiceDtocs updatedInvoice)
        {
            var existingInvoice = await _dbContext.Invoices.SingleOrDefaultAsync(i => i.InvoiceNumber == updatedInvoice.InvoiceNumber);

            if (existingInvoice != null)
            {

                existingInvoice.InvoiceNumber = updatedInvoice.InvoiceNumber;
                existingInvoice.CurrencyId = updatedInvoice.CurrencyId;
                existingInvoice.VendorId = updatedInvoice.VendorId;
                existingInvoice.InvoiceAmount = updatedInvoice.InvoiceAmount;
                existingInvoice.ReceivedDate = updatedInvoice.ReceivedDate;
                existingInvoice.InvoiceDueDate = updatedInvoice.InvoiceDueDate;
                existingInvoice.IsActive = updatedInvoice.IsActive;


                _dbContext.SaveChanges();

                return Created("", new { V = $"Invoice updated successfully." });
            }

            return NotFound($"Invoice not found.");
        }

        [HttpGet]
        [Route("count/")]
        public int InvoiceCount()
        {
            return _dbContext.Invoices.Count();
        }



        //[HttpGet]
        //[Route("getbypropert/{property}/{value}")]

        //public async Task<IActionResult> GetInvoicesByVendor(string property, string value)
        //{


        //var invoices = await _dbContext.InvoiceView
        //    .Where(iv => iv.VendorCode == vendorCode)

        //    .ToListAsync();
        //       var propertyInfo = typeof(InvoiceViewDto).GetProperty(property, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);


        //       IQueryable<InvoiceViewDto> invoicesQuery = _dbContext.InvoiceView
        //.Select(iv => new InvoiceViewDto
        //{
        //    InvoiceId = iv.InvoiceId,
        //    InvoiceNumber = iv.InvoiceNumber,
        //    InvoiceAmount = iv.InvoiceAmount,
        //    VendorCode = iv.VendorCode,
        //    CurrencyCode = iv.CurrencyCode,
        //    IsActive = iv.IsActive,
        //    ReceivedDate = iv.ReceivedDate
        //});

        //       invoicesQuery = invoicesQuery.Where(iv =>
        //       {
        //           var propertyValue = propertyInfo.GetValue(iv);
        //       });


        //       return Ok(propertyInfo);
        //   }


        [HttpGet]
        [Route("getbypropert/{property}/{value}")]
        public IActionResult GetInvoicesByVendor(string property, string value)
        {
            

            var propertyInfo = typeof(InvoiceViewDto).GetProperty(property);
            if (propertyInfo == null)
            {
                return BadRequest($"Property {property} not found on InvoiceViewDto.");
            }

            var parameter = Expression.Parameter(typeof(InvoiceViewDto), "iv");


            var propertyAccess = Expression.Property(parameter, propertyInfo);


            var typedValue = Convert.ChangeType(value, propertyInfo.PropertyType);

            var constant = Expression.Constant(typedValue);

            var comparison = Expression.Equal(propertyAccess, constant);

            var lambda = Expression.Lambda<Func<InvoiceViewDto, bool>>(comparison, parameter);

            var result = _dbContext.InvoiceView.Where(lambda).ToList();

            return Ok(result);
        }



    }

}



