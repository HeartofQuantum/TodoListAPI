using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using DoToApi.Models;
using System.Linq;

namespace DoToApi.Controllers
{
    [Route("api/[controller]")]
    public class TodoController : Controller
    {
        //private variable that can only be read here
        private readonly TodoContext _context;
             
        //constructor     
        public TodoController(TodoContext context)
        {
            _context = context;

            if (_context.TodoItems.Count() == 0)
            {
                _context.TodoItems.Add(new TodoItem { Name = "Item1"});
                _context.SaveChanges();
            }
        }    


        // gets all the todo items
        [HttpGet]
        public IEnumerable<TodoItem> GetAll()
        {
            return _context.TodoItems.ToList();
        }

        //gets a specific item based on the id and name
        [HttpGet("{id}", Name = "GetTodo")]
        public IActionResult GetById(long id)
        {
            // gets the first item or the default item in the database
            var item = _context.TodoItems.FirstOrDefault( t => t.Id == id);
            // if the item is null and didn't get anything from the _context..... return a 404 page
            if (item == null)
            {
                return NotFound();
            }
            //if there is an item - return it
            return new ObjectResult(item);
        }
        
        [HttpPost]
        public IActionResult Create([FromBody] TodoItem item)
        {
            // item cannot be null ... duh this creates the item
            if (item == null)
            {
                return BadRequest();
            }

            // adds the respective item to the items list and puts it in the db
            _context.TodoItems.Add(item);
            //saves the changes
            _context.SaveChanges();
            //returns a created route
            return CreatedAtRoute("GetTodo", new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
        public IActionResult Update(long id, [FromBody] TodoItem item)
        {
            //if the item is null or if there is no item id return bad request
            if (item == null || item.Id != id)
            {
                return BadRequest();
            }
            // gets the first row or the default
            var todo = _context.TodoItems.FirstOrDefault( t => t.Id ==id);
            
            // if todo is null returns a 404
            if (todo == null)
            {
                return NotFound();
            }
            // sets the item to be complete or not
            todo.IsComplete = item.IsComplete;
            // sets the name to item.name 
            todo.Name = item.Name;

            //updates the corresponding properties
            _context.TodoItems.Update(todo);
            //saves to the db
            _context.SaveChanges();
            
            //
            return new NoContentResult();
        }

        // deletes an item
        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {
            // gets the first row or default row
            var todo = _context.TodoItems.FirstOrDefault( t => t.Id ==id);
            // if the todo is null return 404
            if (todo == null)
            {
                return NotFound();
            }
            // removes an item from the todo list
            _context.TodoItems.Remove(todo);
            //saves the changes to the db
            _context.SaveChanges();
            // doesn't return a page but a status
            return new NoContentResult();
        }

    }
}