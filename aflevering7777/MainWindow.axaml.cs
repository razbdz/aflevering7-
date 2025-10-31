using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace aflevering7777
{
    /// <summary>
    /// ViewModel til GUI'en: styrer kø, processed orders, revenue,
    /// inventory og statusbeskeder. Binder direkte til MainWindow.axaml.
    /// </summary> 
    public class MainWindowView : INotifyPropertyChanged
    {
        // ---------- UI-bindings ----------
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

        // ---------- Commands ----------
        public ICommand ProcessNextOrderCommand { get; }
        public ICommand ProcessOrderRobotCommand { get; }

        // ---------- Model/Robot ----------
        private readonly OrderBook _orderBook = new();
        private readonly ItemSorterRobot _robot = new();

        // Simpelt lager: ItemName -> antal
        private readonly Dictionary<string, int> _stock = new();

        // Simpel tekst-kø til GUI-visning
        private readonly Queue<string> _queueText = new();

        public MainWindowView()
        {
            // Robot IP (URSim = localhost, ellers robot-VM IP)
            _robot.IpAddress = "localhost";

            // Seed data: inventory + ordrer
            SeedData();

            // Fyld UI-lister og tekster
            RebuildQueuedOrdersList();
            RebuildInventoryText();

            // Commands
            ProcessNextOrderCommand = new AsyncCommand(ProcessNextOrderAsync);
            ProcessOrderRobotCommand = new AsyncCommand(ProcessOrderRobotAsync);

            AppendStatus("Klar. Tryk 'Process Next Order' eller 'Process Order (Robot)'.");
        }

        // ---------- Init/Seed ----------
        private void SeedData()
        {
            // Varer med lokationer: a=1, b=2, c=3
            var screw = new UnitItem("M3 screw", 1.0m, 1);
            var nut   = new UnitItem("M3 nut",   1.5m, 2);
            var pen   = new UnitItem("pen",      1.0m, 3);

            // Lager
            _stock[screw.Name] = 10;
            _stock[nut.Name]   = 20;
            _stock[pen.Name]   = 15;

            // Order 1: screw x1, nut x2, pen x1
            var o1 = new Order(new Customer("Ramanda"));
            o1.AddOrderLine(new OrderLine(screw, 1));
            o1.AddOrderLine(new OrderLine(nut,   2));
            o1.AddOrderLine(new OrderLine(pen,   1));
            _orderBook.AddOrder(o1);
            _queueText.Enqueue($"Order - {o1.GetTotalOrderPrice():0.00} kr.");

            // Order 2: nut x2
            var o2 = new Order(new Customer("Totoro"));
            o2.AddOrderLine(new OrderLine(nut, 2));
            _orderBook.AddOrder(o2);
            _queueText.Enqueue($"Order - {o2.GetTotalOrderPrice():0.00} kr.");
        }

        // ---------- Helpers ----------
        private void RebuildQueuedOrdersList()
        {
            QueuedOrders.Clear();
            foreach (var t in _queueText)
                QueuedOrders.Add(t);
        }

        private void RebuildInventoryText()
        {
            var lines = new List<string>();
            foreach (var kv in _stock)
                lines.Add($"- {kv.Key}: {kv.Value}");
            InventoryText = string.Join(Environment.NewLine, lines);
        }

        private void AppendStatus(string msg)
        {
            StatusMessages += msg + Environment.NewLine;
        }

        // ---------- Commands ----------
        private async Task ProcessNextOrderAsync()
        {
            AppendStatus("Processing next order (no robot) ...");

            decimal orderTotal = 0m;
            bool any = false;

            foreach (var line in _orderBook.ProcessNextOrder())
            {
                any = true;

                ProcessedOrders.Add($"{line.Item.Name} × {line.Quantity}  →  {line.GetLinePrice():0.00} kr.");
                orderTotal += line.GetLinePrice();

                if (_stock.TryGetValue(line.Item.Name, out var have))
                    _stock[line.Item.Name] = Math.Max(0, have - line.Quantity);
            }

            if (!any)
            {
                AppendStatus("Ingen ordrer i køen.");
                return;
            }

            _totalRevenue += orderTotal;
            OnPropertyChanged(nameof(TotalRevenueText));

            if (_queueText.Count > 0) _queueText.Dequeue();
            RebuildQueuedOrdersList();
            RebuildInventoryText();

            AppendStatus($"Done. (+{orderTotal:0.00} kr.)");
            await Task.CompletedTask;
        }

        private async Task ProcessOrderRobotAsync()
        {
            AppendStatus("Processing next order (ROBOT) ...");

            decimal orderTotal = 0m;
            bool any = false;

            foreach (var line in _orderBook.ProcessNextOrder())
            {
                any = true;

                ProcessedOrders.Add($"{line.Item.Name} × {line.Quantity} (robot)");

                for (int i = 0; i < line.Quantity; i++)
                {
                    uint loc = line.Item.InventoryLocation;
                    AppendStatus($"→ Picking {line.Item.Name} @ {loc}");
                    _robot.PickUp(loc);
                    await Task.Delay(9500);

                    if (_stock.TryGetValue(line.Item.Name, out var have))
                        _stock[line.Item.Name] = Math.Max(0, have - 1);
                }

                orderTotal += line.GetLinePrice();
            }

            if (!any)
            {
                AppendStatus("Ingen ordrer i køen.");
                return;
            }

            _totalRevenue += orderTotal;
            OnPropertyChanged(nameof(TotalRevenueText));

            if (_queueText.Count > 0) _queueText.Dequeue();
            RebuildQueuedOrdersList();
            RebuildInventoryText();

            AppendStatus("Order complete (robot). Transportbånd leverer ny S-boks.");
        }

        // ---------- INotifyPropertyChanged ----------
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    /// <summary>
    /// Enkel async ICommand til knapper.
    /// </summary>
    public class AsyncCommand : ICommand
    {
        private readonly Func<Task> _execute;
        private bool _isRunning;

        public AsyncCommand(Func<Task> execute) => _execute = execute;

        public bool CanExecute(object? parameter) => !_isRunning;

        public event EventHandler? CanExecuteChanged;

        public async void Execute(object? parameter)
        {
            if (_isRunning) return;
            try
            {
                _isRunning = true;
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                await _execute();
            }
            finally
            {
                _isRunning = false;
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
