using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using TodoApi;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using TodoApi.Repository;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using TodoApi.Models;
using Newtonsoft.Json;

namespace TestxUnit
{
    public class IntegrationTestTodoController   
    {
        private HttpClient _client;
        private Uri _url = new Uri(@"http://localhost:61774/api/todo");
   

        public IntegrationTestTodoController()
        {
            var builder = new WebHostBuilder()
               //.UseContentRoot()
               .UseEnvironment("Development")
               .UseStartup<Startup>()
               .UseApplicationInsights();

            var server = new TestServer(builder);
           
            _client = server.CreateClient();
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task GetAllItems()
        {
            var response = await _client.GetAsync(_url);

            response.EnsureSuccessStatusCode();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var responseString = await response.Content.ReadAsStringAsync();
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task ReturnItemIfItIsFound()
        {
            var response = await _client.GetAsync(_url + "/1");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var responseString = await response.Content.ReadAsStringAsync();
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task ReturnNotFoundIfItemIsNotFound()
        {
            var response = await _client.GetAsync(_url + "/100");
           
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

            var responseString = await response.Content.ReadAsStringAsync();
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task Create()
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, _url);
            var item = new TodoItem { Name = "Item 1", IsComplete = false };
            request.Content = new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "applicaiton/json");
            request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task Update()
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, _url + "/1");
            var item = new TodoItem { Id = 1, Name = "Updated name", IsComplete = false };
            request.Content = new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json");
            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task Delete()
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, _url + "/3");
            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
        }
    }
}
