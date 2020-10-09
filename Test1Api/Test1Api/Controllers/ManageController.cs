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
    public class ManageController : ControllerBase
    {
        private readonly ManagerContext _db;
        public ManageController(ManagerContext db)
        {
            _db = db;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Student>>> Get()
        {
            return await _db.Students.ToListAsync();
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<Student>>> Get(long id)
        {
            var todoItem = await _db.Students
                                 .Where(s => s.CourseId == id)
                                 .ToListAsync();

            if (todoItem == null)
            {
                return NotFound();
            }

            return todoItem;
        }
    }
}
