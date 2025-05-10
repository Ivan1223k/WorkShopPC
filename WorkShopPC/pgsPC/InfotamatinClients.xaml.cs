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
    /// Логика взаимодействия для InfotamatinClients.xaml
    /// </summary>
    public partial class InfotamatinClients : Page
    {

        private Clients _clients = new Clients();
        public InfotamatinClients(Clients clients)
        {
            InitializeComponent();

            if (clients != null) _clients = clients;
            DataContext = _clients;
        }
    }
}
