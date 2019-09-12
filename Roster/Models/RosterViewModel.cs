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
        [Required(ErrorMessage = "Employee Name is required")]
        public int employeeid { get; set; }
        public string employee_name { get; set; }
        public string immigration_status { get; set; }
        [Required(ErrorMessage = "Employee Basis is required")]
        public string employment_basis { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
       
        [Required(ErrorMessage = "Contract start date is required")]
        public DateTime? contract_start_date { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        [Required(ErrorMessage = "Contract end date is required")]
        public DateTime? contract_end_date { get; set; }
        public string billable { get; set; }
        public string client { get; set; }
        public string skills { get; set; }
        public string continuity_with_us { get; set; }
        public string client_manager_name { get; set; }
        public string phone_no { get; set; }
       // [EmailAddress]
        //public string empolyee_email_id { get; set; }
        [EmailAddress]
        public string client_email_id { get; set; }
        public string expenses_applicable { get; set; }
        public int payroll { get; set; }
        public string remote { get; set; }     
        public string business_name { get; set; }
        public string business_contact_name { get; set; }    
        public double? salary_or_hourly_rate { get; set; } 
        public double? hourly_cost { get; set; }
        public double? over_head { get; set; }
        public double? loaded_cost { get; set; }
        public double? medical { get; set; }
        public double? other_expenses { get; set; }
        [Required (ErrorMessage = "Bill rate is required ")]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter bill rate greater than 0")]
        public double? bill_rate { get; set; }
        public double? gross_margin { get; set; }
        public double? total_sow_value { get; set; }       
        public string po { get; set; }
        public double? payroll_hours { get; set; }
        public double? qb_hours { get; set; }
        public int exp_weekly_monthly { get; set; }
        public double? total_cost { get; set; }
        public double? overhead_per { get; set; }
        public List<SP_EmployeeList_Result> ObjEmployeeList { get; set; }
        public List<SP_ImmigrationStatusDetails_Result> Objimmigration_status { get; set; }
        public List<SP_PayrollExpenseDetails_Result> Objpayroll { get; set; }
        public List<SP_ExpenseFrequencyDetails_Result> Objexp_weekly_monthly { get; set; }
        public List<SP_EmploymentBasisDetails_Result> ObjEmploymentBasisDetails { get; set; }
        public bool? vendor { get; set; }
        public double? sick_leave_cost { get; set; }       
        public DateTime? po_effective_date { get; set; }
        public string error_message { get; set; }
        public string add_comment { get; set; }
    }
}