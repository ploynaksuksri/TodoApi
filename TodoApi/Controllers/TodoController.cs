using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoApi.Repository;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    public class TodoController : Controller
    {
    
        private ITodoItemsRepository _repo; 

        public TodoController(ITodoItemsRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IEnumerable<TodoItem>> GetAll() 
        {
            return await _repo.GetAll();
        }

        [HttpGet("{id}", Name="GetToDo")]
        public async Task<IActionResult> GetById(long id)
        {
            TodoItem item = await _repo.GetById(id);
            if (item == null)
            {
                return NotFound();
            }

            return new ObjectResult(item);
        } 

        [HttpPost]
        public async Task<IActionResult> CreateItem([FromBody]TodoItem item)
        {
            if (item == null)
            {
                return BadRequest();
            }
            item = await _repo.Create(item);
            return CreatedAtRoute("GetToDo", new {id = item.Id}, item);

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody]TodoItem item)
        {
            if (item == null || item.Id != id)
            {
                return BadRequest();
            }

            var todo = await _repo.GetById(id);
            if (todo == null)
            {
                return NotFound();
            }

            todo.Name = item.Name;
            todo.IsComplete = item.IsComplete;
            await _repo.Update(todo);
            return new NoContentResult();            
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var todo = await _repo.GetById(id);

            if (todo == null)
            {
                return NotFound();
            }

            await _repo.Delete(id);
            return new NoContentResult();
        }
    }
}