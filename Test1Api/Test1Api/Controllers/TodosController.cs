using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Test1Api.Models;
using Test1Api.Models.Requests;
using Test1Api.Models.Responses;
using Test1Api.Utils;

namespace Test1Api.Controllers
{
    //[Authorize(Roles = "Admin, Member")]
    [Route("api/[controller]")]
    [ApiController]
    public class TodosController : ControllerBase
    {
        private readonly TodoContext _db;
        public TodosController(TodoContext db)
        {
            _db = db;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItem>>> getAll()
        {
            return await _db.TodoItems.AsNoTracking()
                                       .Where( x => x.IsDeleted == false)
                                       .OrderBy( x => x.Name)
                                       .ThenByDescending( x => x.IsComplete)
                                       .ToListAsync();
        }
        
        [HttpGet("list")]
        public async Task<ActionResult<PagingResponse>> getPaging([FromQuery] PagingRequest req)
        {
            var query = _db.TodoItems.AsNoTracking().Where ( x => x.IsDeleted == false);

            //filtering
            if ( !string.IsNullOrEmpty(req.Query))
            {
                query = query.Where(x => x.Name.Contains(req.Query));
            }

            // sorting
            //if ( req.Order.ToUpper() == "DESC")
            //{
            //    query = query.OrderByDescending(x => x.Name);
            //}else
            //{
            //    query = query.OrderBy(x => x.Name);
            //}
            if (string.IsNullOrEmpty(req.SortBy))
                query = query.OrderBy(x => x.Name);
            else
            {
                //string sortEx = HttpUtility.UrlDecode(req.SortBy, Encoding.UTF8);
                //var sorts = sortEx.Split(",");
                string sortEx =req.SortBy;
                var sorts = sortEx.Split(",");
                foreach (string sort in sorts)
                {
                    string direction = sort.Trim().Substring(0, 1);
                    string fieldName = sort.Trim().Substring(1).toTitleCase();
                    if ( direction.Equals("+")) //ascending
                    {
                        query = query.OrderBy(x => EF.Property<TodoItem>(x, fieldName));
                    }
                    else //descending
                    {
                        query = query.OrderByDescending(x => x);
                    }
                }
            }

            int totalRows = await query.CountAsync();

            var pageCount = (double)totalRows / req.Size;
            int totalPage = (int)Math.Ceiling(pageCount);

            var skip = (req.Page - 1) * req.Size; // (pageNumber - 1) * pageSize
            var result = await query.Skip(skip).Take(req.Size).ToListAsync();

            return new PagingResponse
            {
                Data = result,
                PagingInfo = new PagingInfo
                {
                    CurrentPage = req.Page,
                    PageSize = req.Size,
                    TotalRecords = totalRows,
                    TotalPages = totalPage
                }
            };
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> Get(long id)
        {
            var todoItem = await _db.TodoItems.AsNoTracking()
                                              .FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);

            if (todoItem == null)
            {
                return NoContent();
            }

            return todoItem;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<TodoItem>> AddNew(TodoItem todoItem)
        {
            var userInfo = Utils.Helper.getClaimData(User);
            todoItem.CreatedBy = userInfo.Id;
            todoItem.CreatedDate = DateTime.Now;

            _db.TodoItems.Add(todoItem);
            await _db.SaveChangesAsync();
            return CreatedAtAction("get", new { id = todoItem.Id }, todoItem);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<TodoItem>> Edit(long id, TodoItem todoItem)
        {
            var todo = await _db.TodoItems.FindAsync(id);
            if (todo == null)
            {
                return NotFound();
            }
            var userInfo = Utils.Helper.getClaimData(User);

            todo.Name = todoItem.Name;
            todo.IsComplete = todoItem.IsComplete;
            todo.UpdatedBy = userInfo.Id;
            todo.UpdatedDate = DateTime.Now;

            //_db.TodoItems.Update(todo);
            await _db.SaveChangesAsync();

            return todo;
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<TodoItem>> Delete(long id)
        {
            var todoItem = await  _db.TodoItems.FindAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }
            //_db.TodoItems.Remove(todoItem);
            var userInfo = Utils.Helper.getClaimData(User);
            todoItem.UpdatedDate = DateTime.Now;
            todoItem.UpdatedBy = userInfo.Id;
            todoItem.IsDeleted = true;

            await _db.SaveChangesAsync();

            return todoItem;
        }
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<TodoItem>>> Search([FromQuery] string q)
        {
            return await _db.TodoItems
                         .Where(x => x.IsDeleted == false && x.Name.Contains(q))
                         .ToListAsync();
        }
    }
}
