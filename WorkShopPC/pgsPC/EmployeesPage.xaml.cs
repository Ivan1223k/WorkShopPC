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
    /// Логика взаимодействия для EmployeesPage.xaml
    /// </summary>
    public partial class EmployeesPage : Page
    {
        public EmployeesPage()
        {
            InitializeComponent();
            DataGridEmployees.ItemsSource = Entities.GetContext().Employees.ToList();
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new InformationEmployees((sender as Button).DataContext as Employees));
        }

        private void UpdateEmpls()
        {
            var currentOrder = Entities.GetContext().Employees.ToList();
            DataGridEmployees.ItemsSource = currentOrder.Where(x =>
        (x.FirstName + " " + x.LastName).ToLower().Contains(SearchEmplName.Text.ToLower())).ToList();

        }

        private void SearchEmplName_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateEmpls();
        }
    }
}
