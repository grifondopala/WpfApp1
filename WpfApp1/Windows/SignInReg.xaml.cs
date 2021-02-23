using Microsoft.Win32;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using WpfApp1.Windows;

namespace WpfApp1
{

    public partial class SignInReg : Window
    {

        ProcessProperty property;
        UserContext db = new UserContext();
        RegistryKey WpfName;

        public SignInReg()
        {
            InitializeComponent();
        }


        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            WpfName = Registry.CurrentUser.OpenSubKey(@"WpfName", true);
            if (WpfName == null || WpfName.GetValue("IsLogined").ToString() == "false")
            {
                if(WpfName == null)
                {
                    WpfName = Registry.CurrentUser.CreateSubKey("WpfName", true);
                    FillRegistry("false", "null", "null", "null");
                }

                property = new ProcessProperty() { Process = "Welcome", LoginProperty = "OK", MailProperty = "OK", PasswordProperty = "OK", SignInProperty = "OK" };
                WelcomeText.DataContext = property;
                BottomGrid.DataContext = property;
                RegistrationGrid.DataContext = property;
                BackButton.DataContext = property;
                LoginGrid.DataContext = property;

                LoginReg.DataContext = property;
                MailReg.DataContext = property;
                PasswordReg.DataContext = property;

                LoginSignIn.DataContext = property;
                PasswordSignIn.DataContext = property;
            }
            else
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
        }

        private void FillRegistry(string IsLogined, string ID, string Login, string Password)
        {
            WpfName.SetValue("IsLogined", IsLogined);
            WpfName.SetValue("ID", ID);
            WpfName.SetValue("Login", Login);
            WpfName.SetValue("Password", Password);
        }
        private void ChangeProgress(object sender, RoutedEventArgs e)
        {
            if (sender.Equals(Register))
            {
                property.Process = "Register";
            }
            else if(sender.Equals(Login))
            {
                property.Process = "Login";
            }
            else
            {
                property.Process = "Welcome";
            }
        }


        private void SignIn(object sender, RoutedEventArgs e)
        {
            string login = LoginSignIn.Text;
            string password = PasswordSignIn.Password;

            User user = db.Users.FirstOrDefault(u => u.Login == login);
            if(user != null)
            {
                if(password != user.Password)
                {
                    property.SignInProperty = "Password";
                    return;
                }
            }
            else
            {
                property.SignInProperty = "UserNotFound";
                return;
            }
            property.SignInProperty = "OK";

            FillRegistry("true", user.ID.ToString(), login, password);

            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
            
        }

        private void RegisterNewUser(object sender, RoutedEventArgs e)
        {
            string login = LoginReg.Text;
            string mail = MailReg.Text;
            string password = PasswordReg.Password;
            string confirmPassword = ConfirmPasswordReg.Password;

            if(login.Length < 5 || login.Length > 16) // длина логина
            {
                property.LoginProperty = "Lenght";
                return;
            }

            int count = db.Users.Count(u => u.Login == login);
            if(count == 1) // Занятость логина
            {
                property.LoginProperty = "WasTaken";
                return;
            }
            property.LoginProperty = "OK";

            if (!mail.Contains('@')) // формат почты
            {
                property.MailProperty = "Format";
                return;
            }
            property.MailProperty = "OK";

            if (password.Length < 8 || password.Length > 24) //длина пароля
            {
                property.PasswordProperty = "Lenght";
                return;
            }

            if(password != confirmPassword) //совпадение паролей
            {
                property.PasswordProperty = "DontMatch";
                return;
            }
            property.PasswordProperty = "OK";

            int index = db.Users.Count();
            User user = new User { ID = index+1 , Login = login, Mail = mail, Password = password };
            db.Users.Add(user);
            db.SaveChanges();

            FillRegistry("true", (index + 1).ToString(), login, password);

            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();

        }
    }
}
