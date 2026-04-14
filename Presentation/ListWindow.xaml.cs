using Domain.Enums;
using Domain.Models;
using Domain.Repositories;
using Domain.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

namespace Presentation
{
    public partial class ListWindow : Window, INotifyPropertyChanged
    {
        // Database and Repos
        IVolcanoRepository volcanoesRepo;

        // Auth prozor
        MainWindow authWindow;

        // Lista vulkana
        public ObservableCollection<Volcano> Volcanoes { get; set; }

        // Korisnik
        User korisnik;

        // Servisi
        IVolcanoUpdateService volcanoUpdateService;
        IStorePhotoService storePhotoService;
        IAddVolcanoService addVolcanoService;
        IStoreRTFService storeRTFService;
        IVolcanoDeleteService volcanoDeleteService;
        IRTFTextEditingService rtfTextEditingService;

        // Select all
        private bool _isUpdating = false;

        private bool? _selectAll = false;
        public bool? SelectAll
        {
            get => _selectAll;
            set
            {
                if (_isUpdating) return;

                _isUpdating = true;

                _selectAll = value;

                if (value.HasValue)
                {
                    foreach (var v in Volcanoes)
                        v.IsSelected = value.Value;
                }

                OnPropertyChanged(nameof(SelectAll));

                _isUpdating = false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        private void Vulkan_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_isUpdating) return;

            if (e.PropertyName == nameof(Volcano.IsSelected))
            {
                _isUpdating = true;

                UpdateSelectAllState();

                _isUpdating = false;
            }
        }

        public ListWindow(User korisnik, MainWindow authWindow, IVolcanoRepository volcanoesRepo, IVolcanoUpdateService volcanoUpdateService, IStorePhotoService storePhotoService, IAddVolcanoService addVolcanoService, IStoreRTFService storeRTFService, IVolcanoDeleteService volcanoDeleteService, IRTFTextEditingService rtfTextEditingService)
        {
            this.volcanoesRepo = volcanoesRepo;
            this.volcanoUpdateService = volcanoUpdateService;
            this.storePhotoService = storePhotoService;
            this.addVolcanoService = addVolcanoService;
            this.storeRTFService = storeRTFService;
            this.volcanoDeleteService = volcanoDeleteService;
            this.rtfTextEditingService = rtfTextEditingService;

            this.korisnik = korisnik;

            InitializeComponent();

            UsernameButton.Content = korisnik.Username;
            this.authWindow = authWindow;

            if (korisnik.Role == UserRoles.Admin) AdminPanelGrid.Visibility = Visibility.Visible;
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
                v.PropertyChanged += Vulkan_PropertyChanged;
                Volcanoes.Add(v);
            }
            UpdateSelectAllState();
        }
        private void UpdateSelectAllState()
        {
            if (Volcanoes.Count == 0)
            {
                SelectAll = false;
                return;
            }

            if (Volcanoes.All(v => v.IsSelected))
                _selectAll = true;
            else if (Volcanoes.All(v => !v.IsSelected))
                _selectAll = false;
            else
                _selectAll = null;

            OnPropertyChanged(nameof(SelectAll));
        }
        private void DodajVulkanWindow(object sender, RoutedEventArgs e)
        {
            DodajVulkan dv = new DodajVulkan(addVolcanoService, this, storePhotoService, storeRTFService, rtfTextEditingService);
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

            VulkanInfo vi = new VulkanInfo(volcano, this, volcanoUpdateService, storePhotoService, korisnik, storeRTFService, rtfTextEditingService);
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
            if (MessageBox.Show("Da li sigurno zelite da se izlogujete?", "Logout...", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                authWindow.Show();
                authWindow.PrikaziPrijavu(sender, e);

                this.Close();
            }
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
