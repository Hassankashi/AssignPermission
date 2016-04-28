using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Treeview.Repository;


namespace Treeview.Controllers
{
    public class EmployeeController : Controller
    {
         private IEmployeeRepository repository;
           
        public EmployeeController()
           
        {
            this.repository = new EmployeeRepository(new MvcAdvancedEntities());
               
           
        }
           
        
        public ActionResult Index()
        {
            return View();
                       
        }

        public JsonResult GetAllEmployees()
        {
                var employeeList = (List<Employee>)repository.All();
                return Json(employeeList, JsonRequestBehavior.AllowGet);
            
        }

        public JsonResult GetEmployeeById(int id)
        {
                int Id = Convert.ToInt32(id);
                var employeeListById = repository.GetByID(Id);

                return Json(employeeListById, JsonRequestBehavior.AllowGet);
           
        }

        public string Update(Employee employee)
        {
            
            if (employee != null)
            {
               repository.Update(employee);
               repository.Save();                          
               
               return "Record has been Updated";
               
            }
            else
            {
                return "Record has Not been Updated";
            }
        }

        public string Delete(int id)
        {

            try
            {
                //repository.Delete(Convert.ToInt32(employee.ID));
                repository.Delete(id);
                repository.Save(); 
                return "Employee Has Been Deleted";
            }
            catch
            {
                return "Employee Hasnot Been Deleted";
            }
        }

        public string Add(Employee employee)
        {
            try
            {
                if (employee != null)
                {
                    repository.Insert(employee);
                    repository.Save();

                    return "Record has been Added";

                }
                else
                {
                    return "Record has Not been Verified";
                }
                return "Record has been Added";
            }

            catch
            {
                return "Record has Not been Added";
            }
        }

       
    }
}
