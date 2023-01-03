using System;
using System.Collections.Generic;
using System.Linq; //Language Integrated Query
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly TodoContext _context;

        public TodoItemsController(TodoContext context)
        {
            _context = context;
        }

        // GET: api/TodoItems
        [HttpGet] //SELECT all
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItems() //'async' controller to handle paralelley controls multiple apis' requests | Synchronous for single api handle
        { //'Task' - internally tracks the authentic caller and returns back 
          if (_context.TodoItems == null)
          {
              return NotFound();
          }
            return await _context.TodoItems.ToListAsync(); //'async' must conmes with await | for eg- R1 reqest await wit R1 response
        } // '_context.TodoItems' - SELECT * FRON TodoItems  | can use multiple 'await' but not good practice

        // GET: api/TodoItems/5
        [HttpGet("{id}")] //SELECT by ID || 
        public async Task<ActionResult<TodoItem>> GetTodoItem(long id)
        {
          if (_context.TodoItems == null)
          {
              return NotFound();
          }
            var todoItem = await _context.TodoItems.FindAsync(id); //WHERE id = 5

            if (todoItem == null)
            {
                return NotFound();
            }

            return todoItem;
        }

        // PUT: api/TodoItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")] /// SELECT with idt and object
        public async Task<IActionResult> PutTodoItem(long id, TodoItem todoItem)
        {
            if (id != todoItem.Id)
            {
                return BadRequest();
            }

            _context.Entry(todoItem).State = EntityState.Modified;//generates query for Modified

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoItemExists(id))
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

        // POST: api/TodoItems  || use POST for Create
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItem todoItem)
        {
          if (_context.TodoItems == null)
          {
              return Problem("Entity set 'TodoContext.TodoItems'  is null.");
          }
            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();/// generated insert query and saveChanges

            return CreatedAtAction("GetTodoItem", new { id = todoItem.Id }, todoItem);//creates and return back the id
        }

        // DELETE: api/TodoItems/5
        [HttpDelete("{id}")] //DELETE by id == id of parameter
        public async Task<IActionResult> DeleteTodoItem(long id)
        {
            if (_context.TodoItems == null)
            {
                return NotFound();
            }
            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem == null)
            {
                return NotFound();
            }

            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();/// generated delete query and saveChanges

            return NoContent(); 
        }

        private bool TodoItemExists(long id)
        {
            return (_context.TodoItems?.Any(e => e.Id == id)).GetValueOrDefault(); //Any give boolean value || 
        }
    }
}
