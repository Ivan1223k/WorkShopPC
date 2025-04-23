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
    /// Логика взаимодействия для PaymentsPage.xaml
    /// </summary>
    public partial class PaymentsPage : Page
    {
        public PaymentsPage()
        {
            InitializeComponent();
            DataGridPayments.ItemsSource = Entities.GetContext().Payments.ToList();
        }

        private void UpdatePayments()
        {

            var currentPayment = Entities.GetContext().Payments.ToList();

            if (SortOrdersCategory.SelectedIndex == 0) DataGridPayments.ItemsSource = currentPayment.Where(x =>
        x.PaymentMethod.ToLower().Contains(SortOrdersCategory.Text.ToLower())).ToList();
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            UpdatePayments();
        }

        private void CleanFilter_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SortOrdersCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
