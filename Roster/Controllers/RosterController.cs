using ClosedXML.Excel;
using Roster.Filter;
using Roster.Models;
using Roster.RosterDLL;
using RosterSystem.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ROSTER.Controllers
{
    [ValidateUserSession]
    [ActionFilter]
    public class RosterController : Controller
    {

        private readonly RosterEntities _entities = new RosterEntities();
        public GenericUnitOfWork UnitOfWork = new GenericUnitOfWork();
        // GET: Roster
        public ActionResult Index()
        {
            try
            {
                return View(_entities.SP_GetAllRosterDetails().ToList().Count != 0 ? _entities.SP_GetAllRosterDetails().ToList() : null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult RosterDetails()
        {
            try
            {
                RosterViewModel objRosterViewModel = new RosterViewModel();
                FilldefaultValue(objRosterViewModel);
                FillMasterDetails(objRosterViewModel);

                return View("RosterDetails", objRosterViewModel);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private void FillMasterDetails(RosterViewModel objRosterViewModel)
        {
            try
            {
                objRosterViewModel.ObjEmployeeList = _entities.SP_EmployeeList().ToList();
                objRosterViewModel.Objimmigration_status = _entities.SP_ImmigrationStatusDetails().ToList();
                objRosterViewModel.ObjEmploymentBasisDetails = _entities.SP_EmploymentBasisDetails().ToList();
                objRosterViewModel.Objpayroll = _entities.SP_PayrollExpenseDetails().ToList();
                objRosterViewModel.Objexp_weekly_monthly = _entities.SP_ExpenseFrequencyDetails().ToList();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public ActionResult NewAddedEmployee()
        {
            try
            {
                RosterViewModel objRosterViewModel = new RosterViewModel();
                FilldefaultValue(objRosterViewModel);
                FillMasterDetails(objRosterViewModel);
                if (Convert.ToInt32(Session["CheckRoster"]) == 1)
                    objRosterViewModel.employeeid = Convert.ToInt32(Session["EmployeeId"]);

                return View("RosterDetails", objRosterViewModel);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RosterDetails(RosterViewModel model)
        {
            try
            {

                FillMasterDetails(model);
                if (!validation(model))
                {
                    return View("RosterDetails", model);
                }


                var dt = UnitOfWork.GetRepositoryInstance<roster>().GetFirstOrDefault(model.roster_id);
                dt = dt ?? new roster();
                #region //pooname code 
                if (!string.IsNullOrEmpty(model.immigration_status))
                {
                    int id = Getimmigrationstatusid(model.immigration_status);
                    if (id != 0)
                    {
                        dt.immigration_status_id = id;
                    }
                }
                else
                {
                    dt.immigration_status_id = null;
                }
                if (!string.IsNullOrEmpty(Convert.ToString((model.employment_basis))))
                {
                    int id = Getemploymentbasisid(Convert.ToString(model.employment_basis));
                    if (id != 0)
                    {
                        dt.employment_basis_id = id;
                    }
                }
                else
                {
                    dt.employment_basis_id = null;
                }
                #endregion
                dt.employee_id = model.employeeid;

                //dt.immigration_status_id = Convert.ToInt32(model.immigration_status);
                //dt.employment_basis_id = Convert.ToInt32(model.employment_basis);
                dt.contract_start_date = model.contract_start_date;
                dt.contract_end_date = model.contract_end_date;
                dt.billable = model.billable;
                dt.client = model.client;
                dt.skills = model.skills;
                dt.continuity_with_us = model.continuity_with_us;
                dt.client_manager_name = model.client_manager_name;
                dt.phone_no = model.phone_no;
                //dt.empolyee_email_id = model.empolyee_email_id;
                dt.client_email_id = model.client_email_id;
                dt.expenses_applicable = model.expenses_applicable;
                dt.payroll_id = Convert.ToInt32(model.payroll);
                dt.remote = model.remote;
                dt.business_contact_name = model.business_contact_name;
                dt.salary_or_hourly_rate = model.salary_or_hourly_rate;
                dt.hourly_cost = model.hourly_cost;
                dt.over_head = model.over_head;
                dt.loaded_cost = model.loaded_cost;
                dt.medical = model.medical;
                dt.other_expenses = model.other_expenses;
                dt.bill_rate = model.bill_rate;
                dt.gross_margin = model.gross_margin;
                dt.total_sow_value = model.total_sow_value;
                dt.po = model.po;
                dt.payroll_hours = model.payroll_hours;
                dt.qb_hours = model.qb_hours;
                dt.exp_weekly_monthly_id = Convert.ToInt32(model.exp_weekly_monthly);
                dt.total_cost = model.total_cost;
                dt.overhead_per = model.overhead_per;
                dt.add_comment = model.add_comment;
                dt.vendor = model.vendor;
                dt.sick_leave_cost = model.sick_leave_cost;
                dt.po_effective_date = model.po_effective_date;
                dt.business_name = model.business_name;
                dt.Status = "Y";
                if (dt.roster_id == 0)
                {
                    //dt.created_by_user_id=Session
                    dt.created_datetime = DateTime.Now;
                    UnitOfWork.GetRepositoryInstance<roster>().Add(dt);
                    TempData["msg3"] = "<script language='javascript' type='text/javascript'>alert('Roster Added Successfully!');</script>";
                }
                else
                {
                    dt.last_updated_datetime = DateTime.Now;
                    UnitOfWork.GetRepositoryInstance<roster>().Update(dt);
                    UnitOfWork.SaveChanges();
                    TempData["msg3"] = "<script language='javascript' type='text/javascript'>alert('Changes Saved Successfully!');</script>";
                }
                return RedirectToAction("Index", "Roster");

            }

            catch (Exception ex)
            {
                throw;
            }
        }



        private bool validation(RosterViewModel model)
        {
            bool IsValid = true;
            if (model.employeeid == 0)
            {
                ModelState.AddModelError("", "");
                IsValid = false;
            }
            else if (model.employment_basis == "")
            {
                ModelState.AddModelError("", "");
                IsValid = false;
            }
           else if (model.po_effective_date.ToString() == null || model.po_effective_date.ToString() == "")
            {
                ModelState.AddModelError("", "");
                IsValid = false;
            }
            else if (model.contract_start_date.ToString() == null || model.contract_start_date.ToString() == "")
            {
                ModelState.AddModelError("", "");
                IsValid = false;
            }
            else if (model.contract_end_date.ToString() == null || model.contract_end_date.ToString() == "")
            {
                ModelState.AddModelError("", "");
                IsValid = false;
            }
            else if (model.bill_rate == 0 || model.bill_rate.ToString() == "")
            {
                ModelState.AddModelError("", "");
                IsValid = false;
            }
            if (model.employment_basis == "" || model.employment_basis ==null)
            {
                if (model.business_name == null)
                {
                    ModelState.AddModelError("business_name", "Business Name required");
                    IsValid = false;
                }
            }
            if (model.salary_or_hourly_rate == 0 || model.salary_or_hourly_rate.ToString() == "")
            {
                if (model.hourly_cost == 0 || model.hourly_cost.ToString() == "")
                {
                    ModelState.AddModelError("salary_or_hourly_rate", "Please enter Salary greater than 0");
                    IsValid = false;
                }
            }
            if (model.hourly_cost == 0 || model.hourly_cost.ToString() == "")
            {
                if (model.salary_or_hourly_rate == 0 || model.salary_or_hourly_rate.ToString() == "")
                {
                    ModelState.AddModelError("hourly_cost", "Please enter hourly cost greater than 0");
                    IsValid = false;
                }
            }
            return IsValid;
        }

        [HttpGet]
        public ActionResult GetRosterbyId(int roster_id)
        {
            try
            {
                var model = UnitOfWork.GetRepositoryInstance<roster>().GetFirstOrDefault(roster_id);
                var dt = new RosterViewModel();
                FillMasterDetails(dt);
                if (model != null)
                {
                    dt.roster_id = roster_id;
                    dt.employeeid = model.employee_id;
                    //dt.immigration_status =Convert.ToInt32(model.immigration_status_id);
                    //dt.employment_basis =Convert.ToInt32( model.employment_basis_id);
                    dt.immigration_status = GetTextimmigrationName(Convert.ToInt32(model.immigration_status_id));
                    dt.employment_basis = GetTextemploymentbasisname(Convert.ToInt32(model.employment_basis_id));
                    dt.contract_start_date = model.contract_start_date;
                    dt.contract_end_date = model.contract_end_date;
                    dt.billable = model.billable;
                    dt.client = model.client;
                    dt.skills = model.skills;
                    dt.continuity_with_us = model.continuity_with_us;
                    dt.client_manager_name = model.client_manager_name;
                    dt.phone_no = model.phone_no;
                    /// dt.empolyee_email_id = model.empolyee_email_id;
                    dt.client_email_id = model.client_email_id;
                    dt.expenses_applicable = model.expenses_applicable;
                    dt.payroll = Convert.ToInt32(model.payroll_id);
                    dt.remote = model.remote;
                    dt.add_comment = model.add_comment;
                    dt.business_name = model.business_name;
                    dt.business_contact_name = model.business_contact_name;
                    dt.salary_or_hourly_rate = model.salary_or_hourly_rate;
                    dt.hourly_cost = model.hourly_cost;
                    dt.over_head = model.over_head;
                    dt.loaded_cost = model.loaded_cost;
                    dt.medical = model.medical;
                    dt.other_expenses = model.other_expenses;
                    dt.bill_rate = model.bill_rate;
                    dt.gross_margin = model.gross_margin;
                    dt.total_sow_value = model.total_sow_value;
                    dt.po = model.po;
                    dt.payroll_hours = model.payroll_hours;
                    dt.qb_hours = model.qb_hours;
                    dt.exp_weekly_monthly = Convert.ToInt32(model.exp_weekly_monthly_id);
                    dt.total_cost = model.total_cost;
                    dt.overhead_per = model.overhead_per;
                    dt.vendor = model.vendor;
                    dt.sick_leave_cost = model.sick_leave_cost;
                    dt.po_effective_date = model.po_effective_date;
                }

                return View("RosterDetails", dt);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #region Export Excel
        [HttpPost]
        public FileResult Export()
        {
            try
            {
                string file_name = string.Concat("RosterDetails_", DateTime.Now.ToString("yyyyMMddHHmmssfff"), ".xlsx");
                var excel_export_headers = _entities.excel_export_header.ToList().Where(x => x.is_active == true && x.excel_export_type_id == 1).OrderBy(x => x.excel_export_srno);
                var dt = new DataTable("RosterDetails");
                if (excel_export_headers != null)
                {

                    foreach (var item in excel_export_headers)
                    {
                        if (Convert.ToInt32(Session["MemberRole"]) == 1 || Convert.ToInt32(Session["MemberRole"]) == 2)
                            dt.Columns.Add(item.excel_export_header_title);
                        else if ((Convert.ToInt32(Session["MemberRole"]) == 3 && item.is_uservisible == true))
                            dt.Columns.Add(item.excel_export_header_title);
                        else if ((Convert.ToInt32(Session["MemberRole"]) == 4 && item.is_salesvisible == true))
                            dt.Columns.Add(item.excel_export_header_title);
                        else if ((Convert.ToInt32(Session["MemberRole"]) == 5 && item.is_invoicevisible == true))
                            dt.Columns.Add(item.excel_export_header_title);



                    }
                }
                var details = _entities.SP_GetAllRosterDetails().ToList();
                if (details != null)
                {
                    foreach (var item in details)
                    {
                        if (Convert.ToInt32(Session["MemberRole"]) == 3)
                            dt.Rows.Add(item.employee_name, item.immigration_status, item.employment_basis, item.phone_no, item.empolyee_email_id, $"{item.contract_start_date:MM/dd/yyyy}", $"{item.contract_end_date:MM/dd/yyyy}",
                           item.billable, item.client, item.client_manager_name, item.client_email_id, item.skills, item.continuity_with_us, item.remote, item.expenses_applicable,
                           item.expenses_weekly_or_monthly, item.payroll, item.business_name, item.business_contact_name, item.vendor);
                        else if (Convert.ToInt32(Session["MemberRole"]) == 4)
                            dt.Rows.Add(item.employee_name, item.phone_no, item.empolyee_email_id, $"{item.contract_start_date:MM/dd/yyyy}", $"{item.contract_end_date:MM/dd/yyyy}",
                           item.billable, item.client, item.client_manager_name, item.client_email_id, item.skills, item.continuity_with_us, item.remote, item.expenses_applicable,
                           item.expenses_weekly_or_monthly, item.payroll, item.business_name, item.business_contact_name, item.vendor, item.bill_rate);
                        else if (Convert.ToInt32(Session["MemberRole"]) == 5)
                            dt.Rows.Add(item.employee_name, item.immigration_status, item.employment_basis, item.phone_no, item.empolyee_email_id, $"{item.contract_start_date:MM/dd/yyyy}", $"{item.contract_end_date:MM/dd/yyyy}",
                           item.billable, item.client, item.client_manager_name, item.client_email_id, item.skills, item.continuity_with_us, item.remote, item.expenses_applicable,
                           item.expenses_weekly_or_monthly, item.payroll, item.business_name, item.business_contact_name, item.vendor, $"{item.hourly_cost:0.00}", item.bill_rate);
                        else if (Convert.ToInt32(Session["MemberRole"]) == 1 || Convert.ToInt32(Session["MemberRole"]) == 2)
                            dt.Rows.Add(item.employee_name, item.immigration_status, item.employment_basis, item.phone_no, item.empolyee_email_id, $"{item.contract_start_date:MM/dd/yyyy}", $"{item.contract_end_date:MM/dd/yyyy}",
                             item.billable, item.client, item.client_manager_name, item.client_email_id, item.skills, item.continuity_with_us, item.remote, item.expenses_applicable,
                             item.expenses_weekly_or_monthly, item.payroll, item.business_name, item.business_contact_name, item.vendor, $"{item.salary_or_hourly_rate:0.00}",
                             $"{item.hourly_cost:0.00}", $"{item.over_head:0.00}", item.overhead_per, $"{item.loaded_cost:0.00}", $"{item.medical:0.00}", $"{item.sick_leave_cost:0.00}",
                             $"{item.other_expenses:0.00}", $"{item.total_cost:0.00}", $"{item.bill_rate:0.00}", $"{item.gross_margin:0.00}", item.po, item.po_effective_date, $"{item.total_sow_value:0.00}");
                    }
                }
                // add ClosedXML ref
                using (var wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(dt);
                    using (var stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", file_name);
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion


        #region Check Employee Roster Exist or not
        public ActionResult EmployeeRosterDetails(string EmployeeId)
        {

            try
            {
                RosterViewModel model = new RosterViewModel();
                FillMasterDetails(model);
                if (EmployeeId != "")
                {
                    var RosterDetails = _entities.SP_GetEmployeeRosterDetails(Convert.ToInt32(EmployeeId)).ToList();
                    return Json(RosterDetails, JsonRequestBehavior.AllowGet);
                }
                else { return Json(""); }

            }
            catch (Exception ex)
            {
                throw;
            }


        }

        #endregion

        #region Default Value 2 & 0 by Vrushali
        private void FilldefaultValue(RosterViewModel objRosterViewModel)
        {
            try
            {

                objRosterViewModel.salary_or_hourly_rate = 0.00;
                objRosterViewModel.hourly_cost = 0.00;
                objRosterViewModel.loaded_cost = 0.00;
                objRosterViewModel.overhead_per = 0;
                objRosterViewModel.over_head = 10.00;
                objRosterViewModel.total_cost = 0.00;
                objRosterViewModel.bill_rate = 0.00;
                objRosterViewModel.gross_margin = 0.00;
                objRosterViewModel.total_sow_value = 0.00;
                objRosterViewModel.medical = 0.00;
                objRosterViewModel.other_expenses = 0.00;
                objRosterViewModel.sick_leave_cost = 0.00;
                objRosterViewModel.billable = "NB";
                objRosterViewModel.expenses_applicable = "N";
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion


        #region //pooname code 
        public int Getemploymentbasisid(string employmentbasisname)
        {
            if (string.IsNullOrEmpty(employmentbasisname))
            {
                return 0;
            }
            using (var db = new RosterEntities())
            {
                var result = db.employment_basis.FirstOrDefault(x => x.employment_basis_desc == employmentbasisname);
                return (result == null) ? Convert.ToInt32(InsertEmployementbasis(employmentbasisname)) : Convert.ToInt32(result.employment_basis_id);
            }
        }
        public int InsertEmployementbasis(string employmentbasisname)
        {
            var newEntry = new employment_basis
            {
                employment_basis_desc = employmentbasisname,
                is_active = true
            };
            using (var db = new RosterEntities())
            {
                db.employment_basis.Add(newEntry);
                db.SaveChanges();
                return newEntry.employment_basis_id;
            }
        }
        public string GetTextemploymentbasisname(int employmentbasisid)
        {
            if (string.IsNullOrEmpty(Convert.ToString(employmentbasisid)))
            {
                return null;
            }
            using (var db = new RosterEntities())
            {
                var result = db.employment_basis.FirstOrDefault(x => x.employment_basis_id == employmentbasisid);
                return (Convert.ToString(result.employment_basis_desc));
            }
        }
        public int Getimmigrationstatusid(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return 0;
            }
            using (var db = new RosterEntities())
            {
                var result = db.immigration_status.FirstOrDefault(x => x.immigration_status_desc == name);
                return (result == null) ? Convert.ToInt32(InsertJobTitle(name)) : Convert.ToInt32(result.immigration_status_id);
            }
        }
        public string GetTextimmigrationName(int immigration_statu)
        {
            if (string.IsNullOrEmpty(Convert.ToString(immigration_statu)))
            {
                return null;
            }
            using (var db = new RosterEntities())
            {
                var result = db.immigration_status.FirstOrDefault(x => x.immigration_status_id == immigration_statu);
                return (Convert.ToString(result.immigration_status_desc));
            }
        }
        public int InsertJobTitle(string name)
        {
            var newEntry = new immigration_status
            {
                immigration_status_desc = name,
                is_active = true
            };
            using (var db = new RosterEntities())
            {
                db.immigration_status.Add(newEntry);
                db.SaveChanges();
                return newEntry.immigration_status_id;
            }
        }

        [HttpPost]
        public JsonResult GetDropDowndataforBind(string search_with_name, string dropdown_id)
        {
            if (!string.IsNullOrEmpty(dropdown_id) && dropdown_id == "immigration_status")
            {
                var immigration_status = (from c in _entities.SP_ImmigrationStatusDetails()
                                          where c.immigration_status.ToLower().Contains(search_with_name.ToLower())
                                          select new { c.immigration_status, c.immigration_status_id });
                return Json(immigration_status, JsonRequestBehavior.AllowGet);
            }
            else if (!string.IsNullOrEmpty(dropdown_id) && dropdown_id == "employment_basis")
            {
                var employment_basis = (from c in _entities.SP_EmploymentBasisDetails()
                                        where c.employment_basis.ToLower().Contains(search_with_name.ToLower())
                                        select new { c.employment_basis, c.employment_basis_id });
                return Json(employment_basis, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        #endregion

        ///delete roster details
        [HttpGet]
        public ActionResult DeleteRoster(int roster_id)
        {
            try
            {
                var dt = UnitOfWork.GetRepositoryInstance<roster>().GetFirstOrDefault(roster_id);
                dt = dt ?? new roster();
                dt.last_updated_by_user_id = Session["UserName"].ToString();
                dt.Status = "N";
                UnitOfWork.GetRepositoryInstance<roster>().Update(dt);
                UnitOfWork.SaveChanges();
                TempData["msg3"] = "<script language='javascript' type='text/javascript'>alert('delete Successfully!');</script>";
                return RedirectToAction("Index", "Roster");
            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }
}