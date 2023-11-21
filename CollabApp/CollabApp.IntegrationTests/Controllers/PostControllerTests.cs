using CollabApp.mvc.Context;
using CollabApp.mvc.Controllers;
using CollabApp.mvc.Models;
using CollabApp.mvc.Repo;
using CollabApp.mvc.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace CollabApp.IntegrationTests.Controllers
{
    public class PostControllerTests : IDisposable
    {
        private CollabAppWebApplicationFactory _factory;
        private HttpClient _httpClient;
        public PostControllerTests()
        {
            _factory = new CollabAppWebApplicationFactory();
            _httpClient = _factory.CreateClient();
        }
        [Theory]
        [InlineData("/Home/Index")]
        [InlineData("/Post/Posts")]
        public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
        {
            // Arrange

            // Act
            var response = await _httpClient.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }


        public async Task Add_Post()
        {
            var response = await _httpClient.GetAsync("/Post/Posts");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        public void Dispose()
        {
            _httpClient.Dispose();
            _factory.Dispose();
        }
    }
}
