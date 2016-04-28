using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Treeview.Repository;

namespace Treeview.Controllers
{
    public class PermissionController : Controller
    {
        private IPermissionRepository repository;

        public PermissionController()
           
        {
            this.repository = new PermissionRepository(new MvcAdvancedEntities());
               
           
        }

        public ActionResult Index()
        {

            return View();
        }

        public JsonResult GetAllEmpRole()
        {
            var employeeList = repository.GetAll();
            return Json(employeeList, JsonRequestBehavior.AllowGet);

        }
        
        public JsonResult GetAllEmpNames()
        {
            var employeeList = repository.GetName();
            return Json(employeeList, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetAllRoles()
        {
            var roleList = repository.GetRole();
            return Json(roleList, JsonRequestBehavior.AllowGet);

        }

        public string UpdateRole(Employee permission)
        {

            if (permission != null)
            {
                repository.UpdateRole(permission);
                repository.Save();

                return "Record has been Updated";

            }
            else
            {
                return "Record has Not been Updated";
            }
        }

    }
}
