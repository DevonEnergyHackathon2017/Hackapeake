//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Overwatch.Shared
{
    using System;
    using System.Collections.Generic;
    
    public partial class PropertyAlertStatu
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PropertyAlertStatu()
        {
            this.PropertyAlerts = new HashSet<PropertyAlert>();
        }
    
        public int id { get; set; }
        public string propertyAlertStatus { get; set; }
        public int severityId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PropertyAlert> PropertyAlerts { get; set; }
        public virtual Severity Severity { get; set; }
    }
}