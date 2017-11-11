using System;
using Xunit;
using TodoApi.Controllers;
using TodoApi.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using TodoApi.Repository;
using Moq;
using System.Threading.Tasks;

namespace Test_xUnit
{
    public class TodoControllerShould
    {
        // private TodoContext _context;
        private Mock<ITodoItemsRepository> _mockRepo;
        private List<TodoItem> _items;


        public TodoControllerShould()
        {
            //var options = new DbContextOptionsBuilder<TodoContext>()
            //    .UseInMemoryDatabase(databaseName: "Add_writes_to_database")
            //    .Options;
            //_context = new TodoContext(options);
            

            _mockRepo = new Mock<ITodoItemsRepository>();
            _items = new List<TodoItem>()
            {
                new TodoItem { Id = 1, Name = "Item 1", IsComplete = false },
                new TodoItem { Id = 2, Name = "Item 2", IsComplete = true }
            };
            
        } 

        

        [Fact]
        public void ReturnAllItems()
        {
           _mockRepo.Setup(x => x.GetAll())
                    .ReturnsAsync(_items);

           var controller = new TodoController(_mockRepo.Object);         
           var items = controller.GetAll().Result.ToList();

            _mockRepo.Verify(x => x.GetAll(), Times.Once);

           Assert.Equal(_items.Count(), items.Count());          
        }

        [Fact]
        public void ReturnItemWithGivenId()
        {            
            _mockRepo.Setup(x => x.GetById(1))
                     .ReturnsAsync(_items.FirstOrDefault(e => e.Id == 1));

            var controller = new TodoController(_mockRepo.Object);
            var result = controller.GetById(1).Result;

            var resultType = Assert.IsType<ObjectResult>(result);
            var item = (TodoItem)resultType.Value;

            _mockRepo.Verify(x => x.GetById(1), Times.Once);
            Assert.Equal(1, item.Id);
        }

        [Fact]
        public void ReturnNotFoundResultIfItemIsNotFound()
        {
            var id = 10;
            TodoItem item = null;
            _mockRepo.Setup(x => x.GetById(id))
                     .ReturnsAsync(item);

            var controller = new TodoController(_mockRepo.Object);
            var result = controller.GetById(id).Result;

            _mockRepo.Verify(x => x.GetById(id), Times.Once);

            Assert.IsType<NotFoundResult>(result);         
        }

        
        [Fact]
        public void CreateNewItemSuccessfully()
        {
            TodoItem newItem = new TodoItem { Id = 3, Name = "Item 3", IsComplete = false };
            _mockRepo.Setup(x => x.Create(newItem))
                     .ReturnsAsync(newItem);

            var controller = new TodoController(_mockRepo.Object);
            var result = controller.CreateItem(newItem).Result;

            var resultType = Assert.IsType<CreatedAtRouteResult>(result);
            var item = (TodoItem)resultType.Value;

            _mockRepo.Verify(x => x.Create(newItem), Times.Once);
      
            Assert.Equal(newItem.Name, item.Name);
        }

        
        [Fact]
        public void ReturnBadRequestIfItemToCreateIsNull()
        {   
            var controller = new TodoController(_mockRepo.Object);

            var result = controller.CreateItem(null).Result;

            Assert.IsType<BadRequestResult>(result);
        }

        
        [Fact]
        public async Task UpdateItemSuccessfully()
        {  
            var updateItem = new TodoItem { Id = 4, Name = "Updated item", IsComplete = true };
            _mockRepo.Setup(x => x.Update(updateItem)).ReturnsAsync(true);
            _mockRepo.Setup(x => x.GetById(4))
                     .ReturnsAsync(updateItem);

            var controller = new TodoController(_mockRepo.Object);
            
            var result = await controller.Update(updateItem.Id, updateItem);
        
            _mockRepo.Verify(x => x.Update(updateItem), Times.Once);

            Assert.IsType<OkResult>(result);
        }
      

        [Fact]
        public async Task ReturnBadRequestWhenItemToUpdateIsNull()
        {       
            var controller = new TodoController(_mockRepo.Object);
            var result = controller.Update(3, null).Result;
            Assert.IsType<BadRequestResult>(result);
        }


          
        [Fact]
        public void ReturnBadRequestWhenIdOfItemToUpdateAndIdAreNotMatched()
        {
            var item = new TodoItem { Id = 1 };
            var controller = new TodoController(_mockRepo.Object);
            var result = controller.Update(item.Id - 1, item).Result;
            Assert.IsType<BadRequestResult>(result);
        }

        

        [Fact]
        public void ReturnNotFoundResultWhenItemToUpdateIsNotFoundInDB()
        {
            _mockRepo.Setup(x => x.GetById(It.IsAny<long>())).ReturnsAsync((TodoItem)null);
        
            var controller = new TodoController(_mockRepo.Object);
            var result = controller.Update(3, new TodoItem { Id = 3 }).Result;

            _mockRepo.Verify(x => x.GetById(3), Times.Once);

            Assert.IsType<NotFoundResult>(result);
        }
        
        [Fact]
        public async Task ShouldReturnOkIfItemIsDeleted()
        {
            _mockRepo.Setup(x => x.GetById(It.IsAny<long>())).ReturnsAsync(new TodoItem());
            _mockRepo.Setup(x => x.Delete(It.IsAny<long>())).ReturnsAsync(true);
            var controller = new TodoController(_mockRepo.Object);

            var result = await controller.Delete(1);

            _mockRepo.Verify(x => x.Delete(1), Times.Once);

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task ShouldResultNotFoundIfItemToDeleteIsNotFound()
        {
            _mockRepo.Setup(x => x.GetById(It.IsAny<long>())).ReturnsAsync((TodoItem)null);

            var controller = new TodoController(_mockRepo.Object);

            var result = await controller.Delete(3);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task ShouldReturnBadRequestWhenFailToDeleteItem()
        {
            _mockRepo.Setup(x => x.GetById(It.IsAny<long>())).ReturnsAsync(new TodoItem { Id = 1 });
            _mockRepo.Setup(x => x.Delete(It.IsAny<long>())).ReturnsAsync(false);

            var controller = new TodoController(_mockRepo.Object);

            var result = await controller.Delete(1);

            _mockRepo.Verify(x => x.Delete(1), Times.Once);

            Assert.IsType<BadRequestResult>(result);

        }


    }
}
