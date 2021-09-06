using System;
using System.Collections.Generic;
using System.Text;
using tallerazure.Functions.Functions;
using tallerazure.Test.Helpers;
using Xunit;

namespace tallerazure.Test.Tests
{
    public class ScheduledTest
    {
        [Fact]
        public void ScheduledFuctionTest()
        {
           
  MockCloudTableTimes mockTimes = new MockCloudTableTimes(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
  MockCloudTableConsolidates mockConsolidated = new MockCloudTableConsolidates(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
 ListLogger logger = (ListLogger)TestFactoryConsolidated.CreateLogger(LoggerTypes.List);

         
            ScheduledFunction.Run(null, mockTimes, mockConsolidated, logger);
            string message = logger.Logs[0];

          
            Assert.Contains($"Trigger Consolidated procces executed at: {DateTime.Now}", message);
        }


    }
}
