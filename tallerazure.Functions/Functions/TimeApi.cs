using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage.Table;
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
            [Table("todo", Connection = "AzureWebJobsStorage")] CloudTable todotable,
            ILogger log)
        {
            //LE MANDAMOS UN MENSAJE AL LOG Y LEEMOS EL BODY
            log.LogInformation("Recieved a new todo.");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            // ACA CREAMOS UN OBJETO time QUE DESERIALIZA EL JSON DE LO QUE LEYO EL BODY
            Time time = JsonConvert.DeserializeObject<Time>(requestBody);

            // ACA VERIFICAMOS SI EL OBJETO time EN SU CAMPO ID ES NULO
            if (time?.EmployedId ==null)
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSucess = false,
                    Message = "The request must have a TaskDescription."

                });

            }

            // SI NO ES NULO CREAMOS EL OBJETO time 
            TimeEntity timeEntity = new TimeEntity
            {
                EmployedId = (int)time.EmployedId,
                Date = DateTime.UtcNow,
                Type = (int)time.Type,
                IsConsolidated = false,
                ETag = "*",
                PartitionKey = "TODO",
                RowKey = Guid.NewGuid().ToString(),
                
            };

            //DEL NUGGET LLAMAMOS LA CLASE TableOperation PARA INSERTAR EL OBJETO DE LA ENTIDAD
            TableOperation addOperation = TableOperation.Insert(timeEntity);
            // EJECUTAMOS LA OPERACION
            await todotable.ExecuteAsync(addOperation);

            //SI TODO FUNCIONO CORRECTAMENTE CREAMOS UN MENSAJE DE BIEN HECHO Y LO CARGAMOS AL LOG (CONSOLA)
            string message = "New todo stored in table.";
            log.LogInformation(message);


            // RETORNAMOS UNA RESPUESTA POSITIVA CON QUE SI SE CUMPLIO LA EJECUCION, CON EL MENSAJE Y CON LA ENTIDAD CREADA
            return new OkObjectResult(new Response
            {
                IsSucess = true,
                Message = message,
                Result = timeEntity,


            });
        }




    }
}
