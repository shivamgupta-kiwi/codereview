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
    
    public partial class defaultlexiconterms
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int LexiconTypeId { get; set; }
        public int LexiconIssuesId { get; set; }
        public System.DateTime Created { get; set; }
        public string CreatedBy { get; set; }
    
        public virtual lexiconissues lexiconissues { get; set; }
        public virtual lexicontype lexicontype { get; set; }
        public virtual user user { get; set; }
    }
}
