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
using System.Windows.Shapes;

namespace WorkShopPC.pgsPC
{
    /// <summary>
    /// Логика взаимодействия для ClientsPage.xaml
    /// </summary>
    public partial class ClientsPage : Page
    {
        public ClientsPage()
        {
            InitializeComponent();
            DataGridClients.ItemsSource = Entities.GetContext().Clients.ToList();
        }

        private void UpdateClients()
        {
            var currentOrder = Entities.GetContext().Clients.ToList();
            currentOrder = currentOrder.Where(x =>
        x.FirstName.ToLower().Contains(SearchOrdersName.Text.ToLower())).ToList();

        }


        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SearchOrdersName_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateClients();
        }
    }
}
