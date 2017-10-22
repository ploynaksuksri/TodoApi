using System.Collections.Generic;
using System.Threading.Tasks;
using TodoApi.Models;

namespace TodoApi.Repository
{
    public interface ITodoItemsRepository
    {
        Task<IEnumerable<TodoItem>> GetAll();
        Task<TodoItem> GetById(long id);
        Task<TodoItem> Create(TodoItem item);
        Task Update(TodoItem item);
        Task Delete(long id);
    }
}