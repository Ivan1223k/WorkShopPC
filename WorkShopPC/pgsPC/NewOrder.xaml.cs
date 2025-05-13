using System;
using System.Collections.Generic;
using System.Data;
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
            PaymentMethods selectedPaymentMethod = PaymentMethodNamebb.SelectedItem as PaymentMethods;

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
                PaymentMethods = selectedPaymentMethod,
                PaymentDate = CompletionDate.SelectedDate.Value
            };

            var regex = new Regex(@"^8\(\d{3}\)\d{3}-\d{2}-\d{2}$");

            if (string.IsNullOrWhiteSpace(_clients.FirstName)) errors.AppendLine("Введите имя!");
            if (string.IsNullOrWhiteSpace(_clients.LastName)) errors.AppendLine("Введите фамилию!");
            if (string.IsNullOrWhiteSpace(_clients.Email)) errors.AppendLine("Введите почту!");
            if (!IsValidMail(EmailBox.Text)) errors.AppendLine("Введите корректный email");
            if (string.IsNullOrWhiteSpace(_clients.PhoneNumber)) errors.AppendLine("Введите телефон!");
            if (!regex.IsMatch(PhoneBox.Text)) errors.AppendLine("Укажите номер телефона в формате 8(XXX)XXX-XX-XX");
            if (string.IsNullOrWhiteSpace(_clients.Address)) errors.AppendLine("Введите адрес!");
            if (string.IsNullOrWhiteSpace(_devices.DeviceType)) errors.AppendLine("Введите тип девайса!");
            if (string.IsNullOrWhiteSpace(_devices.Brand)) errors.AppendLine("Введите бренд!");
            if (string.IsNullOrWhiteSpace(_devices.Model)) errors.AppendLine("Введите модель девайса!");
            if (string.IsNullOrWhiteSpace(_devices.SerialNumber)) errors.AppendLine("Введите серийный номер!");
            if (!CompletionDate.SelectedDate.HasValue) { MessageBox.Show("Выберите дату платежа!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
            if (selectedStatus == null) errors.AppendLine("Выберите статус!");
            if (selectedPaymentMethod == null) errors.AppendLine("Выберите способ оплаты!");
            if (TotalAmountText == null) errors.AppendLine("Добавьте услугу или запчасть в заказ!");

            if (errors.Length > 0) { MessageBox.Show(errors.ToString()); return; }

            // Считаем стоимость деталей
            decimal total = (PartsList.ItemsSource as List<UsedParts>)?
                .Sum(p => (p.Parts?.Price ?? 0) * p.Quantity) ?? 0;

            _orders.OrderDate = OrderDateBox.SelectedDate ?? DateTime.Now;
            _payments.PaymentDate = CompletionDate.SelectedDate.Value;

            _orders.Clients = _clients;
            _orders.Devices = _devices;
            _orders.Status = selectedStatus;
            _orders.TotalCost = total;
            _orders.Payments = new List<Payments> { _payments };

            UpdateTotal();

            // Добавляем клиента, устройство и заказ, если они новые
            if (_clients.ID == 0) Entities.GetContext().Clients.Add(_clients);
            if (_devices.ID == 0) Entities.GetContext().Devices.Add(_devices);
            if (_orders.ID == 0) Entities.GetContext().Orders.Add(_orders);


            _payments.OrderID = _orders.ID;
            Entities.GetContext().Payments.Add(_payments);
            
            Entities.GetContext().SaveChanges();

            // Удаляем старые работы по заказу
            var oldWorks = Entities.GetContext().CompletedWorks
                .Where(w => w.OrderID == _orders.ID)
                .ToList();

            if (oldWorks.Any())
            {
                foreach (var work in oldWorks)
                {
                    // Если вы работаете с DbContext, но в старой версии EF:
                    Entities.GetContext().CompletedWorks.Attach(work);
                    Entities.GetContext().CompletedWorks.Remove(work);
                }

                try
                {
                    Entities.GetContext().SaveChanges();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Работы для удаления не найдены.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            // Добавляем выбранные работы
            var selectedWorks = (WorkList.ItemsSource as IEnumerable<WorkViewModel>)
                ?.Where(vm => vm.IsSelected)
                .Select(vm => new CompletedWorks
                {
                    OrderID = _orders.ID,
                    WorkID = vm.CompletedWork.WorkID
                })
                .ToList();

            if (selectedWorks != null)
            {
                foreach (var work in selectedWorks)
                {
                    Entities.GetContext().CompletedWorks.Add(work);
                }
            }

            try
            {
                Entities.GetContext().SaveChanges();
                MessageBox.Show("Данные успешно сохранены!");
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Ошибка при сохранении", MessageBoxButton.OK, MessageBoxImage.Error);
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
            TotalAmountText.Text = total.ToString();

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

