using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using System.Threading;
using RKLib.ExportData;
using HtmlAgilityPack;
using System.Data;
using System.Net;
namespace Treeview.Controllers
{
    public class TestController : Controller
    {
        //
        // GET: /Test/

        public ActionResult FillQuery()
        {
            return View();
        }
         [HttpPost]
        public ActionResult ExportExcel()
        {
            string b=Request["me"].ToString();
            RKLib.ExportData.Export obj = new RKLib.ExportData.Export("Web");


            System.Net.WebClient client = new System.Net.WebClient();

            //client.Encoding = System.Text.Encoding.UTF8;
            var doc = new HtmlAgilityPack.HtmlDocument();

            //string str = this.TextBox1.Text;
            //string b = HiddenField1.Value;
            doc.LoadHtml(b);

            //doc.LoadHtml(b);

            var nodesT = doc.DocumentNode.SelectNodes("//table/tr");

            var table = new System.Data.DataTable("MyTable");

            var headers = nodesT[0]
                .Elements("th")
                .Select(th => th.InnerText.Trim());
            foreach (var header in headers)
            {
                table.Columns.Add(header);
            }

            var rows = nodesT.Skip(1).Select(tr => tr
               .Elements("td")
               .Select(td => td.InnerText.Trim())
               .ToArray());

            foreach (var row in rows)
            {
                table.Rows.Add(row);
            }
            Response.ContentEncoding = System.Text.Encoding.Unicode;
            Response.BinaryWrite(System.Text.Encoding.Unicode.GetPreamble());


            obj.ExportDetails(table, RKLib.ExportData.Export.ExportFormat.Excel, "Mahsa");
            return RedirectToAction("View");
        }

        // @using (Html.BeginForm("Excel", "Test", FormMethod.Post, new { ID = "MyID", onsubmit = "return sayHello()" }))
   
        [HttpPost]
        public ActionResult Excel(string strvar)
        {
            string str = strvar;

         //   string str2 = odiv;

            Response.Clear();
            Response.AddHeader("content-disposition", "attachment;filename=Test.xls");
            Response.ContentType = "application/vnd.ms-excel";
            //Response.ContentType = "application/vnd.ms-excel";
            Response.ContentEncoding = System.Text.Encoding.Unicode;
            Response.BinaryWrite(System.Text.Encoding.Unicode.GetPreamble());
            Response.Write(str.ToString());
            return View();
        }

    }
}
