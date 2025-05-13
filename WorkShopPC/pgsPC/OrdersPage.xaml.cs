using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;
using System.Data.Entity;
using System.Data;
using WorkShopPC.wndPC;

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
            SortOrdersCategory.ItemsSource = Entities.GetContext().Status.ToList();
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            var selectedOrder = (sender as Button).DataContext as Orders;

            // Получаем текущий Window и проверяем, является ли он AdminWindow
            var window = Window.GetWindow(this) as AdminWindow;

            if (window != null && window.user != null)
            {
                int employeeId = window.user.ID;
                NavigationService.Navigate(new NewOrder(selectedOrder, employeeId));
            }
            else
            {
                MessageBox.Show("Не удалось определить текущего сотрудника.");
            }
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
                        // Получаем все связанные данные
                        var works = context.CompletedWorks.Where(w => w.OrderID == order.ID).ToList();
                        var usedParts = context.UsedParts.Where(u => u.OrderID == order.ID).ToList();
                        var payments = context.Payments.Where(p => p.OrderID == order.ID).ToList();

                        foreach (var work in works)
                        {
                            context.Entry(work).State = EntityState.Deleted;
                        }
                        foreach (var part in usedParts)
                        {
                            context.Entry(part).State = EntityState.Deleted;
                        }
                        foreach (var payment in payments)
                        {
                            context.Entry(payment).State = EntityState.Deleted;
                        }
                        context.Orders.Remove(order);
                    }

                    context.SaveChanges();
                    MessageBox.Show("Заказы и связанные данные успешно удалены!");

                    // Обновляем грид
                    DataGridOrders.ItemsSource = context.Orders.ToList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при удалении: " + ex.Message + "\n\nДетали: " + ex.InnerException?.Message);
                }
            }
        }

        private void UpdateOrders()
        {
            var context = Entities.GetContext();

            var currentOrders = context.Orders
                .Include(o => o.Status)
                .Include(o => o.Clients)
                .Include(o => o.Devices)
                .ToList();

            
            if (!string.IsNullOrWhiteSpace(SearchOrdersName.Text))
            {
                currentOrders = currentOrders
                    .Where(x => x.Clients.FirstName
                        .ToLower()
                        .Contains(SearchOrdersName.Text.ToLower()))
                    .ToList();
            }

            // Фильтрация по статусу
            if (SortOrdersCategory.SelectedItem is Status selectedStatus)
            {
                currentOrders = currentOrders
                    .Where(x => x.StatusID == selectedStatus.ID)
                    .ToList();
            }

            DataGridOrders.ItemsSource = currentOrders;
        }

        private void SearchOrdersName_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateOrders();
        }

        private void SortOrdersCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateOrders();
        }

        private void CleanFilter_Click(object sender, RoutedEventArgs e)
        {
            SearchOrdersName.Text = "";
            SortOrdersCategory.SelectedItem = null;
            UpdateOrders();
        }
    }
}
