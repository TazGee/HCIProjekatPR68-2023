using Domain.Enums;
using Domain.Models;
using Domain.Repositories;
using Domain.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Presentation
{
    public partial class ListWindow : Window, INotifyPropertyChanged
    {
        IVolcanoRepository volcanoesRepo;

        MainWindow authWindow;

        public ObservableCollection<Volcano> Volcanoes { get; set; }

        User user;

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

        public ListWindow(User user, MainWindow authWindow, IVolcanoRepository volcanoesRepo, IVolcanoUpdateService volcanoUpdateService, IStorePhotoService storePhotoService, IAddVolcanoService addVolcanoService, IStoreRTFService storeRTFService, IVolcanoDeleteService volcanoDeleteService, IRTFTextEditingService rtfTextEditingService)
        {
            this.volcanoesRepo = volcanoesRepo;
            this.volcanoUpdateService = volcanoUpdateService;
            this.storePhotoService = storePhotoService;
            this.addVolcanoService = addVolcanoService;
            this.storeRTFService = storeRTFService;
            this.volcanoDeleteService = volcanoDeleteService;
            this.rtfTextEditingService = rtfTextEditingService;

            this.user = user;

            InitializeComponent();

            UsernameButton.Content = user.Username;
            this.authWindow = authWindow;

            if (user.Role == UserRoles.Admin) AdminPanelGrid.Visibility = Visibility.Visible;
            else AdminPanelGrid.Visibility = Visibility.Hidden;

            Volcanoes = new ObservableCollection<Volcano>();
            this.DataContext = this;

            UpdateVolcanoList();
            this.volcanoUpdateService = volcanoUpdateService;
        }

        public void UpdateVolcanoList()
        {
            Volcanoes.Clear();

            foreach (Volcano v in volcanoesRepo.AllVolcanoes())
            {
                v.PropertyChanged += Vulkan_PropertyChanged;

                v.LocalPhotoPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, v.PhotoPath));

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
        private void AddVolcanoWindow(object sender, RoutedEventArgs e)
        {
            AddVolcano av = new AddVolcano(addVolcanoService, this, storePhotoService, storeRTFService, rtfTextEditingService);
            av.Show();
        }
        private void DeleteVolcanoes(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Do you really want to delete selected volcanoes?", "Delete volcanoes", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;

            var forDeletion = Volcanoes.Where(v => v.IsSelected).ToList();

            foreach (var v in forDeletion)
            {
                if(!volcanoDeleteService.DeleteVolcano(v.Name))
                {
                    MessageBox.Show($"There was an error while trying to delete volcano {v.Name}!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                Volcanoes.Remove(v);
            }

            UpdateVolcanoList();
        }

        private void NameClick(object sender, RoutedEventArgs e)
        {
            var hyperlink = sender as Hyperlink;
            if (hyperlink == null) return;

            var volcano = hyperlink.DataContext as Volcano;
            if (volcano == null) return;

            VulkanInfo vi = new VulkanInfo(volcano, this, volcanoUpdateService, storePhotoService, user, storeRTFService, rtfTextEditingService);
            vi.Show();
        }

        private void LogoutPrompt(object sender, RoutedEventArgs e)
        {
            if(MessageBox.Show("Do you really want to log out?", "Logout", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                authWindow.Show();
                authWindow.ShowLogin(sender, e);

                this.Close();
            }
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Do you really want to log out?", "Logout", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                authWindow.Show();
                authWindow.ShowLogin(sender, e);

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
