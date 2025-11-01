using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace aflevering7777
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<string> QueuedOrders { get; } = new();
        public ObservableCollection<string> ProcessedOrders { get; } = new();

        private string _status = "";
        public string StatusMessages { get => _status; set { _status = value; OnPropertyChanged(); } }

        private string _inventory = "- M3 screw: 10\n- M3 nut: 20\n- pen: 15";
        public string InventoryText { get => _inventory; set { _inventory = value; OnPropertyChanged(); } }

        private decimal _revenue = 0m;
        public string TotalRevenueText => $"Total Revenue: {_revenue:0.00} kr.";

        public ICommand ProcessNextOrderCommand  { get; }
        public ICommand ProcessOrderRobotCommand { get; }

        public MainWindowViewModel()
        {
            QueuedOrders.Add("Order - 5,00 kr.");
            QueuedOrders.Add("Order - 3,00 kr.");

            ProcessNextOrderCommand  = new AsyncCommand(async () => await ProcessAsync(false));
            ProcessOrderRobotCommand = new AsyncCommand(async () => await ProcessAsync(true));

            Append("Klar. Tryk på en knap.");
        }

        private async Task ProcessAsync(bool robot)
        {
            if (QueuedOrders.Count == 0) { Append("Ingen ordrer i køen."); return; }

            var order = QueuedOrders[0];
            QueuedOrders.RemoveAt(0);
            ProcessedOrders.Add(order + (robot ? " (robot)" : ""));

            if (decimal.TryParse(order.Replace("Order - ", "").Replace(",00 kr.", ""), out var price))
            {
                _revenue += price;
                OnPropertyChanged(nameof(TotalRevenueText));
            }

            if (robot)
            {
                Append("Sender URScript til robot (demo)...");
                await Task.Delay(1000);
                Append("Robot færdig.");
            }
            else
            {
                Append("Ordre behandlet uden robot.");
            }
        }

        private void Append(string msg) => StatusMessages += msg + Environment.NewLine;

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class AsyncCommand : ICommand
    {
        private readonly Func<Task> _execute;
        private bool _busy;

        public AsyncCommand(Func<Task> execute) => _execute = execute;
        public bool CanExecute(object? parameter) => !_busy;
        public event EventHandler? CanExecuteChanged;
        public async void Execute(object? parameter)
        {
            if (_busy) return;
            try { _busy = true; CanExecuteChanged?.Invoke(this, EventArgs.Empty); await _execute(); }
            finally { _busy = false; CanExecuteChanged?.Invoke(this, EventArgs.Empty); }
        }
    }
}

