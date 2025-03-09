using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace WorkShopPC.wndPC
{
    /// <summary>
    /// Логика взаимодействия для UserWindow.xaml
    /// </summary>
    public partial class UserWindow : Window
    {
        public Employees user;

        public UserWindow()
        {
            InitializeComponent();
            DataGridOrders.ItemsSource = Entities.GetContext().Orders.ToList();
        }
        public UserWindow(Employees user)
        {
            this.user = user;
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
