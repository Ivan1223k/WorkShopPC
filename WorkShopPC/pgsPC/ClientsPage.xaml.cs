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
            var currentClients = Entities.GetContext().Clients.ToList();
            DataGridClients.ItemsSource = currentClients.Where(x =>
        (x.FirstName + " " + x.LastName).ToLower().Contains(SearchOrdersName.Text.ToLower())).ToList();

        }

        private void SearchOrdersName_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateClients();
        }

        private void ButtonCheck_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new InfotamatinClients((sender as Button).DataContext as Clients));
        }

        private void DeleteOrderButton_Click(object sender, RoutedEventArgs e)
        {
            var ClientsForRemoving = DataGridClients.SelectedItems.Cast<Clients>().ToList();

            if (ClientsForRemoving.Count == 0)
            {
                MessageBox.Show("Выберите заказы для удаления!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show($"Вы точно хотите удалить {ClientsForRemoving.Count} записей?",
                "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    var context = Entities.GetContext();


                    foreach (var client in ClientsForRemoving)
                    {
                        context.Clients.Remove(client);
                    }

                    context.SaveChanges();

                    MessageBox.Show("Данные успешно удалены!");


                    DataGridClients.ItemsSource = context.Clients.ToList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при удалении: " + ex.Message);
                }
            }
        }
    }
}
