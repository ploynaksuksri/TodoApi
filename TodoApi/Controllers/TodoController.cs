using Microsoft.AspNetCore.Mvc;
using TodoApi.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    public class TodoController : Controller
    {
        private TodoContext _context;

        public TodoController(TodoContext context)
        {
            _context = context;
            
            if (_context.TodoItems.Count() == 0)
            {
                _context.TodoItems.Add(new TodoItem { Name = "Test" });
                _context.SaveChanges();
            }
        }

        [HttpGet]
        public IEnumerable<TodoItem> GetAll() 
        {
            return _context.TodoItems.ToList();
        }

        [HttpGet("{id}", Name="GetToDo")]
        public IActionResult GetById(long id)
        {
            TodoItem item = _context.TodoItems.FirstOrDefault(e => e.Id == id);
            if (item == null)
            {
                return NotFound();
            }

            return new ObjectResult(item);
        } 

        [HttpPost]
        public IActionResult CreateItem([FromBody]TodoItem item)
        {
            if (item == null)
            {
                return BadRequest();
            }

            _context.TodoItems.Add(item);
            _context.SaveChanges();
            return CreatedAtRoute("GetToDo", new {id = item.Id}, item);

        }

        [HttpPut("{id}")]
        public IActionResult Update(long id, [FromBody]TodoItem item)
        {
            if (item == null || item.Id != id)
            {
                return BadRequest();
            }

            var todo = _context.TodoItems.FirstOrDefault(e => e.Id == id);
            if (todo == null)
            {
                return NotFound();
            }

            todo.Name = item.Name;
            todo.IsComplete = item.IsComplete;
            _context.TodoItems.Update(todo);
            _context.SaveChanges();
            return new NoContentResult();            
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {
            var todo = _context.TodoItems.FirstOrDefault(e => e.Id == id);

            if (todo == null)
            {
                return NotFound();
            }

            _context.TodoItems.Remove(todo);
            _context.SaveChanges();
            return new NoContentResult();
        }
    }
}