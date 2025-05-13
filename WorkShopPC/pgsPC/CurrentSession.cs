using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkShopPC.pgsPC
{
    internal class CurrentSession
    {
        // Свойство для хранения ID авторизованного сотрудника
        public static int EmployeeID { get; set; }

        // Опционально: можно добавить и другие данные пользователя
        public static string EmployeeName { get; set; }
    }
}
