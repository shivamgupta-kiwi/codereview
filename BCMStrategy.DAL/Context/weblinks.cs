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
    
    public partial class weblinks
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public weblinks()
        {
            this.contentloaderlog = new HashSet<contentloaderlog>();
            this.loadererrorlog = new HashSet<loadererrorlog>();
            this.loaderlinkdocuments = new HashSet<loaderlinkdocuments>();
            this.loaderlinklog = new HashSet<loaderlinklog>();
            this.loaderlinkqueue = new HashSet<loaderlinkqueue>();
            this.scanninglinkqueue = new HashSet<scanninglinkqueue>();
            this.scraperenginesetup = new HashSet<scraperenginesetup>();
            this.weblinkactivitytype = new HashSet<weblinkactivitytype>();
            this.weblinkpagecontentregex = new HashSet<weblinkpagecontentregex>();
            this.weblinkpolicymaker = new HashSet<weblinkpolicymaker>();
            this.weblinkproprietarytags = new HashSet<weblinkproprietarytags>();
            this.weblinkrss = new HashSet<weblinkrss>();
            this.weblinksector = new HashSet<weblinksector>();
            this.weburlcategorymapping = new HashSet<weburlcategorymapping>();
        }
    
        public int Id { get; set; }
        public int WebSiteTypeId { get; set; }
        public string WebLinkURL { get; set; }
        public string MediaSource { get; set; }
        public Nullable<int> CountryId { get; set; }
        public Nullable<int> CategoryId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<int> EntityId { get; set; }
        public Nullable<int> InstitutionTypeId { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public Nullable<System.DateTime> Modified { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public bool IsHardCoded { get; set; }
        public bool AllHtmlLinksFetch { get; set; }
        public string RegEx { get; set; }
    
        public virtual category category { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<contentloaderlog> contentloaderlog { get; set; }
        public virtual country country { get; set; }
        public virtual institution institution { get; set; }
        public virtual institutiontypes institutiontypes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<loadererrorlog> loadererrorlog { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<loaderlinkdocuments> loaderlinkdocuments { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<loaderlinklog> loaderlinklog { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<loaderlinkqueue> loaderlinkqueue { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<scanninglinkqueue> scanninglinkqueue { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<scraperenginesetup> scraperenginesetup { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<weblinkactivitytype> weblinkactivitytype { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<weblinkpagecontentregex> weblinkpagecontentregex { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<weblinkpolicymaker> weblinkpolicymaker { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<weblinkproprietarytags> weblinkproprietarytags { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<weblinkrss> weblinkrss { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<weblinksector> weblinksector { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<weburlcategorymapping> weburlcategorymapping { get; set; }
        public virtual websitetypes websitetypes { get; set; }
    }
}