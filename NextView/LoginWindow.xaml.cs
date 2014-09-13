using System.Windows;

namespace NextView
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow(LoginVm loginVm)
        {
            LoginVm = loginVm;
            InitializeComponent();
            this.DataContext = loginVm;
            PwBox.Password = loginVm.Password;
        }
        public LoginVm LoginVm { get; set; }

        private void LoginClick(object sender, RoutedEventArgs e)
        {
            LoginVm.Password = PwBox.Password;
            if (Remember.IsChecked == true)
            {
                LoginVm.Save();
            }
            this.Close();
        }
    }
}
