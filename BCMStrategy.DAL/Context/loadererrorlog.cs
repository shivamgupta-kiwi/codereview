//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BCMStrategy.DAL.Context
{
    using System;
    using System.Collections.Generic;
    
    public partial class loadererrorlog
    {
        public int Id { get; set; }
        public int ProcessId { get; set; }
        public int ProcessIntanceId { get; set; }
        public int WebSiteId { get; set; }
        public string SiteURL { get; set; }
        public byte[] ErrorDesc { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
    
        public virtual processevents processevents { get; set; }
        public virtual processinstances processinstances { get; set; }
        public virtual weblinks weblinks { get; set; }
    }
}
