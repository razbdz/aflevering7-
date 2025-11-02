using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace aflevering7777
{
    // Minimal ViewModel så UI virker (uden afhængighed til dine Models)
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<string> QueuedOrders { get; } = new();
        public ObservableCollection<string> ProcessedOrders { get; } = new();

        private string _statusMessages = "";
        public string StatusMessages
        {
            get => _statusMessages;
            set { _statusMessages = value; OnPropertyChanged(); }
        }

        private string _inventoryText = "";
        public string InventoryText
        {
            get => _inventoryText;
            set { _inventoryText = value; OnPropertyChanged(); }
        }

        private decimal _totalRevenue;
        public string TotalRevenueText => $"Total Revenue: {_totalRevenue:0.00} kr.";

        public ICommand ProcessNextOrderCommand  { get; }
        public ICommand ProcessOrderRobotCommand { get; }

        public MainWindowViewModel()
        {
            // Demo-data så UI ikke er tomt
            QueuedOrders.Add("Order - 90,00 kr.");
            QueuedOrders.Add("Order - 70,00 kr.");
            InventoryText = "- M3 screw: 10\n- M3 nut: 20\n- pen: 15";

            ProcessNextOrderCommand  = new AsyncCommand(ProcessNextOrderAsync);
            ProcessOrderRobotCommand = new AsyncCommand(ProcessOrderRobotAsync);

            Append("Klar. Tryk på en af knapperne.");
        }

        private async Task ProcessNextOrderAsync()
        {
            if (QueuedOrders.Count == 0)
            {
                Append("Ingen ordrer i køen.");
                return;
            }

            // Flyt første ordre fra kø til processed
            var next = QueuedOrders[0];
            QueuedOrders.RemoveAt(0);
            ProcessedOrders.Add(next + "  (no-robot)");

            // Demo: læg lidt til revenue
            _totalRevenue += 10m;
            OnPropertyChanged(nameof(TotalRevenueText));

            Append($"Behandlede: {next}");
            await Task.CompletedTask;
        }

        private async Task ProcessOrderRobotAsync()
        {
            if (QueuedOrders.Count == 0)
            {
                Append("Ingen ordrer i køen (robot).");
                return;
            }

            var next = QueuedOrders[0];
            QueuedOrders.RemoveAt(0);
            ProcessedOrders.Add(next + "  (robot)");

            _totalRevenue += 10m;
            OnPropertyChanged(nameof(TotalRevenueText));

            Append($"[ROBOT] Sender URScript (demo) for: {next}");
            await Task.Delay(300); // lille fake vent
        }

        private void Append(string msg) => StatusMessages += msg + Environment.NewLine;

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    // Lille async ICommand
    public class AsyncCommand : ICommand
    {
        private readonly Func<Task> _execute;
        private bool _running;

        public AsyncCommand(Func<Task> execute) => _execute = execute;

        public bool CanExecute(object? parameter) => !_running;
        public event EventHandler? CanExecuteChanged;

        public async void Execute(object? parameter)
        {
            if (_running) return;
            try
            {
                _running = true;
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                await _execute();
            }
            finally
            {
                _running = false;
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}

