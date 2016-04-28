using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Treeview.Models;
using Treeview.Repository;
using System.Net;
using System.Net.Mail;

namespace Treeview.Controllers
{
    public class ArticleController : Controller
    {
        //
        // GET: /Article/

        private IArticleRepository articleRepository;
        private ICommentRepository commentRepository; 

        public ArticleController()
        {
         this.articleRepository=new ArticleRepository(new CosmicVerseEntities1());
         this.commentRepository = new CommentRepository(new CosmicVerseEntities1());
       
        }
        public ActionResult Index(int id)
        {
            ViewModel obj = new ViewModel();
            var ctx = new CosmicVerseEntities1();
           
            //Article
            List<Article> querylist = new List<Article>();
            querylist = (from m in ctx.Articles
                         where m.ID == id
                         select m).ToList();
            foreach (var item in querylist)
            {
                obj.ArticleDateTime = item.DateTime.ToString();
                obj.ArticleTitle = item.Title;
                obj.Body = item.Body;
                obj.ShortDesc = item.ShortDesc;
                obj.Author = item.Author;
                obj.ArticleID = item.ID;
            }

         
            IEnumerable<Treeview.Comment> querylistC;
            querylistC = (from m in ctx.Comments
                          where m.ArticleID == id && m.Flag == true
                          select m);
            
            obj.allComments = querylistC;

            return View(obj);
        }

        public ActionResult CreateView()
        {
            return View();
        }
        public Article newArticle;

        [HttpPost]
        public ActionResult Create(ViewModel article)
        {
           // string strmy=Request.QueryString["Body"];
            using (var ctx = new CosmicVerseEntities1())
            {
              
                Article art = new Article();
               // art.ID = article.ArticleID;

                art.Title=article.ArticleTitle ;
                art.Body =article.Body ;
                art.ShortDesc = article.ShortDesc;
                art.DateTime = DateTime.Now.DayOfWeek.ToString()+" "+DateTime.Now.Month.ToString()+" "+DateTime.Now.Year.ToString()+" "+DateTime.Now.Hour.ToString()+":"+DateTime.Now.Minute.ToString();
                art.Author = "Mahsa Hassankashi";

                articleRepository.InsertArticle(art);
                articleRepository.Save();
                                             
            }
            return RedirectToAction("Index");
        }


        public ActionResult EditView(int id)
        {
            ViewModel obj = new ViewModel();
            var ctx = new CosmicVerseEntities1();

            //Article
            List<Article> querylist = new List<Article>();
            querylist = (from m in ctx.Articles
                         where m.ID == id
                         select m).ToList();
            foreach (var item in querylist)
            {
                obj.ArticleDateTime = item.DateTime.ToString();
                obj.ArticleTitle = item.Title;
                obj.Body = item.Body;
                obj.ShortDesc = item.ShortDesc;
                obj.Author = item.Author;
                obj.ArticleID = item.ID;
            }

            return View(obj);
        }
        
        [HttpPost]
        public ActionResult Edit(ViewModel article)
        {
            // string strmy=Request.QueryString["Body"];
            using (var ctx = new CosmicVerseEntities1())
            {

                Article art = new Article();
                art.ID = article.ArticleID;
                art.Title = article.ArticleTitle;
                art.Body = article.Body;
                art.ShortDesc = article.ShortDesc;
                art.DateTime = DateTime.Now.DayOfWeek.ToString() + " " + DateTime.Now.Month.ToString() + " " + DateTime.Now.Year.ToString() + " " + DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString();
                art.Author = "Mahsa Hassankashi";

                articleRepository.UpdateArticle(art);
                articleRepository.Save();

            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Comment(ViewModel vcomment)
        {
            // string strmy=Request.QueryString["Body"];
            using (var ctx = new CosmicVerseEntities1())
            {
                Comment com = new Comment();
                
                com.ArticleID = vcomment.ArticleID;
                com.Title = vcomment.ArticleTitle;
                com.BodyMsg = vcomment.BodyMsg;
                com.Name=vcomment.Name;
                com.Email=vcomment.Email;
                com.DateTime = DateTime.Now.DayOfWeek.ToString() + " " + DateTime.Now.Month.ToString() + " " + DateTime.Now.Year.ToString() + " " + DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString();
                com.Flag = false;
                //************START
                try
                {
                    MailMessage objeto_mail = new MailMessage();
                    SmtpClient client = new SmtpClient();
                    client.Port = 25;
                    client.Host = "smtp.internal.cosmicverse.info";
                    client.Timeout = 10000;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new System.Net.NetworkCredential("mail@cosmicverse.info", "75337533");
                    objeto_mail.From = new MailAddress("mail@cosmicverse.info");
                    objeto_mail.To.Add(new MailAddress(vcomment.Email));
                    objeto_mail.Bcc.Add(new MailAddress("mahsa.hassankashi@gmail.com"));
                    objeto_mail.Subject = "cosmic verse";
                    objeto_mail.Body = "Thank you dear for your comment. Your comment is in queue process";
                    client.Send(objeto_mail);
                }
                catch (Exception ex )
                {

                   
                }
               

                //***************END
                commentRepository.InsertComment(com);
                commentRepository.Save();

            }
            //Notification : your message is in confirm process
            return RedirectToAction("Index");
        }

        public ActionResult CommentList()
        {
            //string strID=TempData["articleID"].ToString();

            IEnumerable<Comment> obj = commentRepository.GetCommentByArticleID(1);
            var ctx = new CosmicVerseEntities1();
            
           
            ////Comment
            //List<Comment> querylistC = new List<Comment>();
            //querylistC = (from m in ctx.Comments
            //              where m.ArticleID == Convert.ToInt16(strID) && m.Flag == true
            //              select m).ToList();
            //foreach (var item in querylistC)
            //{
            //    obj.Name = item.Name;
            //    obj.Email = item.Email;
            //    obj.BodyMsg = item.BodyMsg;
            //    obj.commentDateTime = item.DateTime;


            //}
            return View(obj);
        }

    }
}
