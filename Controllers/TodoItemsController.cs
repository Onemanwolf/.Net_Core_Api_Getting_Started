using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using TodoApi.Models;
using TodoApi.Repository;

namespace TodoApi.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly IRepository<TodoItem> _repo;

        public TodoItemsController(IRepository<TodoItem> repo)
        {
            _repo = repo;
        }

        // GET: api/TodoItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItems()
        {
           
            return await _repo.GetAsync();
        }

        /// <summary>
        /// Gets a specific TodoItem.
        /// </summary>
        /// <param name="id"></param>  
        /// <response code="200">Returns the newly created item</response>
        /// <response code="404">If the item is null</response>         
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> GetTodoItem(long id)
        {
            
            var todoItem = await _repo.GetAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            return todoItem;
        }

        // PUT: api/TodoItems/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(long id, TodoItem todoItem)
        {
            if (id != todoItem.Id)
            {
                return BadRequest();
            }



            try
            {
                await _repo.UpdateAsync(todoItem);

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await TodoItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Creates a TodoItem.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /Todo
        ///     {
        ///       
        ///        "name": "Item1",
        ///        "isComplete": true
        ///        "
        ///     }
        ///
        /// </remarks>
        /// <param name="item"></param>
        /// <returns>A newly created TodoItem</returns>
        /// <response code="201">Returns the newly created item</response>
        /// <response code="400">If the item is null</response>         
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TodoItem>> Create(TodoItem todoItem)
        {
            
           await _repo.InsertAsync(todoItem);

            return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, todoItem);
        }

        /// <summary>
        /// Deletes a specific TodoItem.
        /// </summary>
        /// <param name="id"></param>    
        [HttpDelete("{id}")]
        public async Task<ActionResult<TodoItem>> DeleteTodoItem(long id)
        {
            var todoItem = await _repo.GetAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            await _repo.DeleteAsync(todoItem);

            return todoItem;
        }

        private async Task<bool> TodoItemExists(long id)
        {
            var todoItem = await _repo.GetAsync(id);

            if (todoItem == null)
            { 
                return false; 
            }
            else
            {
                return true;
            }


        }
    }
}
