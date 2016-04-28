using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Treeview.Models;
using System.Data.Entity;
using System.Data;

namespace Treeview.Controllers
{
    public class AccountController : Controller
    {
        //
        // GET: /Authentication/
        public ActionResult Index()
        {
            return View();
        }
        [Authorize]
        public ActionResult UserProfileView()
        {
            string username = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            string roles = string.Empty;
            int UserID;

            using (EHealthEntities entities = new EHealthEntities())
            {
                User user = entities.Users.SingleOrDefault(u => u.UserName == username);

                roles = user.Role;
                UserID = user.ID;

            }
            if (roles == "patient")
            {
                EHR obj = new EHR();
                var ctx = new EHealthEntities();

                //Article
                List<EHR> querylist = new List<EHR>();
                querylist = (from m in ctx.EHRs
                             where m.UserID == UserID
                             select m).ToList();
                foreach (var item in querylist)
                {
                    obj.EHRID = item.EHRID;
                    obj.UserID = item.UserID;
                    obj.EHRDescription = item.EHRDescription;
                    obj.LastUpdated = item.LastUpdated;
                }

                return View(obj);
            }
            else if (roles != "patient")
            {
                return RedirectToAction("PatientList");
                //redirect to patientview

            }

            return View();

        }
        [Authorize]
        public ActionResult PatientList()
        {
            string username = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            string roles = string.Empty;
            int UserID;

            ViewModel obj = new ViewModel();

            using (EHealthEntities entities = new EHealthEntities())
            {
                User user = entities.Users.SingleOrDefault(u => u.UserName == username);

                roles = user.Role;
                UserID = user.ID;

                IEnumerable<Treeview.PermissionView> querylistC;
                querylistC = (from m in entities.PermissionViews
                              where m.SupervisorID == UserID
                              select m).ToList();

                IEnumerable<Treeview.EHR> querylistC2 = new List<Treeview.EHR>();
                foreach (var item in querylistC)
                {
                    querylistC2 = (from m in entities.EHRs
                                   where m.UserID == item.PatientID
                                   select m).ToList();

                }
                var mahsa = querylistC2;


                obj.allEHR = querylistC2;
                ViewBag.PatientListName = username;
                // Read and Write
                if (roles == "admin" || roles == "Doctor" || obj.ReadEHR == true || obj.WriteEHR == true)
                {
                    return RedirectToAction("ReadWritePermission");

                }

                //Just Read
                if (roles == "Nurse" || roles == "Police" || obj.ReadEHR == true || obj.WriteEHR == false)
                {
                    return View(querylistC2);
                }
                if (roles == "Accountant" || obj.ReadEHR == false || obj.WriteEHR == false)
                {
                    return RedirectToAction("NoPermission");
                }
                return View();
            }


        }
        [Authorize]
        public ActionResult ReadWritePermission()
        {
            string username = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            string roles = string.Empty;
            int UserID;

            ViewModel obj = new ViewModel();
            List<EHR> EHRlist = new List<EHR>();

            using (EHealthEntities entities = new EHealthEntities())
            {
                User user = entities.Users.SingleOrDefault(u => u.UserName == username);

                roles = user.Role;
                UserID = user.ID;

                IEnumerable<Treeview.PermissionView> querylistC;
                querylistC = (from m in entities.PermissionViews
                              where m.SupervisorID == UserID
                              select m).ToList();

                IEnumerable<Treeview.EHR> querylistC2 = new List<Treeview.EHR>();
                List<Treeview.EHR> querylistC3 = new List<Treeview.EHR>();
                foreach (var item in querylistC)
                {
                    querylistC2 = (from m in entities.EHRs
                                   where m.UserID == item.PatientID
                                   select m).ToList();

                    foreach (var item2 in querylistC2)
                    {
                        querylistC3.Add(item2);
                    }

                }
                var mahsa = querylistC2;


                ViewBag.AdminRole = roles;

                ViewBag.PatientListName = username;
                return View(querylistC3);
            }
        }

        [HttpPost]
        public ActionResult SUPUserProfileView(int ehrid, int ehruserid, string ehrdesc)
        {
            int ehridup = Convert.ToInt32(ehrid);
            int ehruseridup = Convert.ToInt32(ehruserid);
            string ehrdescup = ehrdesc;

            using (var ctx = new EHealthEntities())
            {

                EHR ehr = new EHR();
                ehr.EHRID = ehridup;
                ehr.UserID = ehruseridup;

                ehr.EHRDescription = ehrdescup;

                ctx.Entry(ehr).State = EntityState.Modified;
                ctx.SaveChanges();

            }
            return RedirectToAction("Index");
        }

