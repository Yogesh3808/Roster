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

namespace Roster.Controllers
{
    [ValidateUserSession]
    [ActionFilter]
    public class ReportController : Controller
    {

        private readonly RosterEntities _entities = new RosterEntities();
        public GenericUnitOfWork UnitOfWork = new GenericUnitOfWork();
        ReportViewModels rs = new ReportViewModels();
        // GET: Report
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult EmployeeDetailbyCustomerReport(string billable, string Client)
        {
            ReportViewModels model = new ReportViewModels();
            model.billable = billable;
            model.Client = Client;
            EmployeeDetailbyCustomerReport(model);
            //rs.Client="All";
            //rs.billable = "A";
            //rs.ObjEmployeeDetailbyCustomerList = _entities.sp_EmployeeDetailbyCustomerList("A", "All").ToList();
            //rs.Clientlist = _entities.sp_clientList().ToList();
            return View(rs);
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult EmployeeDetailbyCustomerReport(ReportViewModels rs1)
        {

            rs.ObjEmployeeDetailbyCustomerList = _entities.sp_EmployeeDetailbyCustomerList(rs1.billable, rs1.Client).ToList();
            Session["billable"] = rs1.billable;
            Session["Client"] = rs1.Client;
            var Clist = rs.ObjEmployeeDetailbyCustomerList.Count;
            if (Clist != 0)
            {
                rs.Clientlist = _entities.sp_clientList().ToList();
                return View(rs);
            }
            else
            {
                TempData["msg"] = "Record Not Found!";
                rs.Clientlist = _entities.sp_clientList().ToList();
                return View(rs);
            }

        }

        #region Vrushali Vendor
        public ActionResult RptVendor()
        {
            rs.business_name = "All";
            rs.is_vendor = true;
            RptVendor(rs);
            rs.ObjVendorList = _entities.sp_vendorwisedetails("All", true).ToList();
            rs.Vendorlist = _entities.sp_vendorlist().ToList();
            return View(rs);
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult RptVendor(ReportViewModels rs1)
        {
            rs.ObjVendorList = _entities.sp_vendorwisedetails(rs1.business_name, rs1.is_vendor).ToList();
            Session["business_name"] = rs1.business_name;
            Session["is_vendor"] = rs1.is_vendor;
            var vlist = rs.ObjVendorList.Count;
            if (vlist != 0)
            {
                rs.Vendorlist = _entities.sp_vendorlist().ToList();
                return View(rs);
            }
            else
            {
                TempData["msg"] = "Record Not Found!";
                rs.Vendorlist = _entities.sp_vendorlist().ToList();
                rs.is_vendor = rs1.is_vendor;
                return View(rs);
            }
        }

        public ActionResult ContractEndreport()
        {
            rs.contract_start_date = DateTime.UtcNow;
            rs.contract_end_date = DateTime.UtcNow;
            rs.ObjContractEnd = _entities.sp_createcontractendreport(rs.contract_start_date, rs.contract_end_date).ToList();
            return View(rs);
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ContractEndreport(ReportViewModels rs1)
        {
            rs.ObjContractEnd = _entities.sp_createcontractendreport(rs1.contract_start_date, rs1.contract_end_date).ToList();
            Session["contract_start_date"] = rs1.contract_start_date;
            Session["contract_End_date"] = rs1.contract_end_date;
            var vlist = rs.ObjContractEnd.Count;
            if (vlist != 0)
            {
                return View(rs);
            }
            else
            {
                TempData["msg"] = "Record Not Found!";
                return View(rs);
            }
        }

        #endregion

        #region Yogesh  Export Excel
        [HttpPost]
        public FileResult ExportEmpDetails(ReportViewModels rs1)
        {
            try
            {
                string billable = rs1.billable;
                string nonbillable = rs1.Client;
                string file_name = string.Concat("EmployeeDetailbyCustomerReport_", DateTime.Now.ToString("yyyyMMddHHmmssfff"), ".xlsx");
                DataTable dt = new DataTable();
                dt.Clear();
                dt = new DataTable("EmployeeDetailbyCustomerReport");
                dt.Columns.Add("Name");
                dt.Columns.Add("Client");
                dt.Columns.Add("Contract Start Date");
                dt.Columns.Add("Contract End Date");
                dt.Columns.Add("Manage Name");
                dt.Columns.Add("Billable");
                var details = _entities.sp_EmployeeDetailbyCustomerList(Session["billable"].ToString(), Session["Client"].ToString()).ToList();
                if (details != null)
                {
                    foreach (var item in details)
                    {

                        dt.Rows.Add(item.employeename, item.client, item.contract_start_date, item.contract_end_date, item.client_manager_name, item.billable);
                    }
                }
                //add ClosedXML ref
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
        [HttpPost]
        public FileResult ExportContratDetails()
        {
            try
            {
                string file_name = string.Concat("ContractEndreport_", DateTime.Now.ToString("yyyyMMddHHmmssfff"), ".xlsx");
                DataTable dt = new DataTable();
                dt.Clear();
                dt = new DataTable("ContractEndreport");
                dt.Columns.Add("Employee Name");
                dt.Columns.Add("Client Name");
                dt.Columns.Add("Business Name");
                dt.Columns.Add("Client Manager Name");
                dt.Columns.Add("Contract Start Date");
                dt.Columns.Add("Contract End Date");
                var details = _entities.sp_createcontractendreport(Convert.ToDateTime(Session["contract_start_date"]), Convert.ToDateTime(Session["contract_End_date"])).ToList();
                if (details != null)
                {
                    foreach (var item in details)
                    {

                        dt.Rows.Add(item.employeename, item.client, item.business_name, item.client_manager_name, item.contract_start_date, item.contract_end_date);
                    }
                }
                //add ClosedXML ref
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
        [HttpPost]
        public FileResult ExportVenderDetails()
        {
            try
            {
                string file_name = string.Concat("VenderList", DateTime.Now.ToString("yyyyMMddHHmmssfff"), ".xlsx");
                DataTable dt = new DataTable();
                dt.Clear();
                dt = new DataTable("VenderList");
                dt.Columns.Add("Business Name");
                dt.Columns.Add("Empoyee Name");
                dt.Columns.Add("Client Name");
                dt.Columns.Add("Contract Start Date");
                dt.Columns.Add("Contract End Date");

                //var details = _entities.sp_vendorwisedetails(TempData["business_name"].ToString(), Convert.ToBoolean(TempData["is_vendor"])).ToList();
                var details = _entities.sp_vendorwisedetails(Session["business_name"].ToString(), Convert.ToBoolean(Session["is_vendor"])).ToList();
                if (details != null)
                {
                    foreach (var item in details)
                    {

                        dt.Rows.Add(item.business_name, item.employeename, item.client, item.contract_start_date, item.contract_end_date);
                    }
                }
                //add ClosedXML ref
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

    }
}