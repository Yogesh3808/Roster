using Roster.RosterDLL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Roster.Models
{
    public class ReportViewModels
    {
        public string Client { get; set; }
       public string billable { get; set; }
        public List<sp_EmployeeDetailbyCustomerList_Result> ObjEmployeeDetailbyCustomerList { get; set; }
        public List<sp_clientList_Result> Clientlist { get; set; }

        //vrushali vendar model
        public string business_name { get; set; }
        public List<sp_vendorwisedetails_Result> ObjVendorList { get; set; }
        public List<sp_vendorlist_Result> Vendorlist { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? contract_start_date { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? contract_end_date { get; set; }
        public List<sp_createcontractendreport_Result> ObjContractEnd { get; set; }
        public bool is_vendor { get; set; }
    }
}