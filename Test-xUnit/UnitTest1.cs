using System;
using Xunit;
using TodoApi.Controllers;
using TodoApi.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace Test_xUnit
{
    public class UnitTest1
    {
        private TodoContext _context;
        public UnitTest1()
        {
            var options = new DbContextOptionsBuilder<TodoContext>()
                .UseInMemoryDatabase(databaseName: "Add_writes_to_database")
                .Options;
            _context = new TodoContext(options);
            
            
        } 

        

        [Fact]
        public void Test_GetAll()
        {
           var controller = new TodoController(_context);         
           var items = controller.GetAll().ToList();
           Assert.Equal(_context.TodoItems.Count(), items.Count());          
        }

        [Fact]
        public void Test_GetById_ExistingItem()
        {
            var controller = new TodoController(_context);
            var result = controller.GetById(1);
            var resultType = Assert.IsType<ObjectResult>(result);
            var item = (TodoItem)resultType.Value;
            Assert.Equal(1, item.Id);
        }

        [Fact]
        public void Test_GetById_NonExisitngItem()
        {
            var controller = new TodoController(_context);
            var result = controller.GetById(2);
            Assert.IsType<NotFoundResult>(result);         
        }

        [Fact]
        public void Test_CreateItem_SuccessCase()
        {
            var controller = new TodoController(_context);
            var result = controller.CreateItem(new TodoItem { Name = "Test Create"});
            var resultType = Assert.IsType<CreatedAtRouteResult>(result);
            var item = (TodoItem)resultType.Value;
            Assert.Equal("Test Create", item.Name);
        }

        [Fact]
        public void Test_CreateItem_FailCase()
        {
            var controller = new TodoController(_context);
            var result = controller.CreateItem(null);
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void Test_Update_SuccessCase()
        {
            var lastItem = (TodoItem)_context.TodoItems.LastOrDefault();
            var controller = new TodoController(_context);
            lastItem.Name = "Updated item";
            var result = controller.Update(lastItem.Id, lastItem);
            Assert.IsType<NoContentResult>(result);
        }
        
        [Fact]
        public void Test_Update_FailCase_NullItem()
        {
            var lastItem = (TodoItem)_context.TodoItems.LastOrDefault();
            var controller = new TodoController(_context);
            var result = controller.Update(lastItem.Id, null);
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void Test_Update_FailCase_MismatchId()
        {
            var lastItem = (TodoItem)_context.TodoItems.LastOrDefault();
            var controller = new TodoController(_context);
            var result = controller.Update(lastItem.Id - 1, lastItem);
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void Test_Update_FailCase_NonExistingItem()
        {
            var lastItem = (TodoItem)_context.TodoItems.LastOrDefault();
            var fakeItem = new TodoItem{ Id = lastItem.Id + 1 };
            var controller = new TodoController(_context);
            var result = controller.Update(lastItem.Id + 1, fakeItem);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Test_Delete_SuccessCase()
        {
            var item = new TodoItem{ Name = "Item to test delete" };
            var controller = new TodoController(_context);
            item = (TodoItem)((CreatedAtRouteResult)controller.CreateItem(item)).Value;
            var deleteResult = controller.Delete(item.Id);
            Assert.IsType<NoContentResult>(deleteResult);

            var getResult = controller.GetById(item.Id);
            Assert.IsType<NotFoundResult>(getResult);
        }

        [Fact]
        public void Test_Delete_FailCase()
        {
            var lastItem = _context.TodoItems.LastOrDefault();
            var controller = new TodoController(_context);
            var result = controller.Delete(lastItem.Id + 1);
            Assert.IsType<NotFoundResult>(result);

        }


    }
}
