using Avalonia.Controls;
using Avalonia.Interactivity; // for RoutedEventArgs

namespace aflevering7777
{
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel _vm = new();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = _vm;              // Sæt DataContext KUN her
            this.Title = "Item Sorter Robot (VM OK)";
        }

        // Midlertidige click-handlere der bare kalder Commands direkte:
        private void OnProcessNextOrder(object? sender, RoutedEventArgs e)
        {
            if (_vm.ProcessNextOrderCommand?.CanExecute(null) == true)
                _vm.ProcessNextOrderCommand.Execute(null);
        }

        private void OnProcessOrderRobot(object? sender, RoutedEventArgs e)
        {
            if (_vm.ProcessOrderRobotCommand?.CanExecute(null) == true)
                _vm.ProcessOrderRobotCommand.Execute(null);
        }
    }
}
