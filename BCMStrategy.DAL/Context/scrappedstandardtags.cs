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
    
    public partial class scrappedstandardtags
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public scrappedstandardtags()
        {
            this.scrappedstandardtag_policymakers = new HashSet<scrappedstandardtag_policymakers>();
            this.scrappedstandardtags_sectors = new HashSet<scrappedstandardtags_sectors>();
            this.scrapperstandardtags_entitytypes = new HashSet<scrapperstandardtags_entitytypes>();
        }
    
        public int Id { get; set; }
        public Nullable<int> ScanningLinkDetailsId { get; set; }
        public byte[] Content { get; set; }
        public Nullable<System.DateTime> DateOfIssue { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public string SearchType { get; set; }
    
        public virtual scanninglinkdetails scanninglinkdetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<scrappedstandardtag_policymakers> scrappedstandardtag_policymakers { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<scrappedstandardtags_sectors> scrappedstandardtags_sectors { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<scrapperstandardtags_entitytypes> scrapperstandardtags_entitytypes { get; set; }
    }
}
