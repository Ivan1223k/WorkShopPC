using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WorkShopPC.pgsPC
{
    /// <summary>
    /// Логика взаимодействия для Profile.xaml
    /// </summary>
    public partial class Profile : Page
    {
        public Profile(Employees user)
        {
            InitializeComponent();
            var fullName = $"{user.LastName} {user.FirstName}";

            this.DataContext = new
            {
                FullName = fullName,
                Phone = user.PhoneNumber,
                Login = user.Email,
                Role = GetRoleName(user.RoleID)
            };
        }

        private string GetRoleName(int? roleId)
        {
            switch (roleId)
            {
                case 1:
                    return "Администратор";
                case 2:
                    return "Специалист";
                default:
                    return "Неизвестная роль";
            }
        }
    }

}