        public ActionResult NoPermission()
        {

            return View();
        }
        [HttpPost]
        public ActionResult UserProfileView(EHR ehr)
        {
            // string strmy=Request.QueryString["Body"];
            using (var ctx = new EHealthEntities())
            {

                EHR objehr = new EHR();
                objehr.UserID = ehr.UserID;
                objehr.EHRID = ehr.EHRID;
                objehr.EHRDescription = ehr.EHRDescription;
                objehr.UserName = ehr.UserName;
                objehr.LastUpdated = ehr.LastUpdated;

                ctx.Entry(ehr).State = EntityState.Modified;

                ctx.SaveChanges();

            }
            return RedirectToAction("Index");
        }

        public ActionResult Login()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(User model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                using (EHealthEntities entities = new EHealthEntities())
                {
                    string username = model.UserName;
                    string password = model.Password;


                    bool userValid = entities.Users.Any(user => user.UserName == username && user.Password == password);

                    if (userValid)
                    {
                        //Create Cookie
                        FormsAuthentication.SetAuthCookie(username, false);
                        if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                            && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                        {
                            return Redirect(returnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "The user name or password provided is incorrect.");
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("LogOff", "Account");
        }


        [HttpPost]
        public ActionResult CreateUser()
        {
            return View();
        }

        // [Authorize(Roles = "admin")]
        public ViewResult CreatePermission()
        {

            EHealthEntities ctx = new EHealthEntities();

            // ViewBag.Names = ctx.Countries.ToList();

            var model = new ViewModel();
            ViewBag.Patient = ctx.Users.Where(x => x.Role == "patient").ToList();
            // ViewBag.Supervisor = ctx.Users.ToList().Where((x => x.Role == "patient") AND (x => x.));
            model.UserList = GetArchiveCategories();

            return View(model);

        }

        [HttpPost]
        public ActionResult AddPermission(int sid, int pid, bool ReadEHR, bool WriteEHR, bool DeleteEHR)
        {

            EHealthEntities ctx = new EHealthEntities();
            Permission per = new Permission();

            per.SupervisorID = sid;
            per.PatientID = pid;
            per.ReadEHR = ReadEHR;
            per.WriteEHR = WriteEHR;
            per.DeleteEHR = DeleteEHR;

            ctx.Permissions.Add(per);
            ctx.SaveChanges();
            return View();
        }

        [HttpPost]
        public ActionResult DeletePermission(int id)
        {

            EHealthEntities ctx = new EHealthEntities();
            Permission per = ctx.Permissions.Find(id);
            ctx.Permissions.Remove(per);

            ctx.SaveChanges();
            return View();
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult PatientView(int id)
        {
            var model = new ViewModel();
            EHealthEntities ctx = new EHealthEntities();

            //var categories = ctx.Permissions.ToList().Where(x => x.SupervisorID == id);
            // var list = new List<PermissionModel>();
            var childList = ctx.PermissionViews.ToList().Where(x => x.SupervisorID == id);

            var childGroup = childList.Select(m => new SelectListItem()
            {
                Text = m.PatientID.ToString() + "," + m.UserName.ToString() + "," + m.DeleteEHR.ToString() + "," + m.ReadEHR.ToString() + "," + m.WriteEHR.ToString(),
                Value = Convert.ToString(m.ID),
            });

            return Json(childGroup, JsonRequestBehavior.AllowGet);

        }

        public IEnumerable<SelectListItem> GetArchiveCategories()
        {
            EHealthEntities ctx = new EHealthEntities();
            var categories = ctx.Users.ToList().Where(x => x.Role != "patient");

            var list = new List<SelectListItem>();

            //you are filling your list with {Text=something Name, Value=something ID}
            foreach (var cats in categories)
            {

                list.Add(new SelectListItem
                {
                    Text = cats.UserName + "-" + cats.Role,
                    Value = cats.ID.ToString()
                });

            }
            return list;

        }



        public ActionResult ChangePermission1(int supid, string stext)
        {


            Session["supid2"] = supid;
            Session["stext2"] = stext;
            return RedirectToAction("ChangePermission", "Account");


        }

        public ActionResult ChangePermission()
        {
            int supid22 = Int32.Parse(Session["supid2"].ToString());
            string stext22 = Session["stext2"].ToString();

            EHealthEntities ctx = new EHealthEntities();
            ViewModel obj = new ViewModel();


            IEnumerable<Treeview.PermissionView> querylistC;
            querylistC = (from m in ctx.PermissionViews
                          where m.SupervisorID == supid22
                          select m);

            obj.allPatient = querylistC;


            ViewBag.Name = stext22;


            return View(obj);

        }



    }
}
