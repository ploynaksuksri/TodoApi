using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoApi.Repository;
using TodoApi.Models;
using System.Net;

namespace TodoApi.Controllers
{
    /// <summary>
    /// Todo Api
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class TodoController : Controller
    {
    
        private ITodoItemsRepository _repo; 

        public TodoController(ITodoItemsRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Get list of Todo items
        /// </summary>
        /// <returns>List of Todo items</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<TodoItem>),200)]
        public async Task<IEnumerable<TodoItem>> GetAll() 
        {
            return await _repo.GetAll();
        }

        /// <summary>
        /// Get Todo item with specific Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="404">If the item is not found</response>
        [HttpGet("{id}", Name="GetToDo")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [Produces("application/json", Type = typeof(TodoItem))]
        public async Task<IActionResult> GetById(long id)
        {
            TodoItem item = await _repo.GetById(id);
            if (item == null)
            {
                return NotFound();
            }

            return new ObjectResult(item);
        } 

        /// <summary>
        /// Create a new Todo item
        /// </summary>
        /// <param name="item"></param>
        /// <returns>a newly created Todo item</returns>
        /// <response code="201">Returns the newly-created item</response>
        /// <response code="400">If the item is null</response>          
        [HttpPost]
        [ProducesResponseType(typeof(TodoItem), 201)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateItem([FromBody]TodoItem item)
        {
            if (item == null)
            {
                return BadRequest();
            }
            item = await _repo.Create(item);
            return CreatedAtRoute("GetToDo", new {id = item.Id}, item);

        }

        /// <summary>
        /// Update Todo item
        /// </summary>
        /// <param name="id"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        /// <response code="400">If the item is null or id isn't matched</response>
        /// <response code="404">If the item is not found</response>
        /// <response code="204">Item is updated successfully</response>
        [HttpPut("{id}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
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
            var result = await _repo.Update(todo);
            if (result)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
            
                      
        }

        /// <summary>
        /// Delete item with specific id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="404">If the item is not found</response>
        /// <response code="204">If the item is deleted<response>
        [HttpDelete("{id}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Delete(long id)
        {
            var todo = await _repo.GetById(id);

            if (todo == null)
            {
                return NotFound();
            }

            var result = await _repo.Delete(id);
            if (result)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
            
        }
    }
}