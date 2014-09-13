using System.Windows;

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
            _nextVm = new NextVm(NextClient.TestClient);
            this.DataContext = _nextVm;
        }

        private void LoginClick(object sender, RoutedEventArgs e)
        {
            _nextVm.Login();
        }
    }
}
