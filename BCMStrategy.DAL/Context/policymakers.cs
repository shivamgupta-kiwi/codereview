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
    
    public partial class policymakers
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public policymakers()
        {
            this.weblinkpolicymaker = new HashSet<weblinkpolicymaker>();
        }
    
        public int Id { get; set; }
        public Nullable<int> CountryId { get; set; }
        public Nullable<int> DesignationId { get; set; }
        public int InstitutionId { get; set; }
        public string PolicyFirstName { get; set; }
        public string PolicyLastName { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public Nullable<System.DateTime> Modified { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
    
        public virtual country country { get; set; }
        public virtual designation designation { get; set; }
        public virtual institutiontypes institutiontypes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<weblinkpolicymaker> weblinkpolicymaker { get; set; }
    }
}
