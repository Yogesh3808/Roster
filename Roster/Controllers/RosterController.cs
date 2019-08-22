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
                objRosterViewModel.ObjEmployeeList = _entities.SP_EmployeeList().ToList();
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
         
                    var dt = UnitOfWork.GetRepositoryInstance<roster>().GetFirstOrDefault(model.roster_id);
                    dt = dt ?? new roster();
                    dt.employee_id = model.employeeid;
                    dt.immigration_status = model.immigration_status;
                    dt.employment_basis = model.employment_basis;
                    dt.contract_start_date = model.contract_start_date;
                    dt.contract_end_date = model.contract_end_date;
                    dt.billable = model.billable;
                    dt.client = model.client;
                    dt.skills = model.skills;
                    dt.continuity_with_us = model.continuity_with_us;
                    dt.client_manager_name = model.client_manager_name;
                    dt.phone_no = model.phone_no;
                    dt.empolyee_email_id = model.empolyee_email_id;
                    dt.client_email_id = model.client_email_id;
                    dt.expenses_applicable = model.expenses_applicable;
                    dt.payroll = model.payroll;
                    dt.remote = model.remote;
                //dt.business_name = model.business_name;
        
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
                    dt.exp_weekly_monthly = model.exp_weekly_monthly;
                    dt.total_cost = model.total_cost;
                    dt.overhead_per = model.overhead_per;
                    dt.vendor = model.vendor;
                    dt.sick_leave_cost = model.sick_leave_cost;
                    dt.po_effective_date = model.po_effective_date;
                    dt.business_name = model.business_name;      
                if (dt.roster_id == 0)
                    {
                        dt.created_datetime = DateTime.Now;
                        UnitOfWork.GetRepositoryInstance<roster>().Add(dt);
                        TempData["msg3"] = "<script language='javascript' type='text/javascript'>alert('Roster Added Successfully!');</script>";
                    }
                    else
                    {
                        UnitOfWork.GetRepositoryInstance<roster>().Update(dt);
                        UnitOfWork.SaveChanges();
                        TempData["msg3"] = "<script language='javascript' type='text/javascript'>alert('Changes Saved Successfully!');</script>";
                    }                  
                
                model.ObjEmployeeList = _entities.SP_EmployeeList().ToList();
                return RedirectToAction("Index", "Roster");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet]
        public ActionResult GetRosterbyId(int roster_id)
        {
            try
            {
                var model = UnitOfWork.GetRepositoryInstance<roster>().GetFirstOrDefault(roster_id);
                var dt = new RosterViewModel();
                dt.ObjEmployeeList = _entities.SP_EmployeeList().ToList();
                if (model != null)
                {
                    dt.roster_id = roster_id;
                    dt.employeeid = model.employee_id;
                    dt.immigration_status = model.immigration_status;
                    dt.employment_basis = model.employment_basis;
                    dt.contract_start_date = model.contract_start_date;
                    dt.contract_end_date = model.contract_end_date;
                    dt.billable = model.billable;
                    dt.client = model.client;
                    dt.skills = model.skills;
                    dt.continuity_with_us = model.continuity_with_us;
                    dt.client_manager_name = model.client_manager_name;
                    dt.phone_no = model.phone_no;
                    dt.empolyee_email_id = model.empolyee_email_id;
                    dt.client_email_id = model.client_email_id;
                    dt.expenses_applicable = model.expenses_applicable;
                    dt.payroll = model.payroll;
                    dt.remote = model.remote;
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
                    dt.exp_weekly_monthly = model.exp_weekly_monthly;
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
                        if (Convert.ToInt32(Session["MemberRole"])== 1  || Convert.ToInt32(Session["MemberRole"]) == 2)                       
                            dt.Columns.Add(item.excel_export_header_title);
                        else if((Convert.ToInt32(Session["MemberRole"]) == 3 && item.is_uservisible == true))
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
                           item.expenses_weekly_or_monthly, item.payroll, item.business_name, item.business_contact_name,item.vendor);
                        else if (Convert.ToInt32(Session["MemberRole"]) == 4)
                            dt.Rows.Add(item.employee_name, item.phone_no, item.empolyee_email_id, $"{item.contract_start_date:MM/dd/yyyy}", $"{item.contract_end_date:MM/dd/yyyy}",
                           item.billable, item.client, item.client_manager_name, item.client_email_id, item.skills, item.continuity_with_us, item.remote, item.expenses_applicable,
                           item.expenses_weekly_or_monthly, item.payroll, item.business_name, item.business_contact_name, item.vendor);
                        else if (Convert.ToInt32(Session["MemberRole"]) == 5)
                            dt.Rows.Add(item.employee_name,  item.immigration_status, item.employment_basis, item.phone_no, item.empolyee_email_id, $"{item.contract_start_date:MM/dd/yyyy}", $"{item.contract_end_date:MM/dd/yyyy}",
                           item.billable, item.client, item.client_manager_name, item.client_email_id, item.skills, item.continuity_with_us, item.remote, item.expenses_applicable,
                           item.expenses_weekly_or_monthly, item.payroll, item.business_name, item.business_contact_name, item.vendor,item.bill_rate);                        
                        else if(Convert.ToInt32(Session["MemberRole"]) == 1 || Convert.ToInt32(Session["MemberRole"]) == 2)
                            dt.Rows.Add(item.employee_name, item.immigration_status, item.employment_basis, item.phone_no, item.empolyee_email_id, $"{item.contract_start_date:MM/dd/yyyy}", $"{item.contract_end_date:MM/dd/yyyy}",
                             item.billable, item.client, item.client_manager_name, item.client_email_id, item.skills, item.continuity_with_us, item.remote, item.expenses_applicable,
                             item.expenses_weekly_or_monthly, item.payroll, item.business_name, item.business_contact_name, item.vendor, $"{item.salary_or_hourly_rate:0.00}",
                             $"{item.hourly_cost:0.00}", $"{item.over_head:0.00}", item.overhead_per, $"{item.loaded_cost:0.00}", $"{item.medical:0.00}",
                             $"{item.other_expenses:0.00}", $"{item.total_cost:0.00}", $"{item.bill_rate:0.00}", $"{item.gross_margin:0.00}", item.po, $"{item.total_sow_value:0.00}");
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
                var RosterDetails = _entities.SP_GetEmployeeRosterDetails(Convert.ToInt32(EmployeeId)).ToList();
                 return Json(RosterDetails, JsonRequestBehavior.AllowGet);
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
    }
}