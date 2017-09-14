using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestNotification.Models
{
    public class StockInfoTable
    {
        public DateTime Date { get; set; }
        public List<string> TableColumn { get; set; }
        public List<object> TableData { get; set; }

    }
}