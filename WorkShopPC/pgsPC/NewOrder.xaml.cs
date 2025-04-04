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
using WorkShopPC.wndPC;

namespace WorkShopPC.pgsPC
{
    /// <summary>
    /// Логика взаимодействия для NewOrder.xaml
    /// </summary>
    public partial class NewOrder : Page
    {
        public NewOrder(Orders orders)
        {
            InitializeComponent();

        }





        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            Clients _clients = new Clients
            {
                FirstName = FirstNameBox.Text,
                LastName = SecondNameBox.Text,
                PhoneNumber = PhoneBox.Text,
                Email = EmailBox.Text,
                Address = AddressBox.Text,
            };

            Devices _devices = new Devices
            {
                DeviceType = DeviceTypeBox.Text,
                Brand = BrandBox.Text,
                Model = ModelBox.Text,
                SerialNumber = SerialNumberBox.Text
            };

            Orders _orders = new Orders
            {

                OrderDate = DateTime.Parse(OrderDateBox.Text),
                Status = StatusTextBox.Text
            };





            if (string.IsNullOrWhiteSpace(_clients.FirstName)) errors.AppendLine("Введите имя!");
            if (string.IsNullOrWhiteSpace(_clients.LastName)) errors.AppendLine("Введите фамилию!");
            if (string.IsNullOrWhiteSpace(_clients.Email)) errors.AppendLine("Введите почту!");
            if (string.IsNullOrWhiteSpace(_clients.PhoneNumber)) errors.AppendLine("Введите телефон!");
            if (string.IsNullOrWhiteSpace(_clients.Address)) errors.AppendLine("Введите адресс!");
            if (string.IsNullOrWhiteSpace(_devices.DeviceType)) errors.AppendLine("Введите тип девайса!");
            if (string.IsNullOrWhiteSpace(_devices.Brand)) errors.AppendLine("Введите брэнд!");
            if (string.IsNullOrWhiteSpace(_devices.Model)) errors.AppendLine("Введите модель девайса!");
            if (string.IsNullOrWhiteSpace(_devices.SerialNumber)) errors.AppendLine("Введите серийный номер!");
            if (string.IsNullOrWhiteSpace(OrderDateBox.Text)) errors.AppendLine("Введите дату!");
            if (string.IsNullOrWhiteSpace(_orders.Status)) errors.AppendLine("выберите статус!");

            if (errors.Length > 0) { MessageBox.Show(errors.ToString()); return; }

            if (_clients.ID == 0) Entities.GetContext().Clients.Add(_clients);
            if (_devices.ID == 0) Entities.GetContext().Devices.Add(_devices);
            if (_orders.ID == 0) Entities.GetContext().Orders.Add(_orders);


            try
            {
                Entities.GetContext().SaveChanges();
                MessageBox.Show("Данные успешно добавлены!");
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}

