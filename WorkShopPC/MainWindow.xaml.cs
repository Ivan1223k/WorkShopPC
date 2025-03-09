using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WorkShopPC.wndPC;
using static System.Net.Mime.MediaTypeNames;

namespace WorkShopPC
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //private bool isValidMail(string email)
        //{
        //    return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        //}

        //private void signUpButton_Click(object sender, RoutedEventArgs e)
        //{
        //    if (loginText.Text.Length > 8)
        //    {
        //        using (var db = new Entities())
        //        {
        //            var user = db.Employees.AsNoTracking().FirstOrDefault(em => em.Email == loginText.Text);

        //            if (user != null)
        //            {
        //                MessageBox.Show("Пользователь с такими данными уже существует!");
        //                return;
        //            }
        //        }

        //        bool en = true;
        //        bool number = false;

        //        for (int i = 0; i < passwordText.Password.Length; i++)
        //        {
        //            if (passwordText.Password[i] >= 'A' && passwordText.Password[i] <= 'Z') en = false;
        //            if (passwordText.Password[i] >= '0' && passwordText.Password[i] <= '9') number = true;
        //        }

        //        var regex = new Regex(@"^8\(\d{3}\)\d{3}-\d{2}-\d{2}$");
        //        StringBuilder errors = new StringBuilder();

        //        if (passwordText.Password.Length < 6) errors.AppendLine("Пароль должен быть больше 6 символов");
        //        if (!regex.IsMatch(phoneText.Text)) errors.AppendLine("Укажите номер телефона в формате 8(XXX)XXX-XX-XX");
        //        if (!en) errors.AppendLine("Пароль должен быть на английском языке");
        //        if (!number) errors.AppendLine("Пароль должен содержать хотя бы одну цифру");
        //        if (!isValidMail(loginText.Text)) errors.AppendLine("Введите корректный email");

        //        if (errors.Length > 0)
        //        {
        //            MessageBox.Show(errors.ToString());
        //            return;
        //        }
        //        else

        //        {
        //            string selectedPosition = (positionComboBox.SelectedItem as ComboBoxItem).Content.ToString();
        //            Entities db = new Entities();
        //            Employees userObject = new Employees
        //            {
        //                FirstName = nameText.Text,
        //                LastName = surnameText.Text,
        //                Email = loginText.Text,
        //                Password = GetHash(passwordText.Password),
        //                Position = selectedPosition,
        //                RoleID = 1,
        //                PhoneNumber = phoneText.Text,
        //            };

        //            db.Employees.Add(userObject);
        //            db.SaveChanges();
        //            MessageBox.Show("Вы успешно зарегистрировались!", "Успешно!", MessageBoxButton.OK);
        //            //NavigationService.Navigate(new Uri("/Pages/AuthLog.xaml", UriKind.Relative));
        //        }
        //    }

        //    else MessageBox.Show("Укажите лоигн!");

        //}


        public static string GetHash(string password)
        {
            using (var hash = SHA1.Create())
            {
                return string.Concat(hash.ComputeHash(Encoding.UTF8.GetBytes(password)).Select(x => x.ToString("X2")));
            }
        }

        private void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(loginTextBox.Text) || string.IsNullOrEmpty(passwordTextBox.Password))
            {
                MessageBox.Show("Введите логин и пароль!");
                return;
            }

            string _password = GetHash(passwordTextBox.Password);
            using (var db = new Entities())
            {
                var user = db.Employees.AsNoTracking().FirstOrDefault(em => em.Email == loginTextBox.Text && em.Password == _password);
                if (user == null) { MessageBox.Show("Пользователь с такими данными не найден!"); return; }
                if (user.RoleID == 1)
                {
                    var newWindow = new UserWindow(user);
                    newWindow.Show();
                    this.Close();
                }
                else
                {
                    var newWindow = new AdminWindow(user);
                    newWindow.Show();
                    this.Close();
                }
            }
        }

        private void SignUpWindowButton_Click(object sender, RoutedEventArgs e)
        {
            var newWindow = new SignUpWindow();
            newWindow.Show();
            this.Close();
        }
    }
}
    

