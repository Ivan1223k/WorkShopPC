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
    
    public partial class Payments
    {
        public int ID { get; set; }
        public int OrderID { get; set; }
        public Nullable<System.DateTime> PaymentDate { get; set; }
        public int PaymentMethodID { get; set; }
        public Nullable<decimal> Amount { get; set; }
    
        public virtual Orders Orders { get; set; }
        public virtual PaymentMethods PaymentMethods { get; set; }
    }
}
