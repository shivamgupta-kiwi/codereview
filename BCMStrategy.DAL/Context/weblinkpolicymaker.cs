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
    
    public partial class weblinkpolicymaker
    {
        public int Id { get; set; }
        public int WebLinkId { get; set; }
        public int PolicyMakerId { get; set; }
    
        public virtual policymakers policymakers { get; set; }
        public virtual weblinks weblinks { get; set; }
    }
}