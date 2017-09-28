using LinqToExcel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using TestNotification.Models;

namespace TestNotification.Service
{
    public class StockService
    {
        #region 建構函式
        public StockService()
        {
            ComapnyEntityPath = HttpContext.Current.Server.MapPath(string.Format("/Stock/Company/{0}.xlsx", DateTime.Now.ToString("yyyyMMdd")));
            excel = new ExcelQueryFactory(ComapnyEntityPath);
        }
        #endregion

        #region 公有方法
        /// <summary>
        /// 取得要顯示在網頁Table的料
        /// </summary>
        /// <param name="CIndustry">產業別</param>
        /// <returns></returns>
        public StockInfoTable Func_GStockInfoTable(string CIndustry)
        {

            var CompanySource = Func_GCompanySource().Where(x => x.CIndustry.Equals(CIndustry));
            var StockList = APIFunc_GrealTimeStock(CompanySource.Select(x => x.CID).ToList());

            var TableData = from cs in CompanySource.ToList()
                            join s in StockList on cs.CID equals s.StockCode
                            select new
                            {
                                CID = cs.CID,
                                CName = cs.CName,
                                Bid = s.Bid,
                                Ask = s.Ask,
                                Change = s.Change,
                                Volume= s.Volume

                            } as object;

            return new StockInfoTable()
            {
                Date = DateTime.Now,
          ///*  TableColumn = new List<string>() { "公司代號", "公司名稱", "買進", "賣出", "漲跌" }*/,
                TableData = TableData.ToList()
            };

        }

        /// <summary>
        /// 透過政府公開資訊API取得所有公司的資料
        /// </summary>
        /// <returns></returns>
        public IQueryable<StockCompany> Func_GCompanySource()
        {
            if (!System.IO.File.Exists(ComapnyEntityPath))
            {
                APIFunc_ExportNewCompanySource();
                return Func_GCompanySource();
            }
            else
            {

                foreach (var property in typeof(StockCompany).GetProperties())
                {
                    foreach (var item in property.CustomAttributes)
                    {
                        if (item.AttributeType.Equals(typeof(DisplayAttribute)))
                        {
                            var displayName = (string)item.NamedArguments.Where(x => x.MemberName.Equals("Name")).FirstOrDefault().TypedValue.Value;
                            excel.AddMapping(property.Name, displayName);
                        }

                    }
                }

                var CompanyRows = excel.Worksheet<StockCompany>(0).Select(x => x);

                return CompanyRows;
            }
        }
        
        #endregion

        #region API方法
        /// <summary>
        /// 透過yahoo finance API取得即時股票資訊
        /// </summary>
        /// <param name="StockCodeList">股票代碼列表</param>
        /// <returns></returns>
        private List<StockInfo> APIFunc_GrealTimeStock(List<string> StockCodeList)
        {
            StringBuilder sb = new StringBuilder();

            var CodesList = StockCodeList.Select(x => { return string.Format("{0}.TW", x); });
            var strCode = string.Join("+", CodesList);

            /*yahoo finance API f参数对照表 http://www.nginx.cn/1414.html*/
            var url = string.Format("http://finance.yahoo.com/d/quotes.csv?s={0}&f={1}", strCode, "sbac1v");
            List<StockInfo> StockList = new List<StockInfo>();

            HttpClient client = new HttpClient();
            using (var response = client.GetAsync(url).Result)
            {
                using (StreamReader reader = new StreamReader(response.Content.ReadAsStreamAsync().Result))
                {


                    while (!reader.EndOfStream)
                    {
                        string result = reader.ReadLine();
                        string[] results = result.Split(',');
                        if (results.Length == 5)
                        {
                            StockInfo stock = new StockInfo()
                            {
                                StockCode = results[0].Replace(".TW", "").Replace("\"", ""),
                                Bid = results[1].Replace("\"", ""),
                                Ask = results[2].Replace("\"", ""),
                                Change = results[3].Replace("\"", ""),
                                Volume = string.Format("{0:0,0}", int.Parse(results[4].Replace("\"", "")) /1000),
                            };
                            StockList.Add(stock);
                        }
                    }
                }
            }
            return StockList;
        }




        /// <summary>
        /// 匯出新的公司資源到excel
        /// </summary>
        private void APIFunc_ExportNewCompanySource()
        {
            var url = "http://dts.twse.com.tw/opendata/t187ap03_L.csv";
            HttpClient client = new HttpClient();

            using (var response = client.GetAsync(url).Result)
            {
                using (StreamReader reader = new StreamReader(response.Content.ReadAsStreamAsync().Result, Encoding.Default))
                {
                    string Title = reader.ReadLine();

                    List<string> columns = reader.ReadLine().Split(',').Select(x => x.Trim().Replace("\"", "")).ToList();
                    List<List<object>> DataList = new List<List<object>>();
                    while (!reader.EndOfStream)
                    {
                        var dataRow = reader.ReadLine().Split(new string[] { "\"," }, StringSplitOptions.None).Select(y => (object)(y.Replace("\"", "")));
                        DataList.Add(dataRow.ToList());
                    }
                    DataList.RemoveAt(DataList.Count() - 1);

                    ExportToExcel(columns, DataList, ComapnyEntityPath);

                }
            }

        }

        #endregion

        #region 私有方法
        /// <summary>
        /// 會出資料至exceL
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="DataList"></param>
        /// <param name="SavePath"></param>
        private void ExportToExcel(List<string> columns, List<List<object>> DataList, string SavePath)
        {
            IWorkbook wb = new XSSFWorkbook();
            ISheet ws = wb.CreateSheet(DateTime.Now.AddDays(1).ToString("yyyyMMdd"));
            ws.CreateRow(0);
            for (int i = 0; i < columns.Count(); i++)
            {
                ws.GetRow(0).CreateCell(i).SetCellValue(columns[i]);
            }

            for (int i = 0; i < DataList.Count(); i++)
            {
                ws.CreateRow(i + 1);
                for (int j = 0; j < DataList[i].Count(); j++)
                {
                    ws.GetRow(i + 1).CreateCell(j).SetCellValue(string.Format("{0}", DataList[i][j]));
                }
            }

            using (var filestream = new FileStream(SavePath, FileMode.Create))
            {
                wb.Write(filestream);
            }

        }

        #endregion

        #region 屬性成員
        string ComapnyEntityPath { get; set; }
        ExcelQueryFactory excel { get; set; }
        #endregion

    }





}