using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Test1Api.Models;

namespace Test1Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReceiptsController : ControllerBase
    {
        private readonly TodoContext _db;
        public ReceiptsController(TodoContext db)
        {
            _db = db;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Receipt>>> getAll()
        {
            return await _db.Receipts
                .AsNoTracking()
                .Include(r => r.ReceiptItems )
                .ToListAsync();
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Receipt>> Get(long id)
        {
            var Receipt = await _db.Receipts
                                .Where(r => r.Receipt_ID == id)
                                .Include(r => r.ReceiptItems)
                                .FirstAsync();
                                            

            if (Receipt == null)
            {
                return NoContent();
            }

            return Receipt;
        }
    }
}
