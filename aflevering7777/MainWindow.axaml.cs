using Avalonia.Controls;

namespace aflevering7777
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel(); // sørg for at denne fil ligger i ViewModels/
        }
    }
}