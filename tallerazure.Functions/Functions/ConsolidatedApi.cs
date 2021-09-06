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
using tallerazure.Functions.Entities;
using System.Collections.Generic;
using System.Linq;
using tallerazure.Common.Responses;

namespace tallerazure.Functions.Functions
{
    public static class ConsolidatedApi
    {
        [FunctionName(nameof(ConsolidatedProcess))]
        public static async Task<IActionResult> ConsolidatedProcess(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "consolidated")] HttpRequest req,
            [Table("time", Connection = "AzureWebJobsStorage")] CloudTable timeTable,
            [Table("consolidated", Connection = "AzureWebJobsStorage")] CloudTable consolidatedTable,
            ILogger log)
        {
            int newc= 0;
            int update = 0;

            log.LogInformation($"Recieved a new consolidated process, starts at:{ DateTime.Now} ");

            // filtro para traer los no consolidados
            string filter = TableQuery.GenerateFilterConditionForBool("IsConsolidated", QueryComparisons.Equal, false); 
            //generamos la consulta con el filtro y lo casteamos como un timeentity
            TableQuery<TimeEntity> queryTime = new TableQuery<TimeEntity>().Where(filter);
            //ejecutamos la consulta y estos serian los registros no consolidados sin ordenar
            TableQuerySegment<TimeEntity> messyTimes = await timeTable.ExecuteQuerySegmentedAsync(queryTime, null);

            //ordenamos la consulta anterior por fecha y id del empleado
            List<TimeEntity> orderedTimes = messyTimes.OrderBy(x => x.EmployedId).ThenBy(x => x.Date).ToList();

       
            if (orderedTimes.Count > 1)
            {
                int i; 
                for (i = 0; i < orderedTimes.Count;)
                {
                  

                    if (orderedTimes[i].EmployedId==orderedTimes[i+1].EmployedId)
                    {
                        // resto las fechas lo cual es el tiempo trabajado
                        TimeSpan worktime = orderedTimes[i + 1].Date - orderedTimes[i].Date;
                        //creo la fecha donde se hizo la consolidacion
                        DateTime dateconsolidated = new DateTime(orderedTimes[i].Date.Year, orderedTimes[i].Date.Month, orderedTimes[i].Date.Day);

                        //filtramos en la tabla consolidado el id donde estamos parados
                        string filterid = TableQuery.GenerateFilterConditionForInt("EmployedId", QueryComparisons.Equal, orderedTimes[i].EmployedId);
                        //filtramos en la tabla consolidado la fecha que acabamos de armar
                        string filterDate = TableQuery.GenerateFilterConditionForDate("Date", QueryComparisons.Equal, dateconsolidated);
                        //combinamos los dos filtros
                        string combinedFilter = TableQuery.CombineFilters(filterid, TableOperators.And, filterDate);

                        //creamos el query con el filtro combinado
                        TableQuery<ConsolidatedEntity> queryConsolidated = new TableQuery<ConsolidatedEntity>().Where(combinedFilter);
                        //ejecutamos la consulta en la tabla consolidado (buscamos el id donde estamos parados y la fecha que acabamos de armar)
                        TableQuerySegment<ConsolidatedEntity> filteredConsolidated = await consolidatedTable.ExecuteQuerySegmentedAsync(queryConsolidated, null);
                        //traemos la consulta anterior en una lista de consolidatedEntity
                        List<ConsolidatedEntity> filteredConsolidatedList = filteredConsolidated.ToList();

                        //si la lista de la consulta anterior esta vacia
                        if (filteredConsolidatedList.Count == 0)
                        {  //creamos el registro en la tabla
                            ConsolidatedEntity consolidatedEntity = new ConsolidatedEntity
                            {
                                EmployedId = orderedTimes[i].EmployedId,
                                Date = dateconsolidated,
                                MinutesWork = (int)worktime.TotalMinutes,
                                ETag = "*",
                                PartitionKey = "CONSOLIDATED",
                                RowKey = Guid.NewGuid().ToString(),
                            };
                            TableOperation addOperation = TableOperation.Insert(consolidatedEntity);
                            await consolidatedTable.ExecuteAsync(addOperation);
                            //aumentamos el contador de nuevos registros en la tabla consolidated
                            newc++;
                        }
                        else
                        {  //si la lista no esta vacia, es porque hay registos entonces la recorremos la consulta con un foreach para poder modificarlo
                            foreach (ConsolidatedEntity cons in filteredConsolidated)
                            {
                                cons.Date = dateconsolidated;
                                cons.MinutesWork += (int)worktime.TotalMinutes;
                                await consolidatedTable.ExecuteAsync(TableOperation.Replace(cons));
                            }
                            //aumentamos el contador de registros actualizados en la tabla consolidated
                            update++;
                        }

                    }
                    orderedTimes[i].IsConsolidated = true;
                    await timeTable.ExecuteAsync(TableOperation.Replace(orderedTimes[i]));

                    i++;


                    if (i + 1 == orderedTimes.Count)
                    {

                        if (orderedTimes[i].EmployedId == orderedTimes[i - 1].EmployedId)
                        {
                            orderedTimes[i].IsConsolidated = true;
                            await timeTable.ExecuteAsync(TableOperation.Replace(orderedTimes[i]));

                        }

                        i++;
                    }
                }

                
            }

            string message = $"Agregados {newc} y actualizados {update}" ;
            log.LogInformation(message);


            // RETORNAMOS UNA RESPUESTA POSITIVA CON QUE SI SE CUMPLIO LA EJECUCION, CON EL MENSAJE Y CON LA ENTIDAD CREADA
            return new OkObjectResult(new Response
            {
                IsSucess = true,
                Message = message,

            });


        }
    }
}
