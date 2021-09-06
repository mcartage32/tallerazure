using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using tallerazure.Common.Models;
using tallerazure.Functions.Entities;

namespace tallerazure.Test.Helpers
{
    public class TestFactoryConsolidated
    {
       
        public static DefaultHttpRequest CreateHttpRequest(Consolidated consolidatedRequest)
        {
            string request = JsonConvert.SerializeObject(consolidatedRequest);
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = GenerateStreamFromString(request)
            };
        }

        public static DefaultHttpRequest CreateHttpRequest(List<Consolidated> consolidatedRequest)
        {
            string request = JsonConvert.SerializeObject(consolidatedRequest, Formatting.Indented);
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = GenerateStreamFromString(request)
            };
        }

        public static DefaultHttpRequest CreateHttpRequest()
        {
            return new DefaultHttpRequest(new DefaultHttpContext());
        }

        public static Consolidated GetConsolidatedRequest()
        {
            return new Consolidated
            {   EmployedId=123,
                Date = DateTime.Now,
                MinutesWork = 800,
                
            };
        }

        public static List<Consolidated> GetConsolidatedsRequest()
        {
            return new List<Consolidated>
            {
                new Consolidated
                {   EmployedId=123,
                    Date = DateTime.UtcNow,
                    MinutesWork = 8000,
                    
                },
                new Consolidated
                {   EmployedId=456,
                    Date = DateTime.UtcNow,
                    MinutesWork = 8001, 
                }
            };
        }



        public static Stream GenerateStreamFromString(string stringToConvert)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(stringToConvert);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }


        public static ILogger CreateLogger(LoggerTypes type = LoggerTypes.Null)
        {
            ILogger logger;
            if (type == LoggerTypes.List)
            {
                logger = new ListLogger();
            }
            else
            {
                logger = NullLoggerFactory.Instance.CreateLogger("Null Logger");
            }
            return logger;
        }


    }
}
