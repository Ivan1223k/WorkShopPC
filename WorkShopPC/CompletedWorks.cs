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
    
    public partial class CompletedWorks
    {
        public int ID { get; set; }
        public int OrderID { get; set; }
        public int WorkID { get; set; }
        public int EmployeeID { get; set; }
        public Nullable<System.DateTime> CompletionDate { get; set; }
    
        public virtual Employees Employees { get; set; }
        public virtual Orders Orders { get; set; }
        public virtual Works Works { get; set; }
    }
}
