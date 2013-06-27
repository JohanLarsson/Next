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
using Next;

namespace NextView
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly NextVm _nextVm;

        public MainWindow()
        {
            InitializeComponent();
            _nextVm = new NextVm(new NextClient(ApiVersion.Test));
            this.DataContext = _nextVm;
        }

        private void LoginClick(object sender, RoutedEventArgs e)
        {
            _nextVm.Login();
        }
    }
}
