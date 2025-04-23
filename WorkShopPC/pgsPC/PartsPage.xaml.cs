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

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void UpdateParts()
        {
            var currentOrder = Entities.GetContext().Parts.ToList();
            currentOrder = currentOrder.Where(x =>
        x.PartName.ToLower().Contains(SearchPartsName.Text.ToLower())).ToList();

        }

        private void SearchPartsName_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateParts();
        }
    }
}
