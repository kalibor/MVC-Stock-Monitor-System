using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.ComponentModel.DataAnnotations;
using LinqToExcel;
using TestNotification.Service;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace TestNotification.Controllers
{
    public class HomeController : Controller
    {
        StockService service = new StockService();

        // GET: Home
        public ActionResult Index()
        {

            return View();
        }


        [HttpPost]
        public JsonResult GetAllStockCategory()
        {
            return Json(service.Func_GCompanySource().Select(x => x.CIndustry).Distinct());
        }

        [HttpPost]
        public JsonResult AllStock(string CIndustry)
        {

            try
            {
                var table = service.Func_GStockInfoTable(CIndustry);
                return Json(table);
            }
            catch (Exception e)
            {
                return null;
            }


        }




        [HttpPost]
        public JsonResult TestStock(string CIndustry)
        {
         
            try
            {
                var table = service.Func_GStockInfoTable(CIndustry);
                return Json(table.TableData);
            }
            catch (Exception e)
            {
                return null;
            }


        }



    }
}