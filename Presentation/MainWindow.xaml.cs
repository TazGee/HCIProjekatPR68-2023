using Database.DataBase;
using Database.Repositories;
using Domain.Database;
using Domain.Enums;
using Domain.Models;
using Domain.Repositories;
using Domain.Services;
using Services.AddVolcanoService;
using Services.AuthService;
using Services.RTFTextEditingService;
using Services.SetPhotoService;
using Services.StoreRTFService;
using Services.VolcanoDeleteService;
using Services.VolcanoUpdateService;
using System.Windows;
using System.Windows.Input;


namespace Presentation
{
    public partial class MainWindow : Window
    {
        // Database and Repo
        static IDataBase usersDatabase = new UsersXMLDataBase();
        static IUserRepository userRepo = new UserRepository(usersDatabase);

        static IDataBase volcanoesDatabase = new VolcanoesXMLDataBase();
        static IVolcanoRepository volcanoesRepo = new VolcanoRepository(volcanoesDatabase);

        // Services
        IAuthService authService = new AuthService(userRepo);

        IVolcanoUpdateService volcanoUpdateService = new VolcanoUpdateService(volcanoesDatabase);
        IAddVolcanoService addVolcanoService = new AddVolcanoService(volcanoesRepo);
        IVolcanoDeleteService volcanoDeleteService = new VolcanoDeleteService(volcanoesRepo, volcanoesDatabase);

        IStorePhotoService storePhotoService = new StorePhotoService();
        IStoreRTFService storeRTFService = new StoreRTFService();

        IRTFTextEditingService rtfTextEditingService = new RTFTextEditingService();

        // User
        User user = new User();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void LoginClick(object sender, RoutedEventArgs e)
        {
            if (LoginUsername.Text == null || LoginUsername.Text == "")
            {
                AuthLoginErrorText.Text = "Username field must be filled!";
                return;
            }
            if (LoginPassword.Password == null || LoginPassword.Password == "")
            {
                AuthLoginErrorText.Text = "Password field must be filled!";
                return;
            }

            bool success;
            (success, user) = authService.Login(LoginUsername.Text, LoginPassword.Password);

            ClearFields();

            if (success)
            {
                ListWindow lw = new ListWindow(user, this, volcanoesRepo, volcanoUpdateService, storePhotoService, addVolcanoService, storeRTFService, volcanoDeleteService, rtfTextEditingService);
                lw.Show();
                this.Hide();
            }
            else
            {
                AuthLoginErrorText.Text = "Login info is not valid!";
                return;
            }
        }
        private void RegisterClick(object sender, RoutedEventArgs e)
        {
            if(RegisterUsername.Text == null || RegisterUsername.Text == "")
            {
                AuthRegErrorText.Text = "Username field must be filled!";
                return;
            }
            if (RegisterPassword.Password == null || RegisterPassword.Password == "" || RegisterPassword.Password.Length < 8)
            {
                AuthRegErrorText.Text = "Password field must be filled with atleast 8 characters!";
                return;
            }

            User u = new User(RegisterUsername.Text, RegisterPassword.Password, RegisterAdmin.IsChecked == true ? UserRoles.Admin : UserRoles.Visitor);

            bool success;
            (success, user) = authService.Register(u);

            ClearFields();

            if (success)
            {
                ListWindow lw = new ListWindow(user, this, volcanoesRepo, volcanoUpdateService, storePhotoService, addVolcanoService, storeRTFService, volcanoDeleteService, rtfTextEditingService);
                lw.Show();
                this.Hide();
            }
            else
            {
                AuthRegErrorText.Text = "There was an error while trying to create account!";
                return;
            }
        }
        void ClearFields()
        {
            LoginUsername.Text = "";
            LoginPassword.Password = "";

            RegisterUsername.Text = "";
            RegisterPassword.Password = "";

            AuthLoginErrorText.Text = "";
            AuthRegErrorText.Text = "";
        }
        public void ShowRegister(object sender, RoutedEventArgs e)
        {
            LoginGrid.Visibility = Visibility.Collapsed;
            RegistrationGrid.Visibility = Visibility.Visible;
        }
        public void ShowLogin(object sender, RoutedEventArgs e)
        {
            LoginGrid.Visibility = Visibility.Visible;
            RegistrationGrid.Visibility = Visibility.Collapsed;
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
