using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
using System.Windows.Shapes;

namespace WorkShopPC.wndPC
{
    /// <summary>
    /// Логика взаимодействия для SignUpWindow.xaml
    /// </summary>
    public partial class SignUpWindow : Window
    {
        public SignUpWindow()
        {
            InitializeComponent();
        }

        private bool IsValidMail(string email)
        {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        public static string GetHash(string password)
        {
            using (var hash = SHA1.Create())
            {
                return string.Concat(hash.ComputeHash(Encoding.UTF8.GetBytes(password)).Select(x => x.ToString("X2")));
            }
        }

        private void SignUpButton_Click(object sender, RoutedEventArgs e)
        {
            if (loginText.Text.Length > 8)
            {
                using (var db = new Entities())
                {
                    var user = db.Employees.AsNoTracking().FirstOrDefault(em => em.Email == loginText.Text);

                    if (user != null)
                    {
                        MessageBox.Show("Пользователь с такими данными уже существует!");
                        return;
                    }
                }

                bool en = true;
                bool number = false;

                for (int i = 0; i < passwordText.Password.Length; i++)
                {
                    if (passwordText.Password[i] >= 'A' && passwordText.Password[i] <= 'Z') en = false;
                    if (passwordText.Password[i] >= '0' && passwordText.Password[i] <= '9') number = true;
                }

                var regex = new Regex(@"^8\(\d{3}\)\d{3}-\d{2}-\d{2}$");
                StringBuilder errors = new StringBuilder();

                if (passwordText.Password.Length < 6) errors.AppendLine("Пароль должен быть больше 6 символов");
                if (!regex.IsMatch(phoneText.Text)) errors.AppendLine("Укажите номер телефона в формате 8(XXX)XXX-XX-XX");
                if (!en) errors.AppendLine("Пароль должен быть на английском языке");
                if (!number) errors.AppendLine("Пароль должен содержать хотя бы одну цифру");
                if (!IsValidMail(loginText.Text)) errors.AppendLine("Введите корректный email");

                if (errors.Length > 0)
                {
                    MessageBox.Show(errors.ToString());
                    return;
                }
                else

                {
                    Entities db = new Entities();
                    Employees userObject = new Employees
                    {
                        FirstName = nameText.Text,
                        LastName = surnameText.Text,
                        Email = loginText.Text,
                        Password = GetHash(passwordText.Password),
                        RoleID = 2,
                        PhoneNumber = phoneText.Text,
                    };

                    db.Employees.Add(userObject);
                    db.SaveChanges();
                    MessageBox.Show("Вы успешно зарегистрировались!", "Успешно!", MessageBoxButton.OK);
                }
            }

            else MessageBox.Show("Укажите лоигн!");

        }

        private void SignInWindow_Click(object sender, RoutedEventArgs e)
        {
            var newWindow = new MainWindow();
            newWindow.Show();
            this.Close();

        }
    }
}
