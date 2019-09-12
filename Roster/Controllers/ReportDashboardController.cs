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
using static Roster.Models.ReportCountViewModel;

namespace Roster.Controllers
{
    [ValidateUserSession]
    [ActionFilter]
    public class ReportDashboardController : Controller
    {
        // GET: ReportDashboard
        private readonly RosterEntities _entities = new RosterEntities();
        public GenericUnitOfWork UnitOfWork = new GenericUnitOfWork();
        public ActionResult Index()
        {
            try
            {
                return View(_entities.SP_ReportList().ToList().Count != 0 ? _entities.SP_ReportList().ToList() : null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult GetReportDetails(string report_id)
        {
            try
            {
                if (report_id == "2")
                {
                    return RedirectToAction("EmployeeDetailbyCustomerReport", "Report");
                }
                if (report_id == "4")
                {
                    return RedirectToAction("RptVendor", "Report");
                }
                if (report_id == "5")
                {
                    return RedirectToAction("ContractEndreport", "Report");
                }
                return RedirectToAction("Index", "ReportDashboard");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult ReportCount(string report_id , string ReportName)
        {
            try
            {
                ReportCountViewModel model = new ReportCountViewModel();
                if (report_id == "4")
                {                  
                    return RedirectToAction("RptVendor", "Report");
                }
                else if (report_id == "5")
                {                  
                    return RedirectToAction("ContractEndreport", "Report");
                }
                ViewBag.data = ReportName;
                ViewBag.reportId = report_id;
                Session["report_id"] = report_id;
                return View(_entities.sp_createsummaryreport(Convert.ToInt32(report_id)).ToList().Count != 0 ? _entities.sp_createsummaryreport(Convert.ToInt32(report_id)).ToList() : null);
                
             }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #region Yogesh  Export Excel
        [HttpPost]
        public FileResult Export()
        {
            try
            {
                string file_name = string.Concat("ReportSummary_", DateTime.Now.ToString("yyyyMMddHHmmssfff"), ".xlsx");
                DataTable dt = new DataTable();
                dt.Clear();
                dt = new DataTable("ReportSummary");
                dt.Columns.Add("Description");
                dt.Columns.Add("Bilable");
                dt.Columns.Add("Non Bilable");
                dt.Columns.Add("Terminated");
                dt.Columns.Add("Total");
                var details = (_entities.sp_createsummaryreport(Convert.ToInt32(Session["report_id"])).ToList().Count != 0 ? _entities.sp_createsummaryreport(Convert.ToInt32(Session["report_id"])).ToList() : null);    
                if (details != null)
                {
                    foreach (var item in details)
                    {
                     
                            dt.Rows.Add(item.description,item.Billable,item.Non_Billable,item.Terminated,item.Total);
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
    }
}