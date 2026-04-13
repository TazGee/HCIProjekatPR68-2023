using Database.DataBase;
using Database.Repositories;
using Domain.Database;
using Domain.Models;
using Domain.Repositories;
using Domain.Services;
using Services.AddVolcanoService;
using Services.AuthService;
using Services.SetPhotoService;
using Services.StoreRTFService;
using Services.VolcanoDeleteService;
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

namespace Presentation
{
    public partial class ListWindow : Window
    {
        // Database and Repos
        IVolcanoRepository volcanoesRepo;

        // Auth prozor
        MainWindow authWindow;

        // Lista vulkana
        public ObservableCollection<Volcano> Volcanoes { get; set; }

        // Servisi
        IVolcanoUpdateService volcanoUpdateService;
        IStorePhotoService storePhotoService;
        IAddVolcanoService addVolcanoService;
        IStoreRTFService storeRTFService;
        IVolcanoDeleteService volcanoDeleteService;

        public ListWindow(User korisnik, MainWindow authWindow, IVolcanoRepository volcanoesRepo, IVolcanoUpdateService volcanoUpdateService, IStorePhotoService storePhotoService, IAddVolcanoService addVolcanoService, IStoreRTFService storeRTFService, IVolcanoDeleteService volcanoDeleteService)
        {
            this.volcanoesRepo = volcanoesRepo;
            this.volcanoUpdateService = volcanoUpdateService;
            this.storePhotoService = storePhotoService;
            this.addVolcanoService = addVolcanoService;
            this.storeRTFService = storeRTFService;
            this.volcanoDeleteService = volcanoDeleteService;

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
        private void DodajVulkanWindow(object sender, RoutedEventArgs e)
        {
            DodajVulkan dv = new DodajVulkan(addVolcanoService, this, storePhotoService, storeRTFService);
            dv.Show();
        }
        private void ObrisiVulkane(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Da li sigurno zelite da obrisete izabrane vulkane?", "Brisanje...", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;

            var zaBrisanje = Volcanoes.Where(v => v.IsSelected).ToList();

            foreach (var v in zaBrisanje)
            {
                if(!volcanoDeleteService.DeleteVolcano(v.NazivVulkana))
                {
                    MessageBox.Show("Doslo je do greske prilikom brisanja!", "Greska", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                Volcanoes.Remove(v);
            }

            AzurirajListuVulkana();
        }

        private void Naziv_Click(object sender, RoutedEventArgs e)
        {
            var hyperlink = sender as Hyperlink;
            if (hyperlink == null) return;

            var volcano = hyperlink.DataContext as Volcano;
            if (volcano == null) return;

            VulkanInfo vi = new VulkanInfo(volcano, this, volcanoUpdateService, storePhotoService);
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
