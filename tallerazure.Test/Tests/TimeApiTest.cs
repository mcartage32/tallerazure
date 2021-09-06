using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;
using tallerazure.Common.Models;
using tallerazure.Functions.Entities;
using tallerazure.Functions.Functions;
using tallerazure.Test.Helpers;
using Xunit;

namespace tallerazure.Test.Tests
{
    public class TimeApiTest
    {
        private readonly ILogger logger = TestFactory.CreateLogger();

        //Prueba unitaria create
        [Fact]
        public async void CreateTime_Should_Return_200()
        {
            // Arrage (preparar prueba unitaria)
            MockCloudTableTimes mockTime = new MockCloudTableTimes(new Uri("htpp://127.0.0.1:10002/devstoreaccount1/reports"));
            Time timeRequest = TestFactory.GetTimeRequest();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(timeRequest);

            // Act (ejecutar prueba unitaria)
            IActionResult response = await TimeApi.CreateTime(request, mockTime, logger);

            // Assert (verificar si la prueba unitario dio el resultado correcto)
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

        //Prueba unitaria update
        [Fact]
        public async void UpdateTime_Should_Return_200()
        {
            // Arrage (preparar prueba unitaria)
            MockCloudTableTimes mockTime = new MockCloudTableTimes(new Uri("htpp://127.0.0.1:10002/devstoreaccount1/reports"));
            Guid timeId = Guid.NewGuid();
            Time timeRequest = TestFactory.GetTimeRequest();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(timeId, timeRequest);

            // Act (ejecutar prueba unitaria)
            IActionResult response = await TimeApi.UpdateTime(request, mockTime, timeId.ToString(), logger);

            // Assert (verificar si la prueba unitario dio el resultado correcto)
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }


        //Prueba unitaria getbyid
        [Fact]
        public async void GetByIdTimeShould_Return_200()
        {
            // Arrage (preparar prueba unitaria)
            MockCloudTableTimes mockTimes = new MockCloudTableTimes(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            Time timeRequest = TestFactory.GetTimeRequest();
            Guid id = Guid.NewGuid();

            TableOperation find = TableOperation.Retrieve<TimeEntity>("TIME", id.ToString());
            TableResult action = await mockTimes.ExecuteAsync(find);
            TimeEntity timeEntity = (TimeEntity)action.Result;

            DefaultHttpRequest request = TestFactory.CreateHttpRequest(id, timeRequest);

            // Act (ejecutar prueba unitaria)
            IActionResult response = TimeApi.GetTimeById(request, timeEntity, id.ToString(), logger);

            // Assert (verificar si la prueba unitario dio el resultado correcto)
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);

        }


        [Fact]
        public async void GetAllTimeShould_Return_200()
            {

            // Arrage (preparar prueba unitaria)
            MockCloudTableTimes mockTimes = new MockCloudTableTimes(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            List<Time> timesList = TestFactory.GetTimesList();
            DefaultHttpRequest request = TestFactory.CreateHttpRequestList(timesList);

            // Act (ejecutar prueba unitaria)
            IActionResult response = await TimeApi.GetAllTime(request, mockTimes, logger);

            // Assert (verificar si la prueba unitario dio el resultado correcto)
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);

        }


        //Prueba unitaria update
        [Fact]
        public async void DeleteTime_Should_Return_200()
        {
            // Arrage (preparar prueba unitaria)
            Guid timeId = Guid.NewGuid();
            Time timeRequest = TestFactory.GetTimeRequest(); // creamos el objeto time
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(timeId, timeRequest);

            TimeEntity timeEntity = TestFactory.GetTimeEntity();

            MockCloudTableTimes mockTime = new MockCloudTableTimes(new Uri("htpp://127.0.0.1:10002/devstoreaccount1/reports"));

            // Act (ejecutar prueba unitaria)
            IActionResult response = await TimeApi.DeleteTime(request, timeEntity, mockTime, timeId.ToString(), logger);

            // Assert (verificar si la prueba unitario dio el resultado correcto)
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

 

    }
}
