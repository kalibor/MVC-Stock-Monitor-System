using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TestNotification.Models
{
    public class StockCompany
    {
        [Display(Name = "公司代號")]
        public string CID { get; set; }
        [Display(Name = "公司名稱")]
        public string CName { get; set; }
        [Display(Name = "產業別")]
        public string CIndustry { get; set; }
        [Display(Name = "住址")]
        public string CAddress { get; set; }
        [Display(Name = "營利事業統一編號")]
        public string UniformNumber { get; set; }
        [Display(Name = "董事長")]
        public string Chairman { get; set; }
        [Display(Name = "總經理")]
        public string GeneralManager { get; set; }
        [Display(Name = "發言人")]
        public string Spokesman { get; set; }
        [Display(Name = "發言人職稱")]
        public string SpokesmanTitle { get; set; }
        [Display(Name = "代理發言人")]
        public string ProxySpokesman { get; set; }
        [Display(Name = "總機電話")]
        public string Tel { get; set; }
        [Display(Name = "成立日期")]
        public string BuildDay { get; set; }
        [Display(Name = "上市日期")]
        public string ListedDay { get; set; }
        [Display(Name = "普通股每股面額")]
        public string NormalStockValue { get; set; }
        [Display(Name = "實收資本額")]
        public string Paid_upCapital { get; set; }
        [Display(Name = "私募股數")]
        public string PrivateStock { get; set; }
        [Display(Name = "特別股")]
        public string SpecialStock { get; set; }
        [Display(Name = "編制財務報表類型")]
        public string FinancialStatementCategory { get; set; }
        [Display(Name = "股票過戶機構")]
        public string TransferAgency { get; set; }
        [Display(Name = "過戶電話")]
        public string TransferAgencyTel { get; set; }
        [Display(Name = "過戶地址")]
        public string TransferAgencyAddress { get; set; }
        [Display(Name = "英文簡稱")]
        public string EnglishNickName { get; set; }
        [Display(Name = "英文通訊地址")]
        public string EnglishAddress { get; set; }
        [Display(Name = "傳真機號碼")]
        public string FaxMachine { get; set; }
        [Display(Name = "電子郵件信箱")]
        public string Email { get; set; }
        [Display(Name = "網址")]
        public string Website { get; set; }


    }
}