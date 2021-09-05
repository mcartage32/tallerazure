using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace tallerazure.Functions.Entities
{
    public class TimeEntity : TableEntity
    {
        public int EmployedId { get; set; }

        public DateTime Date { get; set; }

        public int Type { get; set; }

        public bool IsConsolidated { get; set; }
    }
}
