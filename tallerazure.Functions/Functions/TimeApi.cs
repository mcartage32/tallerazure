using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using tallerazure.Common.Models;
using tallerazure.Common.Responses;
using tallerazure.Functions.Entities;

namespace tallerazure.Functions.Functions
{
    public static class TimeApi
    {

        // CREAR ENTRADA
        [FunctionName(nameof(CreateTime))]
        public static async Task<IActionResult> CreateTime(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "time")] HttpRequest req,
            [Table("time", Connection = "AzureWebJobsStorage")] CloudTable timetable,
            ILogger log)
        {
            //LE MANDAMOS UN MENSAJE AL LOG Y LEEMOS EL BODY
            log.LogInformation("Recieved a new time.");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            // ACA CREAMOS UN OBJETO time QUE DESERIALIZA EL JSON DE LO QUE LEYO EL BODY
            Time time = JsonConvert.DeserializeObject<Time>(requestBody);

            // ACA VERIFICAMOS SI EL OBJETO time EN SU CAMPO ID ES NULO
            if (time?.EmployedId == null)
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSucess = false,
                    Message = "The request must have a Id of employed."

                });

            }

            // SI NO ES NULO CREAMOS EL OBJETO time 
            TimeEntity timeEntity = new TimeEntity
            {
                EmployedId = (int)time.EmployedId,
                Date = (DateTime)time.Date,
                Type = (int)time.Type,
                IsConsolidated = false,
                ETag = "*",
                PartitionKey = "TIME",
                RowKey = Guid.NewGuid().ToString(),

            };

            //DEL NUGGET LLAMAMOS LA CLASE TableOperation PARA INSERTAR EL OBJETO DE LA ENTIDAD
            TableOperation addOperation = TableOperation.Insert(timeEntity);
            // EJECUTAMOS LA OPERACION
            await timetable.ExecuteAsync(addOperation);

            //SI TODO FUNCIONO CORRECTAMENTE CREAMOS UN MENSAJE DE BIEN HECHO Y LO CARGAMOS AL LOG (CONSOLA)
            string message = "New time stored in table.";
            log.LogInformation(message);


            // RETORNAMOS UNA RESPUESTA POSITIVA CON QUE SI SE CUMPLIO LA EJECUCION, CON EL MENSAJE Y CON LA ENTIDAD CREADA
            return new OkObjectResult(new Response
            {
                IsSucess = true,
                Message = message,
                Result = timeEntity,


            });
        }

        // EDITAR ENTRADA POR ID
        [FunctionName(nameof(UpdateTime))]
        public static async Task<IActionResult> UpdateTime(
           [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "time/{id}")] HttpRequest req,
           [Table("time", Connection = "AzureWebJobsStorage")] CloudTable timetable,
           string id,
           ILogger log)
        {
            log.LogInformation($"Update for time {id} recieved.");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Time time = JsonConvert.DeserializeObject<Time>(requestBody);

            //VALIDAR EL ID DEL time
            TableOperation findOperation = TableOperation.Retrieve<TimeEntity>("TIME", id);
            TableResult findResult = await timetable.ExecuteAsync(findOperation);

            if (findResult.Result == null)
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSucess = false,
                    Message = "Employed not found."

                });

            }

            //Validacion del campo taskdescription y actualizamos la descripcion y actualizamos a que si se hizo en la propiedad completado

            TimeEntity timeEntity = (TimeEntity)findResult.Result;

            timeEntity.Date = (DateTime)time.Date;
            timeEntity.Type = (int)time.Type;

            //DEL NUGGET LLAMAMOS LA CLASE TableOperation PARA REEMPLAZAR EL OBJETO DE LA ENTIDAD
            TableOperation addOperation = TableOperation.Replace(timeEntity);
            // EJECUTAMOS LA OPERACION
            await timetable.ExecuteAsync(addOperation);

            //SI TODO FUNCIONO CORRECTAMENTE CREAMOS UN MENSAJE DE BIEN HECHO Y LO CARGAMOS AL LOG (CONSOLA)
            string message = $"Employed {id}, update in table.";
            log.LogInformation(message);


            // RETORNAMOS UNA RESPUESTA POSITIVA CON QUE SI SE CUMPLIO LA EJECUCION, CON EL MENSAJE Y CON LA ENTIDAD CREADA
            return new OkObjectResult(new Response
            {
                IsSucess = true,
                Message = message,
                Result = timeEntity,


            });
        }


        //OBTENER TODAS LAS ENTRADAS
        [FunctionName(nameof(GetAllTime))]
        public static async Task<IActionResult> GetAllTime(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "time")] HttpRequest req,
            [Table("time", Connection = "AzureWebJobsStorage")] CloudTable timetable,
            ILogger log)
        {

            log.LogInformation("Get all employeds received.");

            TableQuery<TimeEntity> query = new TableQuery<TimeEntity>();
            TableQuerySegment<TimeEntity> todos = await timetable.ExecuteQuerySegmentedAsync(query, null);


            //SI TODO FUNCIONO CORRECTAMENTE CREAMOS UN MENSAJE DE BIEN HECHO Y LO CARGAMOS AL LOG (CONSOLA)
            string message = "Retrieved all employeds.";
            log.LogInformation(message);


            // RETORNAMOS UNA RESPUESTA POSITIVA CON QUE SI SE CUMPLIO LA EJECUCION, CON EL MENSAJE Y CON LA ENTIDAD CREADA
            return new OkObjectResult(new Response
            {
                IsSucess = true,
                Message = message,
                Result = todos,


            });
        }


        //OBTENER ENTRADA POR ID
        [FunctionName(nameof(GetTimeById))]
        public static IActionResult GetTimeById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "time/{id}")] HttpRequest req,
            [Table("time", "TIME", "{id}", Connection = "AzureWebJobsStorage")] TimeEntity timeEntity,
            string id,
            ILogger log)
        {
            //LE MANDAMOS UN MENSAJE AL LOG Y VALIDAMOS SI EL OBJETO todoEntity NO LLEGO NULO
            log.LogInformation($"Get employed by id:{id}, received.");

            if (timeEntity == null)
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSucess = false,
                    Message = "Todo not found."

                });

            }


 
            string message = $"Employed {timeEntity.RowKey} retrieved.";
            log.LogInformation(message);


            
            return new OkObjectResult(new Response
            {
                IsSucess = true,
                Message = message,
                Result = timeEntity,


            });
        }


        //BORRAR POR ID DEL REGISTRO
        [FunctionName(nameof(DeleteTime))]
        public static async Task<IActionResult> DeleteTime(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "time/{id}")] HttpRequest req,
            [Table("time", "TIME","{id}", Connection = "AzureWebJobsStorage")] TimeEntity timeEntity,
            [Table("time", Connection = "AzureWebJobsStorage")] CloudTable todotable,
            string id,
            ILogger log)
        {
            
            log.LogInformation($"Delete employed :{id}, received.");

            if (timeEntity == null)
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSucess = false,
                    Message = "Todo not found."

                });

            }

            await todotable.ExecuteAsync(TableOperation.Delete(timeEntity));
            string message = $"Employed {timeEntity.RowKey} deleted.";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                IsSucess = true,
                Message = message,
                Result = timeEntity,


            });
        }

    }
}
