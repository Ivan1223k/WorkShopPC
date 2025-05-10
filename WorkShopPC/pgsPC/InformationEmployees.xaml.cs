using System;
using System.Collections.Generic;
using System.Data;
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
    /// Логика взаимодействия для InformationEmployees.xaml
    /// </summary>
    public partial class InformationEmployees : Page
    {

        private Employees _employees = new Employees();

        public InformationEmployees(Employees employees)
        {
            InitializeComponent();

            SelectedRoleComboBox.ItemsSource = Entities.GetContext().Roles.ToList();

            if (employees != null) _employees = employees;
            DataContext = _employees;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            // Получаем выбранную роль
            Roles selectedRole = SelectedRoleComboBox.SelectedItem as Roles;

            // Проверка, что роль выбрана
            if (selectedRole == null)
            {
                errors.AppendLine("Выберите роль!");
            }

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            // Если это редактирование заказа, обновляем роль
            _employees.Roles = selectedRole;

            // Сохраняем изменения в базе
            try
            {
                Entities.GetContext().SaveChanges();
                MessageBox.Show("Роль успешно обновлена!");
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
