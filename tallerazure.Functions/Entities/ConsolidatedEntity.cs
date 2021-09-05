﻿using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace tallerazure.Functions.Entities
{
    public class  ConsolidatedEntity : TableEntity
    {
        public int EmployedId { get; set; }

        public DateTime Date { get; set; }

        public int MinutesWork { get; set; }
    }
}
