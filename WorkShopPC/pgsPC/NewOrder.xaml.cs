using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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


        public Orders _orders = new Orders();

        private bool IsValidMail(string email)
        {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        public NewOrder(Orders orders)
        {
            InitializeComponent();
            StatusComboBox.ItemsSource = Entities.GetContext().Status.ToList();

            PaymentMethodNamebb.ItemsSource = Entities.GetContext().PaymentMethods.ToList();

            if (orders != null && orders.ID != 0)
            {
                // Редактирование: грузим заказ и связанные данные
                _orders = orders;

                // Инициализируем клиентов и устройство
                DataContext = _orders;

                // Подгружаем детали
                var allParts = Entities.GetContext().Parts.ToList();
                var existingUsedParts = Entities.GetContext().UsedParts
                    .Where(up => up.OrderID == _orders.ID)
                    .ToList();

                var combinedUsedParts = allParts.Select(p =>
                {
                    var existing = existingUsedParts.FirstOrDefault(up => up.PartID == p.ID);
                    return new UsedParts
                    {
                        ID = existing?.ID ?? 0,
                        PartID = p.ID,
                        Parts = p,
                        Quantity = existing?.Quantity ?? 0,
                        OrderID = _orders.ID
                    };
                }).ToList();

                PartsList.ItemsSource = combinedUsedParts;

                // --- Работы ---
                var allWorks = Entities.GetContext().CompletedWorks.ToList();
                var orderWorks = allWorks.Where(w => w.OrderID == _orders.ID).ToList();

                // Формируем список WorkViewModel:
                // - Все работы доступны
                // - Те, что привязаны к заказу — помечены как IsSelected=true
                var workViewModels = allWorks.Select(w => new WorkViewModel(
                    w,
                    isSelected: orderWorks.Contains(w)
                )).ToList();

                WorkList.ItemsSource = workViewModels;
            }
            else
            {
                // Новый заказ: создаём пустой список
                var allParts = Entities.GetContext().Parts.ToList();
                var usedParts = allParts.Select(p => new UsedParts
                {
                    PartID = p.ID,
                    Parts = p,
                    Quantity = 0,
                    OrderID = 0
                }).ToList();

                PartsList.ItemsSource = usedParts;

                // Все работы, ни одна не выбрана
                var allWorks = Entities.GetContext().CompletedWorks.ToList();
                var workViewModels = allWorks.Select(w => new WorkViewModel(w)).ToList();
                WorkList.ItemsSource = workViewModels;
            }

            if (orders != null) _orders = orders;
            DataContext = _orders;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            Status selectedStatus = StatusComboBox.SelectedItem as Status;

            PaymentMethods selectedPaymentMethods = PaymentMethodNamebb.SelectedItem as PaymentMethods;

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

            Payments _payments = new Payments
            {
                Amount = Convert.ToDecimal(TotalAmountText.Text),
                PaymentMethods = selectedPaymentMethods,


                PaymentDate = CompletionDate.SelectedDate.Value,
            };


            var regex = new Regex(@"^8\(\d{3}\)\d{3}-\d{2}-\d{2}$");

            if (string.IsNullOrWhiteSpace(_clients.FirstName)) errors.AppendLine("Введите имя!");
            if (string.IsNullOrWhiteSpace(_clients.LastName)) errors.AppendLine("Введите фамилию!");
            if (string.IsNullOrWhiteSpace(_clients.Email)) errors.AppendLine("Введите почту!");
            if (!IsValidMail(EmailBox.Text))errors.AppendLine("Введите корректный email");
            if (string.IsNullOrWhiteSpace(_clients.PhoneNumber)) errors.AppendLine("Введите телефон!");
            if (!regex.IsMatch(PhoneBox.Text)) errors.AppendLine("Укажите номер телефона в формате 8(XXX)XXX-XX-XX");
            if (string.IsNullOrWhiteSpace(_clients.Address)) errors.AppendLine("Введите адресс!");
            if (string.IsNullOrWhiteSpace(_devices.DeviceType)) errors.AppendLine("Введите тип девайса!");
            if (string.IsNullOrWhiteSpace(_devices.Brand)) errors.AppendLine("Введите брэнд!");
            if (string.IsNullOrWhiteSpace(_devices.Model)) errors.AppendLine("Введите модель девайса!");
            if (string.IsNullOrWhiteSpace(_devices.SerialNumber)) errors.AppendLine("Введите серийный номер!");
            if (!CompletionDate.SelectedDate.HasValue){MessageBox.Show("Выберите дату платежа!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);return;}
            if (selectedStatus == null){errors.AppendLine("Выберите статус!");}
            if (selectedPaymentMethods == null){errors.AppendLine("Выберите способ оплаты!");}

            if (errors.Length > 0) { MessageBox.Show(errors.ToString()); return; }

            
            decimal total = (PartsList.ItemsSource as List<UsedParts>)?
                .Sum(p => (p.Parts?.Price ?? 0) * p.Quantity) ?? 0;

            // Получаем список выбранных работ
            var selectedWorks = (WorkList.ItemsSource as IEnumerable<WorkViewModel>)
                ?.Where(vm => vm.IsSelected)
                .Select(vm => vm.CompletedWork)
                .ToList();

            // Удаляем старые записи для этого заказа
            var oldWorks = Entities.GetContext().CompletedWorks
                .Where(w => w.OrderID == _orders.ID)
                .ToList();

            foreach (var work in oldWorks)
            {
                Entities.GetContext().CompletedWorks.Remove(work);
            }

            // Добавляем новые
            foreach (var work in selectedWorks)
            {
                work.OrderID = _orders.ID;
            }

            Entities.GetContext().SaveChanges();


            if (!OrderDateBox.SelectedDate.HasValue){errors.AppendLine("Введите дату!");}
            else
            {
                _orders.OrderDate = OrderDateBox.SelectedDate.Value;
            }
            _orders.Clients = _clients;
            _orders.Devices = _devices;
            _orders.Status = selectedStatus;
            _orders.TotalCost = total;

            UpdateTotal();




            if (_clients.ID == 0) Entities.GetContext().Clients.Add(_clients);
            if (_devices.ID == 0) Entities.GetContext().Devices.Add(_devices);
            if (_orders.ID == 0) Entities.GetContext().Orders.Add(_orders);
            if (_orders.ID == 0) Entities.GetContext().Payments.Add(_payments);




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

        private void UpdateTotal()
        {
            // Стоимость деталей
            var usedParts = PartsList.ItemsSource as List<UsedParts>;
            decimal partsTotal = usedParts?.Sum(p => (p.Parts?.Price ?? 0) * p.Quantity) ?? 0;

            // Стоимость работ
            var worksList = WorkList.ItemsSource as IEnumerable<WorkViewModel>;
            decimal worksTotal = worksList?
                .Where(w => w.IsSelected)
                .Sum(w => w.CompletedWork.Works?.Price ?? 0) ?? 0;

            // Общая сумма
            decimal total = partsTotal + worksTotal;
            TotalAmountText.Text = total.ToString("C");

            // Привязываем к заказу
            _orders.TotalCost = total;
        }

        private void IncreaseCount_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is UsedParts part)
            {
                part.Quantity++;
                PartsList.Items.Refresh();
                UpdateTotal();
            }
        }

        private void WorkCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            UpdateTotal();
        }

        private void DecreaseCount_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is UsedParts part)
            {
                if (part.Quantity > 0)
                {
                    part.Quantity--;
                    PartsList.Items.Refresh();
                    UpdateTotal();
                }
            }
        }
    }
}

