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
    
    public partial class emailgenerationstatus
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TemplateId { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public System.DateTime SendAfterTime { get; set; }
        public System.DateTime SendBeforeTime { get; set; }
        public Nullable<System.DateTime> SendMoment { get; set; }
        public string Status { get; set; }
        public byte[] EmailBody { get; set; }
        public string EmailSubject { get; set; }
        public string ValidationKey { get; set; }
    
        public virtual emailtemplate emailtemplate { get; set; }
        public virtual user user { get; set; }
    }
}