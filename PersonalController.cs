using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Diagnostics;
using System.Text;

namespace Treeview.Controllers
{
    public class PersonalController : Controller
    {
        
        
        int mainNode = 0;
        int childquantity = 0;
        int myflag;

        [HttpPost]
        public ActionResult SaveNode(string childid, string parentid)
        {
            //*****REVISION******Do not allow reportsto = itself 
            if (childid != null && parentid != null && parentid != childid  )
            {
                string source = childid;
                string destination = parentid;
                using (var ctx = new CosmicVerseEntities1())
                {
                    var personals = ctx.Personals.Where(m => m.Name == source).First();

                   var personalsParent = ctx.Personals.Where(m => m.Name == destination).First();

                    //***check
                   //*****REVISION******Do not allow make circle in data base such as P1 is parent for P2 and also P2 is parent for P1 ==> Prevent these mistake by checking it
                    //
           
                    if (personalsParent.ReportsTo!=personals.ID)
                    {
                        personals.ReportsTo = Convert.ToInt16(personalsParent.ID);


                        ctx.SaveChanges();
                    }
                    //***check

                    
                }
                
            }
            return RedirectToAction("Index");
        }

        public string Treeview(int itemID, string mystr, int j, int flag)
        {
           
            List<Personal> querylist = new List<Personal>();
            var ctx = new CosmicVerseEntities1();
            
            if (flag==0)
            {
               
               querylist = (from m in ctx.Personals
                             where m.ReportsTo == null
                             select m).ToList();
               mainNode = querylist.Count;
              
                mystr += "[";
            }
            if (flag == 1)
            {

                querylist = (from m in ctx.Personals
                             where m.ReportsTo == itemID
                             select m).ToList();
                mainNode = querylist.Count;
                mystr += ",items:[";
            }
            
            //Below line shows an example of how to make parent node with his child
            //[{ id: "1", text: "P1", items: [{ id: "5", text: "P2" }] }]


           int i=1;
                foreach (var item in querylist)
                {
                            myflag = 0;
                            mystr += "{id:\"" + item.ID + "\",text:\"" + item.Name + "\"";
                            List<Personal> querylistParent = new List<Personal>();
                            //Check this parent has child or not , if yes how many?
                            querylistParent = (from m in ctx.Personals
                                           where m.ReportsTo == item.ID
                                           select m).ToList();
                            childquantity = querylistParent.Count;
                            //If Parent Has Child again call Treeview with new parameter
                            if (childquantity > 0)
                            {
                                mystr = Treeview(item.ID, mystr, i, 1);
                              
                            }
                            //No Child and No Last Node
                            else if (childquantity == 0 && i < querylist.Count)
                            {
                                mystr += "},";
                            }
                            //No Child and Last Node
                            else if (childquantity == 0 && i == querylist.Count)
                            {
                               
                               int fcheck2=0;
                               int fcheck3 = 0;
                               int counter = 0;
                               int flagbreak = 0;

                                int currentparent;
                                List<Personal> parentquery ;
                                List<Personal> childlistquery;
                                TempData["counter"] =0;
                                currentparent = Convert.ToInt16(item.ReportsTo);
                                int coun;
                                while (currentparent != 0)
                                {
                                    //count parent of parent
                                   
                                     fcheck2 = 0;
                                     fcheck3 = 0;
                                     parentquery = new List<Personal>();
                                     parentquery = (from m in ctx.Personals
                                                      where m.ID == currentparent
                                                      select m).ToList();
                                     var rep2 = (from h in parentquery select new { h.ReportsTo }).First();
                                   
                                    //put {[ up to end
                                    
                                    //list of child
                                     childlistquery = new List<Personal>();
                                     childlistquery = (from m in ctx.Personals
                                                      where m.ReportsTo == currentparent
                                                     select m).ToList();


                                     foreach (var item22 in childlistquery)
                                        {
                                         
                                           
                                            if (mystr.Contains(item22.ID.ToString()))
                                            {

                                                if (item22.ReportsTo == currentparent)
                                                {
                                                    fcheck3 += 1;
                                                    if (fcheck3 == 1)
                                                    {
                                                        counter += 1;
                                                    }
                                                }

                                            }
                                            else
                                            {
                                               
                                                myflag = 1;
                                                if (item22.ReportsTo == currentparent)
                                                {
                                                     fcheck2+=1;
                                                     if (fcheck2==1)
                                                     {
                                                         counter -= 1;
                                                         flagbreak = 1;
                                                         
                                                     }
                                                }
                                                                                                                                             
                                            }
                                        }

                                     var result55 = (from h in parentquery select new { h.ID }).First();
                                        coun = Convert.ToInt16(result55.ID);
                                        TempData["coun"] = Convert.ToInt16(coun);
                                        currentparent = Convert.ToInt16(rep2.ReportsTo);
                                        if (flagbreak == 1)
                                        {
                                            break;
                                        }
                                       
                                }
                              
                                for (int i2 = 0; i2 < counter; i2++)
                                {
                                    mystr += "}]";
                                }

                                List<Personal> lastchild = new List<Personal>();
                                lastchild = (from m in ctx.Personals
                                                where m.ReportsTo == item.ReportsTo
                                                select m).ToList();

                                List<Personal> lastparent = new List<Personal>();
                                lastparent = (from m in ctx.Personals
                                                where m.ReportsTo == null
                                                select m).ToList();

                                if (lastchild.Count > 0)
                                {
                                    var result_lastchild = (from h in lastchild select new { h.ID }).Last();
                                    var result_lastparent = (from h in lastparent select new { h.ID }).Last();
                                    int mycount = Convert.ToInt16(TempData["coun"]);
                                    if (item.ID == result_lastchild.ID && mycount == result_lastparent.ID && myflag == 0)
                                    {
                                       mystr += "}]";
                                    }
                                    else if (item.ID == result_lastchild.ID && mycount == result_lastparent.ID && myflag == 1)
                                    {
                                       mystr += "},";
                                    }
                                    else if (item.ID == result_lastchild.ID && mycount != result_lastparent.ID)
                                    {
                                       mystr += "},";
                                    }
                                    
                                }
                                //  finish }]
                                else if (lastchild.Count == 0 && item.ReportsTo == null)
                                {
                                   mystr += "}]";
                                }
                            }
                            i++;   
                    }
                    
            
            return mystr;
        }
       
        public ActionResult Index()
        {
           
            ViewData["treeviewds"] = Treeview(0, "", 0, 0);
            return View();
           
        }

        public ActionResult DynamicTreeview()
        {

            ViewData["treeviewds"] = Treeview(0, "", 0, 0);
            return View();

        }
      
       
         
    }
    
}


