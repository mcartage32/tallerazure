using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using tallerazure.Common.Models;
using tallerazure.Functions.Functions;
using tallerazure.Test.Helpers;
using Xunit;

namespace tallerazure.Test.Tests
{
    public class TimeApiTest
    {
        private readonly ILogger logger = TestFactory.CreateLogger();

        //PRUEBA UNITARIA PARA EL CREATE
        [Fact]
        public async void CreateTime_Should_Return_200()
        {
            // Arrage (preparar prueba unitaria)
            MockCloudTableTodos mockTodos = new MockCloudTableTodos(new Uri("htpp://127.0.0.1:10002/devstoreaccount1/reports"));
            Time todoRequest = TestFactory.GetTimeRequest();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(todoRequest);

            // Act (ejecutar prueba unitaria)
            IActionResult response = await TimeApi.CreateTime(request, mockTodos, logger);

            // Assert (verificar si la prueba unitario dio el resultado correcto)
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

        [Fact]
        public async void UpdateTime_Should_Return_200()
        {
            // Arrage (preparar prueba unitaria)
            MockCloudTableTodos mockTodos = new MockCloudTableTodos(new Uri("htpp://127.0.0.1:10002/devstoreaccount1/reports"));
            Guid todoId = Guid.NewGuid();
            Time todoRequest = TestFactory.GetTimeRequest();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(todoId, todoRequest);

            // Act (ejecutar prueba unitaria)
            IActionResult response = await TimeApi.UpdateTime(request, mockTodos, todoId.ToString(), logger);

            // Assert (verificar si la prueba unitario dio el resultado correcto)
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }


    }
}
