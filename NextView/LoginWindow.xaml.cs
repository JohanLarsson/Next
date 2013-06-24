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
