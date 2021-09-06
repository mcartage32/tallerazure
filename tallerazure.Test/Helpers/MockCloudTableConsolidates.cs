using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using tallerazure.Functions.Entities;

namespace tallerazure.Test.Helpers
{
    public class MockCloudTableConsolidates: CloudTable
    {
        public MockCloudTableConsolidates(Uri tableAddress) : base(tableAddress)
        {
        }

        public MockCloudTableConsolidates(Uri tableAbsoluteUri, StorageCredentials credentials) : base(tableAbsoluteUri, credentials)
        {
        }

        public MockCloudTableConsolidates(StorageUri tableAddress, StorageCredentials credentials) : base(tableAddress, credentials)
        {
        }

        public override async Task<TableResult> ExecuteAsync(TableOperation operation)
        {
            return await Task.FromResult(new TableResult
            {
                HttpStatusCode = 200,
                Result = TestFactory.GetTimeEntity()

            });
        }


  public override async Task<TableQuerySegment<ConsolidatedEntity>> ExecuteQuerySegmentedAsync<ConsolidatedEntity>(TableQuery<ConsolidatedEntity> query,
            TableContinuationToken token)
        {
            ConstructorInfo constructor = typeof(TableQuerySegment<TimeEntity>)
                   .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
                   .FirstOrDefault(c => c.GetParameters().Count() == 1);

      return await Task.FromResult(constructor.Invoke(new object[] { TestFactory.GetTimesEntityList() }) as TableQuerySegment<ConsolidatedEntity>);
        }


    }
}
