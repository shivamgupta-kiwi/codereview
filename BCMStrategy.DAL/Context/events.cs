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
    
    public partial class events
    {
        public int Id { get; set; }
        public int ProcessEventId { get; set; }
        public int ProcessTypeId { get; set; }
        public Nullable<int> ProcessInstanceId { get; set; }
        public System.DateTime StartDateTime { get; set; }
        public Nullable<System.DateTime> EndDateTime { get; set; }
        public Nullable<decimal> TimeTaken { get; set; }
        public Nullable<int> PagesProcessed { get; set; }
    
        public virtual processevents processevents { get; set; }
        public virtual processinstances processinstances { get; set; }
        public virtual processtypes processtypes { get; set; }
    }
}