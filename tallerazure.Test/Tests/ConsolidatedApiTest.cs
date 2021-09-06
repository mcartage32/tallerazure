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
    public class ConsolidatedApiTest
    {

        private readonly ILogger logger = TestFactoryConsolidated.CreateLogger();

        [Fact]
        public async void GetAConsolidatedByDate_Should_Return_200()
        {
            
            MockCloudTableConsolidates mockConsolidate = new MockCloudTableConsolidates(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            List<Consolidated> consolidatedRequest = TestFactoryConsolidated.GetConsolidatedsRequest();
            DefaultHttpRequest request = TestFactoryConsolidated.CreateHttpRequest(consolidatedRequest);

            IActionResult response = await ConsolidatedApi.GetConsolidatedByDate(request, mockConsolidate,"2021-08-10", logger);

            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }


    }



}
