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
    
    public partial class institution
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public institution()
        {
            this.scrapperstandardtags_entitytypes = new HashSet<scrapperstandardtags_entitytypes>();
            this.weblinks = new HashSet<weblinks>();
        }
    
        public int Id { get; set; }
        public Nullable<int> CountryId { get; set; }
        public int InstitutionTypeId { get; set; }
        public string InstitutionName { get; set; }
        public bool IsEuropean { get; set; }
        public string EntityName { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public Nullable<System.DateTime> Modified { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }
    
        public virtual country country { get; set; }
        public virtual institutiontypes institutiontypes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<scrapperstandardtags_entitytypes> scrapperstandardtags_entitytypes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<weblinks> weblinks { get; set; }
    }
}
