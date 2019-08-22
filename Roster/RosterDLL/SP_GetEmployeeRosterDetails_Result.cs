//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Roster.RosterDLL
{
    using System;
    
    public partial class SP_GetEmployeeRosterDetails_Result
    {
        public int roster_id { get; set; }
        public int employee_id { get; set; }
        public string immigration_status { get; set; }
        public string employment_basis { get; set; }
        public Nullable<System.DateTime> contract_start_date { get; set; }
        public Nullable<System.DateTime> contract_end_date { get; set; }
        public string billable { get; set; }
        public string client { get; set; }
        public string skills { get; set; }
        public string continuity_with_us { get; set; }
        public string client_manager_name { get; set; }
        public string phone_no { get; set; }
        public string empolyee_email_id { get; set; }
        public string client_email_id { get; set; }
        public string expenses_applicable { get; set; }
        public string payroll { get; set; }
        public string remote { get; set; }
        public string business_name { get; set; }
        public string business_contact_name { get; set; }
        public Nullable<double> salary_or_hourly_rate { get; set; }
        public Nullable<double> hourly_cost { get; set; }
        public Nullable<double> over_head { get; set; }
        public Nullable<double> loaded_cost { get; set; }
        public Nullable<double> medical { get; set; }
        public Nullable<double> other_expenses { get; set; }
        public Nullable<double> bill_rate { get; set; }
        public Nullable<double> gross_margin { get; set; }
        public Nullable<double> total_sow_value { get; set; }
        public string po { get; set; }
        public Nullable<double> payroll_hours { get; set; }
        public Nullable<double> qb_hours { get; set; }
        public Nullable<int> created_by_user_id { get; set; }
        public Nullable<System.DateTime> created_datetime { get; set; }
        public Nullable<int> last_updated_by_user_id { get; set; }
        public Nullable<System.DateTime> last_updated_datetime { get; set; }
        public string exp_weekly_monthly { get; set; }
        public Nullable<double> total_cost { get; set; }
        public Nullable<double> overhead_per { get; set; }
        public Nullable<bool> vendor { get; set; }
        public Nullable<double> sick_leave_cost { get; set; }
        public Nullable<System.DateTime> po_effective_date { get; set; }
    }
}
