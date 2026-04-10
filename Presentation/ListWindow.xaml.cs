using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        IVolcanoRepository volcanoesRepo;

        // Korisnik
        User korisnik = new User();

        // Auth prozor
        MainWindow authWindow;

        // Lista vulkana
        public ObservableCollection<Volcano> Volcanoes { get; set; }

        // Servisi
        IVolcanoUpdateService volcanoUpdateService;

        public ListWindow(User korisnik, MainWindow authWindow, IVolcanoRepository volcanoesRepo, IVolcanoUpdateService volcanoUpdateService)
        {
            this.volcanoesRepo = volcanoesRepo;
            this.volcanoUpdateService = volcanoUpdateService;
            this.korisnik = korisnik;
            InitializeComponent();

            UsernameButton.Content = korisnik.Username;
            this.authWindow = authWindow;

            if (korisnik.Admin) AdminPanelGrid.Visibility = Visibility.Visible;
            else AdminPanelGrid.Visibility = Visibility.Hidden;

            Volcanoes = new ObservableCollection<Volcano>();
            this.DataContext = this;

            AzurirajListuVulkana();
            this.volcanoUpdateService = volcanoUpdateService;
        }

        public void AzurirajListuVulkana()
        {
            Volcanoes.Clear();

            foreach (Volcano v in volcanoesRepo.AllVolcanoes())
            {
                Volcanoes.Add(v);
            }
        }

        private void Naziv_Click(object sender, RoutedEventArgs e)
        {
            var hyperlink = sender as Hyperlink;
            if (hyperlink == null) return;

            var volcano = hyperlink.DataContext as Volcano;
            if (volcano == null) return;

            VulkanInfo vi = new VulkanInfo(volcano, this, volcanoUpdateService);
            vi.Show();
        }

        private void LogoutPrompt(object sender, RoutedEventArgs e)
        {
            if(MessageBox.Show("Da li sigurno zelite da se izlogujete?", "Logout...", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                authWindow.Show();
                authWindow.PrikaziPrijavu(sender, e);

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
