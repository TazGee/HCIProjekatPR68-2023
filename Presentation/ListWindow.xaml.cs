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
using System.Windows.Shapes;
using Database.DataBase;
using Database.Repositories;
using Domain.Database;
using Domain.Models;
using Domain.Repositories;
using Domain.Services;
using Services.AuthService;

namespace Presentation
{
    public partial class ListWindow : Window
    {
        // Database and Repos
        static IDataBase vulcansDatabase = new VulcansXMLDataBase();

        // Korisnik
        User korisnik = new User();

        public ListWindow(User user)
        {
            korisnik = user;
            InitializeComponent();

            UsernameButton.Content = korisnik.Username;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
    }
}
