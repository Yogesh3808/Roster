using Roster.RosterDLL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Roster.Models
{

    public class RosterViewModel
    {

        public int roster_id { get; set; }
        public int employeeid { get; set; }
        public string employee_name { get; set; }
        public string immigration_status { get; set; }
        public string employment_basis { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? contract_start_date { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? contract_end_date { get; set; }
        public string billable { get; set; }
        public string client { get; set; }
        public string skills { get; set; }
        public string continuity_with_us { get; set; }
        public string client_manager_name { get; set; }
        public string phone_no { get; set; }
        [EmailAddress]
        public string empolyee_email_id { get; set; }
        [EmailAddress]
        public string client_email_id { get; set; }
        public string expenses_applicable { get; set; }
        public string payroll { get; set; }
        public string remote { get; set; }
        public string business_name { get; set; }
        public string business_contact_name { get; set; }
        public double? salary_or_hourly_rate { get; set; }
        public double? hourly_cost { get; set; }
        public double? over_head { get; set; }
        public double? loaded_cost { get; set; }
        public double? medical { get; set; }
        public double? other_expenses { get; set; }
        public double? bill_rate { get; set; }
        public double? gross_margin { get; set; }
        public double? total_sow_value { get; set; }
        public string po { get; set; }
        public double? payroll_hours { get; set; }
        public double? qb_hours { get; set; }
        public string exp_weekly_monthly { get; set; }

        public double? total_cost { get; set; }
        public double? overhead_per { get; set; }

        public List<SP_EmployeeList_Result> ObjEmployeeList { get; set; }

        public bool? vendor { get; set; }
        public double? sick_leave_cost { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? po_effective_date { get; set; }

    }
}