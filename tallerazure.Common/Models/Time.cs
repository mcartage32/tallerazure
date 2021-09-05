using System;
using System.Collections.Generic;
using System.Text;

namespace tallerazure.Common.Models
{
    public class Time
    {
        public int? EmployedId { get; set; }

        public DateTime? Date  { get; set; }

        public int? Type { get; set; }

        public bool IsConsolidated { get; set; }
    }
}
