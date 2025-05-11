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
                if (user == null)
                {
                    MessageBox.Show("Пользователь с такими данными не найден!");
                    return;
                }
                if (user.RoleID == 1)
                {
                    var newWindow = new AdminWindow(user);
                    newWindow.Show();
                    this.Close();
                }
            else
                {
                    var newWindow = new UserWindow(user);
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
    

