//------------------------------------------------------------------------------
// <auto-generated>
//    Этот код был создан из шаблона.
//
//    Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//    Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WorkShopPC
{
    using System;
    using System.Collections.Generic;
    
    public partial class Works
    {
        public Works()
        {
            this.CompletedWorks = new HashSet<CompletedWorks>();
        }
    
        public int ID { get; set; }
        public string WorkName { get; set; }
        public string Description { get; set; }
        public Nullable<decimal> Price { get; set; }
    
        public virtual ICollection<CompletedWorks> CompletedWorks { get; set; }
    }
}
