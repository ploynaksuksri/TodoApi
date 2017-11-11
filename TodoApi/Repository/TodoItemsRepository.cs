using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Repository
{   
    public class TodoItemsRepository : ITodoItemsRepository
    {
        private TodoContext _context;

        public TodoItemsRepository(TodoContext context)
        {
            _context = context;
             
            if (_context.TodoItems.Count() == 0)
            {
                _context.TodoItems.Add(new TodoItem { Name = "Test" });
                _context.SaveChanges();
            }
        }

        public async Task<IEnumerable<TodoItem>> GetAll()
        {
            return await _context.TodoItems.ToListAsync();
        }

        public async Task<TodoItem> GetById(long id)
        {
            return await _context.TodoItems.SingleOrDefaultAsync(e => e.Id == id);
        }

        public async Task<TodoItem> Create(TodoItem item)
        {
            _context.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<bool> Update(TodoItem item)
        {
            _context.Attach(item);
            _context.Entry(item).State = EntityState.Modified;
            return await _context.SaveChangesAsync() > 0 ? true : false;
        }

        public async Task<bool> Delete(long id)
        {
            var item = await _context.TodoItems.SingleOrDefaultAsync(e => e.Id == id);
            _context.Remove(item);
            return await _context.SaveChangesAsync() > 0 ? true : false;
        }
    }
    
}