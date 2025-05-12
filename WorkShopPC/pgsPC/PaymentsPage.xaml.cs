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

            SortPaymentCategory.ItemsSource =Entities.GetContext().PaymentMethods.ToList();
            DataGridPayments.ItemsSource = Entities.GetContext().Payments.ToList();
        }

        private void UpdatePayments()
        {
            var context = Entities.GetContext();

            // Загружаем все платежи с включением связанных данных (если надо)
            var currentPayments = context.Payments
                .ToList();

            // Фильтрация по способу оплаты
            if (SortPaymentCategory.SelectedItem is PaymentMethods selectedMethod)
            {
                currentPayments = currentPayments
                    .Where(p => p.PaymentMethodID == selectedMethod.ID)
                    .ToList();
            }

            DataGridPayments.ItemsSource = currentPayments;
        }


        private void CleanFilter_Click(object sender, RoutedEventArgs e)
        {
            SortPaymentCategory.SelectedItem = null;

            var context = Entities.GetContext();
            DataGridPayments.ItemsSource = context.Payments.ToList();
        }

        private void SortOrdersCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdatePayments();
        }
    }
}
