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
    using System.Collections.Generic;
    
    public partial class excel_export_header
    {
        public int excel_export_id { get; set; }
        public int excel_export_type_id { get; set; }
        public Nullable<int> excel_export_srno { get; set; }
        public string excel_export_header_title { get; set; }
        public bool is_active { get; set; }
        public Nullable<bool> is_uservisible { get; set; }
        public Nullable<bool> is_salesvisible { get; set; }
        public Nullable<bool> is_invoicevisible { get; set; }
    }
}