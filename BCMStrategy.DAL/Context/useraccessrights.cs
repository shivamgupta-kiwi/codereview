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
    
    public partial class useraccessrights
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int SubMenuId { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public Nullable<System.DateTime> Modified { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
    
        public virtual submenu submenu { get; set; }
        public virtual user user { get; set; }
    }
}
