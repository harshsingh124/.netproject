using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace EntityFramework.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class CurrencyController : Controller
    {

        private readonly AddDbContext _dbContext;
        public CurrencyController(AddDbContext dbContext)
        {

            _dbContext = dbContext;
        }


        [HttpGet]
        [Route("AllCurrency")]
        public IActionResult AllCurrency()
        {
            var currencies = _dbContext.Currencies.ToList();

            if (!currencies.Any())
            {
                return NotFound("No currencies found.");
            }

            return Ok(currencies);
        }



        [HttpGet]
        [Route("getbyid/{id}")]
        public async Task<IActionResult> GetCurrencyByCode(int id)
        {
            var currency = await _dbContext.Currencies.SingleOrDefaultAsync(c => c.CurrencyId == id);

            if (currency == null)
            {
                return NotFound($"Curerncy with code {id} not found.");
            }

            return Ok(currency);
        }


        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> AddNewCurrency(Currency c)
        {
            if (string.IsNullOrWhiteSpace(c.CurrencyCode) || c.CurrencyCode.Length != 3)
            {
                throw new ArgumentException("Currency Code must be exactly 3 characters long.");
            }

            if (string.IsNullOrWhiteSpace(c.CurrencyName))
            {
                throw new ArgumentException("Currency Name cannot be empty.");
            }

            if (_dbContext.Currencies.Any(existingCurrency => existingCurrency.CurrencyCode == c.CurrencyCode))
            {
                throw new ArgumentException("Currency Code already exists in the database.");
            }

            _dbContext.Currencies.Add(c);
            _dbContext.SaveChanges();

            return Created("", new { V = "Currency Added successfully." });
        }


        [HttpDelete]
        [Route("delete/{id}")]
        public async Task<IActionResult> RemoveCurrency(int id)
        {
            var currency = _dbContext.Currencies.SingleOrDefault(c => c.CurrencyId == id);

            if (currency == null)
            {
                throw new Exception($"Currency with code {id} not found.");
            }

            var isCurrencyInUse = await _dbContext.Invoices.AnyAsync(i => i.CurrencyId == currency.CurrencyId);

            if (isCurrencyInUse)
            {
                return BadRequest($"Currency id {id} is present in Invoices ");
            }


            _dbContext.Currencies.Remove(currency);
            _dbContext.SaveChanges();

            return Created("", new { V = $"Currency {id} deleted successfully." });
        }




        [HttpPut]
        [Route("edit/")]
        public async Task<IActionResult> EditCurrency([FromBody] Currency currency)
        {
            var existingCurrency = _dbContext.Currencies.SingleOrDefault(c => c.CurrencyId == currency.CurrencyId);

            if (existingCurrency != null)
            {

                existingCurrency.CurrencyName = currency.CurrencyName;
                existingCurrency.CurrencyCode = currency.CurrencyCode;


                _dbContext.SaveChanges();

                return Created("", new { V = $"Currency updated successfully." });
            }

            return NotFound($"Currency not found.");
        }


        [HttpGet]
        [Route("count/")]
        public int curencyCount()
        {
            return _dbContext.Currencies.Count();
        }


        [HttpGet]
        [Route("pagination/{pageNumber}")]
        public async Task<IActionResult> Pagination(int pageNumber = 1, int pageSize = 3)
        {
          
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return BadRequest("Invalid page number or page size.");
            }

        
            var totalCurrencies = await _dbContext.Currencies.CountAsync();

       
            var currency = await _dbContext.Currencies
                .Skip((pageNumber - 1) * pageSize) 
                .Take(pageSize)                    
                .ToListAsync();

            if (!currency.Any())
            {
                return NotFound("No currency found.");
            }

            return Ok(new
            {
                TotalCurrencies = totalCurrencies,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Currency = currency
            });
        }



    }



}
