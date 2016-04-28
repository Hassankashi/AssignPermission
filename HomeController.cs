using MvcSiteMapProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using Treeview.Models;
using Treeview.Repository;

namespace Treeview.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
         private IContentRepository contentRepository;

         public HomeController()
        {
            this.contentRepository = new ContentRepository(new CosmicVerseEntities1());
        }

         public ActionResult DDLMVC()
         {
             return View();
         }
        
        [HttpPost]
        public ActionResult LinkHit(string link)
         {

             //this line is to check the clien ip address from the server itself
             string IP = "";

             string strHostName = "";
             strHostName = System.Net.Dns.GetHostName();

             IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(strHostName);

             IPAddress[] addr = ipEntry.AddressList;

             IP = addr[2].ToString();

             //Initializing a new xml document object to begin reading the xml file returned
             XmlDocument doc = new XmlDocument();
             doc.Load("http://www.freegeoip.net/xml");
             XmlNodeList nodeLst_IP = doc.GetElementsByTagName("Ip");
             XmlNodeList nodeLst_CountryCode = doc.GetElementsByTagName("CountryCode");
             XmlNodeList nodeLst_CountryName = doc.GetElementsByTagName("CountryName");
             XmlNodeList nodeLst_RegionCode = doc.GetElementsByTagName("RegionCode");
             XmlNodeList nodeLst_RegionName = doc.GetElementsByTagName("RegionName");
             XmlNodeList nodeLst_City = doc.GetElementsByTagName("City");
             XmlNodeList nodeLst_ZipCode = doc.GetElementsByTagName("ZipCode");
             XmlNodeList nodeLst_Latitude = doc.GetElementsByTagName("Latitude");
             XmlNodeList nodeLst_Longitude = doc.GetElementsByTagName("Longitude");
             XmlNodeList nodeLst_MetroCode = doc.GetElementsByTagName("MetroCode");
             XmlNodeList nodeLst_AreaCode = doc.GetElementsByTagName("AreaCode");
             //IP = "" + nodeLst_IP[0].InnerText + "<br>" + nodeLst_CountryCode[0].InnerText + "<br>" +
             //   nodeLst_CountryName[0].InnerText;
            
             //Response.Write(IP);
             //this is my header that I love

             if (link != null)
             {
                 string source = link;
                 //string destination = parentid;
                 using (var ctx = new CosmicVerseEntities1())
                 {
                     LinkHit lnkhit = new LinkHit();

                     lnkhit.Link = link;
                     lnkhit.Date = DateTime.Now.Date.Date.ToString();
                     lnkhit.Time = DateTime.Now.TimeOfDay.ToString();
                     lnkhit.countrycode = nodeLst_CountryCode[0].InnerText;
                     lnkhit.countryname = nodeLst_CountryName[0].InnerText;
                     lnkhit.city = nodeLst_City[0].InnerText;
                     lnkhit.IP = nodeLst_IP[0].InnerText;
                     lnkhit.regioncode = nodeLst_RegionCode[0].InnerText;
                     lnkhit.regionname = nodeLst_RegionName[0].InnerText;
                     lnkhit.areacode = nodeLst_AreaCode[0].InnerText;
                     lnkhit.longitude = nodeLst_Longitude[0].InnerText;
                     lnkhit.latitude = nodeLst_Latitude[0].InnerText;
                     lnkhit.zipcode = nodeLst_ZipCode[0].InnerText;


                     ctx.LinkHits.Add(lnkhit);
                     ctx.SaveChanges();

                 }

             }
            return RedirectToAction(link);
         }
        

        public ActionResult Index()
        {
            //ViewModel obj = new ViewModel();

            //List<Article> querylist = new List<Article>();
            //var ctx = new CosmicVerseEntities();
            //querylist = (from m in ctx.Articles
            //             where m.ID == 1
            //             select m).ToList();
            //foreach (var item in querylist)
            //{
            //    obj.ArticleDateTime = item.DateTime.ToString();
            //    obj.ArticleTitle = item.Title;
            //    obj.Body = item.Body;
            //    obj.ShortDesc = item.ShortDesc;
            //    obj.Author = item.Author;

            //}
            //return View(obj);
            return View();
        }
        public ActionResult AboutME()
        {
            Content cont = new Content();
            Treeview.Models.Content contM = new Treeview.Models.Content();

            using (var ctx = new CosmicVerseEntities1())
            {
                
               cont= contentRepository.GetContentByID(1);
               contM.ContentTitle = cont.Title;
               contM.ContentBody = cont.Body;

            }
            return View(contM);
        }
         [HttpPost]
        public ActionResult SendMail(Treeview.Models.Content cont)
        {
                  
            MailMessage objeto_mail = new MailMessage();
            SmtpClient client = new SmtpClient();
            client.Port = 25;
            client.Host = "smtp.internal.cosmicverse.info";
            client.Timeout = 10000;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential("mail@cosmicverse.info", "75337533");
            objeto_mail.From = new MailAddress(cont.Email.ToString());
            objeto_mail.To.Add(new MailAddress("mail@cosmicverse.info"));
            objeto_mail.Subject = "Cosmic Verse";
            objeto_mail.Body = cont.Name+":"+cont.Message;
            client.Send(objeto_mail);
                return RedirectToAction("Index");
        }

         
        //**************
      //  [MvcSiteMapNode(Title = "ContactME", ParentKey = "Index")]
        public ActionResult ContactME()
        {
            Content cont = new Content();
            Treeview.Models.Content contM = new Treeview.Models.Content();

            using (var ctx = new CosmicVerseEntities1())
            {

                cont = contentRepository.GetContentByID(2);
                contM.ContentTitle = cont.Title;
                contM.ContentBody = cont.Body;

            }
            return View(contM);
        }
      
        public ActionResult Articles()
        {
            Content cont = new Content();
            Treeview.Models.Content contM = new Treeview.Models.Content();

            using (var ctx = new CosmicVerseEntities1())
            {

                cont = contentRepository.GetContentByID(3);
                contM.ContentTitle = cont.Title;
                contM.ContentBody = cont.Body;

            }
            return View(contM);
        }
      
        public ActionResult Links()
        {
            Content cont = new Content();
            Treeview.Models.Content contM = new Treeview.Models.Content();

            using (var ctx = new CosmicVerseEntities1())
            {

                cont = contentRepository.GetContentByID(4);
                contM.ContentTitle = cont.Title;
                contM.ContentBody = cont.Body;

            }
            return View(contM);
        }

        //*********
        public ActionResult CreateContent()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateContent(Treeview.Models.Content content)
        {
            using (var ctx = new CosmicVerseEntities1())
            {

                Content cont = new Content();
                cont.Title = content.ContentTitle;
                cont.Body = content.ContentBody;

                contentRepository.InsertContent(cont);
                contentRepository.Save();

            }
            return RedirectToAction("Index");
        }
        //**************
        //***********

        public ActionResult EditView(int id)
        {
            Content obj = new Content();
            var ctx = new CosmicVerseEntities1();

            //Article
            List<Content> querylist = new List<Content>();
            querylist = (from m in ctx.Contents
                         where m.ID == id
                         select m).ToList();
            foreach (var item in querylist)
            {
                obj.ID = item.ID;
                obj.Title = item.Title;
                obj.Body = item.Body;
            }

            return View(obj);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(Content content)
        {
            // string strmy=Request.QueryString["Body"];
            using (var ctx = new CosmicVerseEntities1())
            {

                Content cont = new Content();
                cont.ID = content.ID;
                cont.Title = content.Title;
                cont.Body = content.Body;

               

                contentRepository.UpdateContent(cont);
                contentRepository.Save();

            }
            return RedirectToAction("Index");
        }

        
    }
}
