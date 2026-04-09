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

namespace Presentation
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void PrijavaClick(object sender, RoutedEventArgs e)
        {
            // TO-DO PRIJAVA
        }
        private void RegistracijaClick(object sender, RoutedEventArgs e)
        {
            // TO-DO REGISTRACIJA
        }
    }
}
