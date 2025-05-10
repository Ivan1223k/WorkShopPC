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
using WorkShopPC.pgsPC;

namespace WorkShopPC.wndPC
{
    /// <summary>
    /// Логика взаимодействия для UserWindow.xaml
    /// </summary>
    public partial class UserWindow : Window
    {
        public Employees user;

        public UserWindow(Employees user)
        {
            InitializeComponent();
            this.user = user;
            
        }


        private void NewOrder_Click(object sender, RoutedEventArgs e)
        {
            AdminFrame.NavigationService.Navigate(new NewOrder(null));
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            AdminFrame.NavigationService.Navigate(new Profile(user));
        }

        private void OrdersButton_Click(object sender, RoutedEventArgs e)
        {
            AdminFrame.NavigationService.Navigate(new OrdersPage());
        }

        private void PrtsButton_Click(object sender, RoutedEventArgs e)
        {
            AdminFrame.NavigationService.Navigate(new PartsPage());
        }

        private void ClientsButton_Click(object sender, RoutedEventArgs e)
        {
            AdminFrame.NavigationService.Navigate(new ClientsPage());
        }
    }
}
