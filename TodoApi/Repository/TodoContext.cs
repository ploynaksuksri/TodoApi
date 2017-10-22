using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Repository
{
    public class TodoContext : DbContext
    {
        public TodoContext(DbContextOptions<TodoContext> options) : base(options)
        {
            
        }

        public DbSet<TodoItem> TodoItems {get;set;}

    }
}