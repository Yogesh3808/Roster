
using Roster.Filter;
using Roster.Models;
using Roster.RosterDLL;
using RosterSystem.Repository;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Roster.Controllers
{
    [ValidateUserSession]
    [ActionFilter]
    public class EmployeeController : Controller
    {
        // GET: Employee
       
        private readonly RosterEntities _entities = new RosterEntities();
        public GenericUnitOfWork UnitOfWork = new GenericUnitOfWork();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult EmployeeDetails()
        {
            EmployeeViewModel objEmployeeViewModel = new EmployeeViewModel();
            objEmployeeViewModel.ObjEmployeeList = _entities.SP_EmployeeList().ToList();
            Session["CheckRoster"] = "0";
            return View("EmployeeDetails", objEmployeeViewModel);
        
        }
        public ActionResult Check_EmployeeDetails()
        {
            EmployeeViewModel objEmployeeViewModel = new EmployeeViewModel();
            objEmployeeViewModel.ObjEmployeeList = _entities.SP_EmployeeList().ToList();
            Session["CheckRoster"] = "1";
            return View("EmployeeDetails", objEmployeeViewModel);

        }
        public ActionResult AddNewEmployee()
        {
            EmployeeViewModel objEmployeeViewModel = new EmployeeViewModel();
            objEmployeeViewModel.ObjEmployeeList = _entities.SP_EmployeeList().ToList();
            return View("EmployeeDetails", objEmployeeViewModel);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EmployeeDetails(EmployeeViewModel model)
        {
            try
            {
            
                var EmailIDExist = UnitOfWork.GetRepositoryInstance<employee>().GetAllRecordsIQueryable().Where(i => i.email == model.Email.Trim()) .Count();
                if (model.EmployeeID == 0)
                {
                    if (EmailIDExist != 0)
                        ModelState.AddModelError("Email", "Email ID is already used");
                }
                else
                {
                    var EmpEmailIDExist = UnitOfWork.GetRepositoryInstance<employee>().GetAllRecordsIQueryable().Where(i => i.email == model.Email.Trim()).ToList();
                    if (EmpEmailIDExist != null)
                    {
                           if (EmpEmailIDExist[0].employeeId != model.EmployeeID)
                            ModelState.AddModelError("Email", "Email ID is already used");
                    }
                }
                if (ModelState.IsValid)
                {
                    var dt = UnitOfWork.GetRepositoryInstance<employee>().GetFirstOrDefault(model.EmployeeID);
                    dt = dt ?? new employee();
                    dt.firstName = model.FirstName.Trim();
                    dt.lastName = model.LastName.Trim();
                    dt.Phone = model.PhoneNumber.Trim();
                    dt.email = model.Email.Trim();
                    dt.employeename = model.FirstName.Trim() + " " + model.LastName.Trim();
                    if (dt.employeeId == 0)
                    {
                        
                        dt.createdDate = DateTime.Now;
                        UnitOfWork.GetRepositoryInstance<employee>().Add(dt);
                        TempData["msg3"] = "<script language='javascript' type='text/javascript'>alert('Employee Added Successfully!');</script>";
                    }
                    else
                    {
                        UnitOfWork.GetRepositoryInstance<employee>().Update(dt);
                        UnitOfWork.SaveChanges();
                        TempData["msg3"] = "<script language='javascript' type='text/javascript'>alert('Changes Updated Successfully!');</script>";
                    }
                    if (Convert.ToInt32(Session["CheckRoster"]) == 1)
                        return RedirectToAction("RosterDetails", "Roster");
                }
                model.ObjEmployeeList = _entities.SP_EmployeeList().ToList();
                 return View("EmployeeDetails", model); ;
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        [HttpGet]
        public ActionResult GetEmployeeById(int empId)
        {
            try
            {
                var dt = UnitOfWork.GetRepositoryInstance<employee>().GetFirstOrDefault(empId);
                //var empRate = _entities.EmployeeRates.Where(x => x.EmployeeId == empId)
                //    .OrderByDescending(x => x.EmployeeRateId).FirstOrDefault();
                var employeeViewModel = new EmployeeViewModel();
                if (dt != null)
                {
                    employeeViewModel.EmployeeID = empId;
                    employeeViewModel.FirstName = dt.firstName;
                    employeeViewModel.LastName = dt.lastName;
                    employeeViewModel.PhoneNumber = dt.Phone;
                    employeeViewModel.Email = dt.email;
                }

                employeeViewModel.ObjEmployeeList = _entities.SP_EmployeeList().ToList();

                return View("EmployeeDetails", employeeViewModel);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}