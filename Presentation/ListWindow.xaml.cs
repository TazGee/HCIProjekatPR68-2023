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

        // Auth prozor
        MainWindow authWindow;

        public ListWindow(User user, MainWindow authWindow)
        {
            korisnik = user;
            InitializeComponent();

            UsernameButton.Content = korisnik.Username;
            this.authWindow = authWindow;
        }
        private void LogoutPrompt(object sender, RoutedEventArgs e)
        {
            if(MessageBox.Show("Da li sigurno zelite da se izlogujete?", "Logout...", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                authWindow.Show();
                this.Close();
            }
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
