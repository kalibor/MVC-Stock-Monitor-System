using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TestNotification.Models
{
    public class StockInfo
    {
        [Display(Name = "股票代號")]
        public string StockCode { get; set; }
        [Display(Name = "賣出價")]
        public string Ask { get; set; }
        [Display(Name = "買入價")]
        public string Bid { get; set; }
        [Display(Name = "漲跌")]
        public string Change { get; set; }
    }

}