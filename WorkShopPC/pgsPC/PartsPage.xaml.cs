using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
using Microsoft.Win32;

namespace WorkShopPC.pgsPC
{
    /// <summary>
    /// Логика взаимодействия для PartsPage.xaml
    /// </summary>
    public partial class PartsPage : Page
    {
        public PartsPage()
        {
            InitializeComponent();

            var currentParts = Entities.GetContext().Parts.ToList();
            ListParts.ItemsSource = currentParts;


        }

        private void UpdateParts()
        {
            var currentOrder = Entities.GetContext().Parts.ToList();
            ListParts.ItemsSource = currentOrder.Where(x =>
        x.PartName.ToLower().Contains(SearchPartsName.Text.ToLower())).ToList();

        }

        private void SearchPartsName_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateParts();
        }


        private void AddImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;

                // Загрузка изображения в byte[]
                byte[] imageData;
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    imageData = new byte[fs.Length];
                    fs.Read(imageData, 0, imageData.Length);
                }

                // Получаем выделенный элемент
                var selectedPart = ListParts.SelectedItem as Parts;
                if (selectedPart != null)
                {
                    selectedPart.PicturePart = imageData;

                    // Сохраняем изменения в БД
                    Entities.GetContext().SaveChanges();

                    MessageBox.Show("Картинка добавлена!");
                }
                else
                {
                    MessageBox.Show("Выберите деталь из списка.");
                }
            }

        }
    }
}
