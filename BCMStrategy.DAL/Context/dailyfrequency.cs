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
    
    public partial class dailyfrequency
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public dailyfrequency()
        {
            this.scraperenginesetup = new HashSet<scraperenginesetup>();
        }
    
        public int Id { get; set; }
        public Nullable<System.DateTime> OccursOnceAt { get; set; }
        public Nullable<int> OccursEveryHour { get; set; }
        public Nullable<System.DateTime> OccursStartTime { get; set; }
        public Nullable<System.DateTime> OccursEndTime { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<scraperenginesetup> scraperenginesetup { get; set; }
    }
}