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
using Services.AuthService;
using Domain.Services;
using Domain.Database;
using Domain.Repositories;
using Domain.Models;
using Database.DataBase;
using Database.Repositories;


namespace Presentation
{
    public partial class MainWindow : Window
    {
        // Database and Repo
        static IDataBase usersDatabase = new UsersXMLDataBase();
        static IUserRepository userRepo = new UserRepository(usersDatabase);

        // Services
        IAuthService authService = new AuthService(userRepo);

        // Korisnik
        User korisnik = new User();

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
            if (PrijavaUsername.Text == null || PrijavaUsername.Text == "")
            {
                AuthLoginErrorText.Text = "Polje za username mora biti popunjeno!";
                return;
            }
            if (PrijavaPassword.Password == null || PrijavaPassword.Password == "")
            {
                AuthLoginErrorText.Text = "Polje za sifru mora biti popunjeno!";
                return;
            }

            bool uspesno;
            (uspesno, korisnik) = authService.Prijava(PrijavaUsername.Text, PrijavaPassword.Password);

            OcistiSvaPolja();

            if (uspesno)
            {
                ListWindow lw = new ListWindow(korisnik, this);
                lw.Show();
                this.Hide();
            }
            else
            {
                AuthLoginErrorText.Text = "Doslo je do greske priikom prijave!";
                return;
            }
        }
        private void RegistracijaClick(object sender, RoutedEventArgs e)
        {
            if(RegistracijaUsername.Text == null || RegistracijaUsername.Text == "")
            {
                AuthRegErrorText.Text = "Polje za username mora biti popunjeno!";
                return;
            }
            if (RegistracijaPassword.Password == null || RegistracijaPassword.Password == "" || RegistracijaPassword.Password.Length < 8)
            {
                AuthRegErrorText.Text = "Polje za sifru mora biti popunjeno sa vise od 8 karaktera!";
                return;
            }

            User k = new User(RegistracijaUsername.Text, RegistracijaPassword.Password, RegistracijaAdmin.IsChecked == true);

            bool uspesno;
            (uspesno, korisnik) = authService.Registracija(k);

            OcistiSvaPolja();

;           if (uspesno)
            {
                ListWindow lw = new ListWindow(korisnik, this);
                lw.Show();
                this.Hide();
            }
            else
            {
                AuthRegErrorText.Text = "Doslo je do greske priikom registracije!";
                return;
            }
        }
        void OcistiSvaPolja()
        {
            PrijavaUsername.Text = "";
            PrijavaPassword.Password = "";

            RegistracijaUsername.Text = "";
            RegistracijaPassword.Password = "";
        }
        public void PrikaziRegistraciju(object sender, RoutedEventArgs e)
        {
            PrijavaGrid.Visibility = Visibility.Collapsed;
            RegistracijaGrid.Visibility = Visibility.Visible;
        }
        public void PrikaziPrijavu(object sender, RoutedEventArgs e)
        {
            PrijavaGrid.Visibility = Visibility.Visible;
            RegistracijaGrid.Visibility = Visibility.Collapsed;
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
