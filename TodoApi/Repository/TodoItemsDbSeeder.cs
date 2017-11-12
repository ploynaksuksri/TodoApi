using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoApi.Models;

namespace TodoApi.Repository
{
    /// <summary>
    /// Database eseeder
    /// </summary>
    public class TodoItemsDbSeeder
    {
        /// <summary>
        /// Database seeder
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public async Task SeedAsync(IServiceProvider serviceProvider)
        {
            //Based on EF team's example at https://github.com/aspnet/MusicStore/blob/dev/samples/MusicStore/Models/SampleData.cs
            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var todoDb = serviceScope.ServiceProvider.GetService<TodoContext>();
                if (await todoDb.Database.EnsureCreatedAsync())
                {
                    if (!await todoDb.TodoItems.AnyAsync())
                    {
                        await InsertTodoItemsSampleData(todoDb);
                    }
                }
            }
        }

        /// <summary>
        /// Insert sample Todo item
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public async Task InsertTodoItemsSampleData(TodoContext db)
        {
            db.TodoItems.Add(new TodoItem { Name = "Item 1", IsComplete = false });
            db.TodoItems.Add(new TodoItem { Name = "Item 2", IsComplete = false });
            db.TodoItems.Add(new TodoItem { Name = "Item 3", IsComplete = false });
            await db.SaveChangesAsync();
        }
    }
}
