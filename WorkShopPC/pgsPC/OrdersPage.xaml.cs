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
    /// Логика взаимодействия для OrdersPage.xaml
    /// </summary>
    public partial class OrdersPage : Page
    {
        public OrdersPage()
        {
            InitializeComponent();
            
            DataGridOrders.ItemsSource = Entities.GetContext().Orders.ToList();
     
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new NewOrder((sender as Button).DataContext as Orders));
        }

        private void DeleteOrderButton_Click(object sender, RoutedEventArgs e)
        {
            var ordersForRemoving = DataGridOrders.SelectedItems.Cast<Orders>().ToList();

            if (ordersForRemoving.Count == 0)
            {
                MessageBox.Show("Выберите заказы для удаления!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show($"Вы точно хотите удалить {ordersForRemoving.Count} записей?",
                "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    var context = Entities.GetContext();

                    
                    foreach (var order in ordersForRemoving)
                    {
                        context.Orders.Remove(order);
                    }

                    context.SaveChanges();

                    MessageBox.Show("Данные успешно удалены!");

                    
                    DataGridOrders.ItemsSource = context.Orders.ToList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при удалении: " + ex.Message);
                }
            }
        }
    }
}
