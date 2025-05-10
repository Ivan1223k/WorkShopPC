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
using System.Windows.Navigation;
using WorkShopPC.pgsPC;
using System.Windows.Shapes;

namespace WorkShopPC.wndPC
{
    /// <summary>
    /// Логика взаимодействия для AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        public Employees user;

        public AdminWindow(Employees user)
        {
            InitializeComponent();
            this.user = user;

        }

        private void PrtsButton_Click(object sender, RoutedEventArgs e)
        {
            AdminFrame.NavigationService.Navigate(new PartsPage());
        }

        private void ClientsButton_Click(object sender, RoutedEventArgs e)
        {
            AdminFrame.NavigationService.Navigate(new ClientsPage());
        }

        private void OrdersButton_Click(object sender, RoutedEventArgs e)
        {
            AdminFrame.NavigationService.Navigate(new OrdersPage());
        }

        private void PaymentsButton_Click(object sender, RoutedEventArgs e)
        {
            AdminFrame.NavigationService.Navigate(new PaymentsPage());
        }

        private void EmployeesButton_Click(object sender, RoutedEventArgs e)
        {
            AdminFrame.NavigationService.Navigate(new EmployeesPage());
        }

        private void NewOrder_Click(object sender, RoutedEventArgs e)
        {
            AdminFrame.NavigationService.Navigate(new NewOrder(null));
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {

            AdminFrame.NavigationService.Navigate(new Profile(user));

        }

        private void ReportButton_Click(object sender, RoutedEventArgs e)
        {
            var newWindow = new Report();
            newWindow.Show();
        }
    }
}
