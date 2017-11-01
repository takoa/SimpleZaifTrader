using ZaifNet.Public;
using ZaifNet.Trade;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Newtonsoft.Json;
using CryptoWei;

using TradeResponse = ZaifNet.Public.TradeResponse;

namespace SimpleZaifTrader
{
    public partial class MainWindow : Window
    {
        private const int Failed = 0;
        private const int Succeeded = 1;

        public CurrencyPairSettings CurrencyPairSettings { get; private set; } = Global.CurrencyPairDictionary[Global.CurrencyPairList[0]];

        private Dictionary<string, Action> commands = new Dictionary<string, Action>();

        private Preferences preferences;

        private HttpClient client = new HttpClient();
        private decimal usdjpy = 1m;

        private PublicApiUtility publicApi = new PublicApiUtility();
        private TradeApiUtility tradeApi;
        private CancellationTokenSource cancellationTokenSource;
        private BackgroundWorker usdjpyUpdater = new BackgroundWorker() { WorkerReportsProgress = true };
        private BackgroundWorker activeOrderUpdater = new BackgroundWorker() { WorkerReportsProgress = true };
        private BackgroundWorker publicApiUpdater;

        private string timestamp;
        private int bidBookSize = 10;
        private int askBookSize = 10;
        private IBookOrder[] bids = new IBookOrder[10];
        private IBookOrder[] asks = new IBookOrder[10];
        private ITradeHistoryData[] tradeHistory = new ITradeHistoryData[20];
        private decimal lastPrice = 0m;

        private ObservableCollection<Order> orders = new ObservableCollection<Order>();
        private ActiveOrdersReturn[] previousActiveOrders;

        public MainWindow()
        {
            for (int i = 0; i < this.bids.Length; i++)
            {
                this.bids[i] = new ZaifBookOrder();
                this.asks[i] = new ZaifBookOrder();
            }

            for (int i = 0; i < this.tradeHistory.Length; i++)
            {
                this.tradeHistory[i] = new ZaifTradeHistoryData();
            }

            using (StreamReader reader = new StreamReader(@"API.json"))
            {
                string json = reader.ReadToEnd();
                ApiKeys keys = JsonConvert.DeserializeObject<ApiKeys>(json);

                this.tradeApi = new TradeApiUtility(keys.ApiKey, keys.SecretKey);
            }

            using (StreamReader reader = new StreamReader(@"Preferences.json"))
            {
                string json = reader.ReadToEnd();

                this.preferences = JsonConvert.DeserializeObject<Preferences>(json);
            }

            this.RegisterCommands();

            InitializeComponent();
        }

        private void RegisterCommands()
        {
            this.commands.Add("CopyLastPrice", () => this.priceNumericTextBox.Text = this.lastPrice.ToString());
            this.commands.Add("ClearPrice", () => this.priceNumericTextBox.Text = 0m.ToString());
            this.commands.Add("ClearAmount", () => this.amountNumericTextBox.Text = 0m.ToString());

            this.commands.Add("IncreasePrice1", () => this.AddToNumericTextBox(this.priceNumericTextBox, this.CurrencyPairSettings.PriceUnitStep));
            this.commands.Add("IncreasePrice2", () => this.AddToNumericTextBox(this.priceNumericTextBox, this.CurrencyPairSettings.PriceUnitStep * 10m));
            this.commands.Add("IncreasePrice3", () => this.AddToNumericTextBox(this.priceNumericTextBox, this.CurrencyPairSettings.PriceUnitStep * 100m));
            this.commands.Add("DecreasePrice1", () => this.AddToNumericTextBox(this.priceNumericTextBox, -this.CurrencyPairSettings.PriceUnitStep));
            this.commands.Add("DecreasePrice2", () => this.AddToNumericTextBox(this.priceNumericTextBox, this.CurrencyPairSettings.PriceUnitStep * -10m));
            this.commands.Add("DecreasePrice3", () => this.AddToNumericTextBox(this.priceNumericTextBox, this.CurrencyPairSettings.PriceUnitStep * -100m));

            this.commands.Add("IncreaseAmount1", () => this.AddToNumericTextBox(this.amountNumericTextBox, this.CurrencyPairSettings.AmountUnitStep));
            this.commands.Add("IncreaseAmount2", () => this.AddToNumericTextBox(this.amountNumericTextBox, this.CurrencyPairSettings.AmountUnitStep * 10m));
            this.commands.Add("IncreaseAmount3", () => this.AddToNumericTextBox(this.amountNumericTextBox, this.CurrencyPairSettings.AmountUnitStep * 100m));
            this.commands.Add("DecreaseAmount1", () => this.AddToNumericTextBox(this.amountNumericTextBox, -this.CurrencyPairSettings.AmountUnitStep));
            this.commands.Add("DecreaseAmount2", () => this.AddToNumericTextBox(this.amountNumericTextBox, this.CurrencyPairSettings.AmountUnitStep * -10m));
            this.commands.Add("DecreaseAmount3", () => this.AddToNumericTextBox(this.amountNumericTextBox, this.CurrencyPairSettings.AmountUnitStep * -100m));
        }

        private void AddToNumericTextBox(NumericTextBox textBox, decimal delta)
        {
            decimal d = (!textBox.Text.Equals(string.Empty) ? decimal.Parse(textBox.Text) : 0m) + delta;

            textBox.Text = (0 <= d ? d : 0m).ToString();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.usdjpyUpdater.DoWork += (obj, eventArgs) =>
            {
                while (true)
                {
                    this.usdjpy = decimal.Parse(this.GetUsdJpy().GetAwaiter().GetResult());
                    Thread.Sleep(60000);
                }
            };
            this.usdjpyUpdater.RunWorkerAsync();

            this.activeOrderUpdater.DoWork += (obj, eventArgs) =>
            {
                while (true)
                {
                    ActiveOrdersResponse activeOrers = this.tradeApi.PostActiveOrders(this.CurrencyPairSettings.Name).GetAwaiter().GetResult();

                    ((BackgroundWorker)obj).ReportProgress(0, activeOrers);
                    Thread.Sleep(5000);
                }
            };
            this.activeOrderUpdater.ProgressChanged += (obj, eventArgs) =>
            {
                ActiveOrdersResponse activeOrders = (ActiveOrdersResponse)eventArgs.UserState;
                bool areActiveOrdersChanged = false;

                if (activeOrders != null && this.previousActiveOrders != null && activeOrders.Return != null && activeOrders.Return.Length == this.previousActiveOrders.Length)
                {
                    for (int i = 0; i < activeOrders.Return.Length; i++)
                    {
                        if (!activeOrders.Return[i].Equals(this.previousActiveOrders[i]))
                        {
                            areActiveOrdersChanged = true;

                            break;
                        }
                    }
                }
                else if (activeOrders != null
                    && ((activeOrders.Return == null && this.previousActiveOrders != null) || (activeOrders.Return != null && this.previousActiveOrders == null)))
                {
                    areActiveOrdersChanged = true;
                }

                if (areActiveOrdersChanged)
                {
                    this.orders.Clear();

                    if (activeOrders.Return != null)
                    {
                        for (int i = 0; i < activeOrders.Return.Length; i++)
                        {
                            this.orders.Add(new Order()
                            {
                                ID = activeOrders.Return[i].ID,
                                Action = activeOrders.Return[i].Action,
                                Price = activeOrders.Return[i].Price,
                                Amount = activeOrders.Return[i].Amount,
                                Comment = activeOrders.Return[i].Comment
                            });
                        }
                    }

                    this.previousActiveOrders = activeOrders.Return;
                }
            };
            this.activeOrderUpdater.RunWorkerAsync();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            for (int i = 0; i < this.preferences.Shortcuts.Length; i++)
            {
                if (this.preferences.Shortcuts[i].IsHit)
                {
                    this.commands[this.preferences.Shortcuts[i].CommandName]();

                    break;
                }
            }
        }

        private async Task<string> GetUsdJpy()
        {
            string usdjpy = await this.client.GetStringAsync("https://finance.google.com/finance/converter?a=1&from=USD&to=JPY");
            Regex regex = new Regex(@"<span class=bld>(?<value>.*?) JPY</span>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            Match result = regex.Match(usdjpy);

            return result.Groups["value"].Value;
        }

        private void CurrencyPairComboBox_Initialized(object sender, EventArgs e) => this.currencyPairComboBox.ItemsSource = Global.CurrencyPairList;

        private async void CurrencyPairComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();

            this.cancellationTokenSource?.Cancel();
            this.cancellationTokenSource?.Dispose();
            this.publicApiUpdater?.CancelAsync();
            this.publicApiUpdater?.Dispose();

            this.CurrencyPairSettings = Global.CurrencyPairDictionary[(string)this.currencyPairComboBox.SelectedItem];

            this.cancellationTokenSource = new CancellationTokenSource();
            this.publicApiUpdater = new BackgroundWorker() { WorkerReportsProgress = true, WorkerSupportsCancellation = true };

            if (this.useHttpsCheckBox != null && this.useHttpsCheckBox.IsChecked == true)
            {
                this.logTextBox.Text += "GETting Data of " + this.CurrencyPairSettings.Name + " using https API." + Environment.NewLine;
                this.logTextBox.ScrollToLine(this.logTextBox.LineCount - 1);

                this.publicApiUpdater.DoWork += this.PublicApiDoWorkEventHandler;
                this.publicApiUpdater.ProgressChanged += this.PublicApiUpdaterProgressChangedEventHandler;
            }
            else
            {
                this.publicApiUpdater.DoWork += this.StreamingApiUpdaterDoWorkEventHandler;
                this.publicApiUpdater.ProgressChanged += this.StreamingApiUpdaterProgressChangedEventHandler;

                await this.GetTradeInfoWithHttps();
                this.UpdateTradeInfo();
            }

            this.publicApiUpdater.RunWorkerAsync();
        }

        private void OrderDataGrid_Initialized(object sender, EventArgs e) => this.orderDataGrid.ItemsSource = this.orders;

        private void OrderDataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "OrderID":
                    e.Column.DisplayIndex = 1;
                    e.Column.Header = "ID";
                    e.Column.IsReadOnly = true;

                    break;
                case "Action":
                    e.Column.DisplayIndex = 2;
                    e.Column.Header = "種類";
                    e.Column.IsReadOnly = true;

                    break;
                case "Price":
                    e.Column = new NumericDataGridColumn()
                    {
                        Binding = new Binding("Price"),
                        DisplayIndex = 3,
                        Header = "値段",
                        IsReadOnly = false
                    };

                    break;
                case "Amount":
                    e.Column = new NumericDataGridColumn()
                    {
                        Binding = new Binding("Amount"),
                        DisplayIndex = 4,
                        Header = "数量",
                        IsReadOnly = false
                    };

                    break;
                case "Limit":
                    e.Column = new NumericDataGridColumn()
                    {
                        Binding = new Binding("Limit"),
                        DisplayIndex = 5,
                        Header = "Limit",
                        IsReadOnly = false
                    };

                    break;
                default:
                    break;
            }
        }

        private async void CancelOrderButton_Click(object sender, RoutedEventArgs e)
        {
            Order order = ((FrameworkElement)sender).DataContext as Order;
            CancelOrderResponse cancelledOrder;

            if (order == null)
            {
                return;
            }

            cancelledOrder = await this.tradeApi.PostCancelOrder(this.CurrencyPairSettings.Name, order.ID);

            if (cancelledOrder.Success == MainWindow.Succeeded)
            {
                this.logTextBox.Text += string.Format("Cancelled an order: {{ID: {0}}}", cancelledOrder.Return.OrderId) + Environment.NewLine;
            }
            else
            {
                this.logTextBox.Text += string.Format("Failed to cancel an order: {{ID: {0}}}", order.ID) + Environment.NewLine;
            }
        }

        #region ApiBackgroundWorkerEventHandlers

        private void StreamingApiUpdaterDoWorkEventHandler(object sender, DoWorkEventArgs args)
        {
            StreamingApiUtility sa = new StreamingApiUtility();

            sa.StartReceiving(this.CurrencyPairSettings.Name, this.StreamingApiCallback, this.publicApiUpdater, cancellationTokenSource.Token).GetAwaiter().GetResult();
        }

        private void StreamingApiUpdaterProgressChangedEventHandler(object sender, ProgressChangedEventArgs args)
        {
            StreamingApiUtility.CallbackArgs callbackArgs = (StreamingApiUtility.CallbackArgs)args.UserState;

            switch (callbackArgs.Status)
            {
                case StreamingApiUtility.CallbackStatus.Connected:
                    this.logTextBox.Text += "Connected to " + callbackArgs.CurrencyPair + "." + Environment.NewLine;
                    this.logTextBox.ScrollToLine(this.logTextBox.LineCount - 1);

                    break;
                case StreamingApiUtility.CallbackStatus.Disconnected:
                    this.logTextBox.Text += "Disconnected from " + callbackArgs.CurrencyPair + "." + Environment.NewLine;
                    this.logTextBox.ScrollToLine(this.logTextBox.LineCount - 1);

                    break;
                case StreamingApiUtility.CallbackStatus.DataReceived:
                    if (callbackArgs.CurrencyPair == this.CurrencyPairSettings.Name)
                    {
                        this.OnStreamingDataReceived(callbackArgs.StreamingData);
                    }

                    break;
                default:
                    break;
            }
        }

        private void StreamingApiCallback(StreamingApiUtility.CallbackArgs args, object callbackState)
        {
            ((BackgroundWorker)callbackState).ReportProgress(0, args);
        }

        private void OnStreamingDataReceived(StreamingData streamingData)
        {
            this.timestamp = streamingData.Timestamp;

            this.bidBookSize = Math.Min(this.bids.Length, 10);
            this.askBookSize = Math.Min(this.asks.Length, 10);

            for (int i = 0; i < this.bidBookSize; i++)
            {
                ((ZaifBookOrder)this.bids[i]).Update(streamingData.Bids[i]);
            }

            for (int i = 0; i < this.askBookSize; i++)
            {
                ((ZaifBookOrder)this.asks[i]).Update(streamingData.Asks[i]);
            }

            for (int i = 0; i < this.tradeHistory.Length; i++)
            {
                ((ZaifTradeHistoryData)this.tradeHistory[i]).Update(streamingData.Logs[i]);
            }

            this.lastPrice = streamingData.LastPrice.PriceRaw;

            this.timestampLabel.Content = string.Format("Last Updated: {0}", this.timestamp);
            this.UpdateTradeInfo();
        }

        private void PublicApiDoWorkEventHandler(object sender, DoWorkEventArgs args)
        {
            BackgroundWorker worker = ((BackgroundWorker)sender);

            while (true)
            {
                if (worker.CancellationPending)
                {
                    break;
                }

                this.GetTradeInfoWithHttps().GetAwaiter().GetResult();

                worker.ReportProgress(0);

                Thread.Sleep(300);
            }
        }

        private void PublicApiUpdaterProgressChangedEventHandler(object sender, ProgressChangedEventArgs args)
        {
            this.timestampLabel.Content = string.Format("Last Updated: {0}", this.timestamp);

            this.UpdateTradeInfo();
        }

        #endregion

        private async Task GetTradeInfoWithHttps()
        {
            DepthResponse depth = await this.publicApi.GetDepth(this.CurrencyPairSettings.Name);
            TradeResponse[] trades = await this.publicApi.GetTrades(this.CurrencyPairSettings.Name);

            this.bidBookSize = Math.Min(this.bids.Length, 10);
            this.askBookSize = Math.Min(this.asks.Length, 10);

            for (int i = 0; i < this.bidBookSize; i++)
            {
                ((ZaifBookOrder)this.bids[i]).Update(depth.Bids[i]);
            }

            for (int i = 0; i < this.askBookSize; i++)
            {
                ((ZaifBookOrder)this.asks[i]).Update(depth.Asks[i]);
            }

            for (int i = 0; i < this.tradeHistory.Length; i++)
            {
                ((ZaifTradeHistoryData)this.tradeHistory[i]).Update(trades[i]);
            }

            this.lastPrice = this.tradeHistory[0].Price;
            this.timestamp = DateTime.Now.ToString("yyyy-mm-dd HH:mm:ss.ffffff");
        }

        private void UpdateTradeInfo()
        {
            StringBuilder sb = new StringBuilder();

            string tradeHistoryFormat = Global.GetTradeHistoryFormat(this.CurrencyPairSettings);
            string orderBookFormat = Global.GetOrderBookFormat(this.CurrencyPairSettings);
            string lastPriceFormat = Global.GetLastPriceFormat(this.CurrencyPairSettings);

            sb.AppendLine("             全取引履歴             ").AppendLine();

            for (int i = 0; i < this.tradeHistory.Length; i++)
            {
                sb.AppendFormat(tradeHistoryFormat,
                    this.tradeHistory[i].Date.ToString("HH:mm:ss"),
                    this.tradeHistory[i].Type == TradeTypes.Bid ? "買い" : "売り",
                    this.tradeHistory[i].Price,
                    this.tradeHistory[i].Amount).AppendLine();
            }

            this.tradeHistoryTextBlock.Text = sb.ToString();
            sb.Clear();
            sb.AppendFormat("      売り         値段         買い      ").AppendLine().AppendLine();

            for (int i = 0; i < this.askBookSize; i++)
            {
                sb.AppendFormat(orderBookFormat,
                    this.asks[this.askBookSize - 1 - i].Amount,
                    this.asks[this.askBookSize - 1 - i].Price,
                    string.Empty).AppendLine();
            }

            sb.AppendLine();
            sb.AppendFormat(lastPriceFormat, this.lastPrice, (this.asks[0].Price - this.bids[0].Price) / this.lastPrice).AppendLine();
            sb.AppendLine();

            for (int i = 0; i < this.bidBookSize; i++)
            {
                sb.AppendFormat(orderBookFormat,
                    string.Empty,
                    this.bids[i].Price,
                    this.bids[i].Amount).AppendLine();
            }

            this.orderBookTextBlock.Text = sb.ToString();

            this.usdPriceLabel.Content = string.Format("${0:#######0.00}  @{1:##0.000} USD/JPY", this.lastPrice / this.usdjpy, this.usdjpy);
        }

        #region TradeButtonEventHandlers

        private async void LimitBuyButton_Click(object sender, RoutedEventArgs e)
        {
            decimal price;
            decimal amount;
            decimal? limit = null;
            ZaifNet.Trade.TradeResponse trade;

            if (!decimal.TryParse(this.priceNumericTextBox.Text, out price))
            {
                this.logTextBox.Text += "値段が不正です。" + Environment.NewLine;

                return;
            }
            if (!decimal.TryParse(this.amountNumericTextBox.Text, out amount))
            {
                this.logTextBox.Text += "数量が不正です。" + Environment.NewLine;

                return;
            }
            if (!this.limitNumericTextBox.Text.Equals(string.Empty))
            {
                decimal d;

                if (!decimal.TryParse(this.limitNumericTextBox.Text, out d))
                {

                    this.logTextBox.Text += "LIMITが不正です。" + Environment.NewLine;

                    return;
                }

                limit = d;
            }

            if (this.lastPrice * 1.05m <= price || price <= this.lastPrice * 0.95m)
            {
                if (MessageBox.Show(this, "最終価格と5%以上の差があります。よろしいですか？", "確認", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                {
                    return;
                }
            }

            trade = await this.tradeApi.PostTrade(this.CurrencyPairSettings.Name, "bid", price, amount, limit, string.Empty);

            if (trade.Success == MainWindow.Succeeded)
            {
                this.logTextBox.Text += string.Format("Opened a buy order: price={0}, amount={1}, limit={2}", price, amount, limit == null ? "--" : limit.ToString()) + Environment.NewLine;
            }
            else
            {
                this.logTextBox.Text += "Failed to open a buy order." + Environment.NewLine;
            }

            this.PutInterval(this.limitBuyButton);
        }

        private async void LimitSellButton_Click(object sender, RoutedEventArgs e)
        {
            decimal price;
            decimal amount;
            decimal? limit = null;
            ZaifNet.Trade.TradeResponse trade;

            if (!decimal.TryParse(this.priceNumericTextBox.Text, out price))
            {
                this.logTextBox.Text += "値段が不正です。" + Environment.NewLine;

                return;
            }
            if (!decimal.TryParse(this.amountNumericTextBox.Text, out amount))
            {
                this.logTextBox.Text += "数量が不正です。" + Environment.NewLine;

                return;
            }
            if (!this.limitNumericTextBox.Text.Equals(string.Empty))
            {
                decimal d;

                if (!decimal.TryParse(this.limitNumericTextBox.Text, out d))
                {

                    this.logTextBox.Text += "LIMITが不正です。" + Environment.NewLine;

                    return;
                }

                limit = d;
            }

            if (this.lastPrice * 1.05m <= price || price <= this.lastPrice * 0.95m)
            {
                if (MessageBox.Show(this, "最終価格と5%以上の差があります。よろしいですか？", "確認", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                {
                    return;
                }
            }

            trade = await this.tradeApi.PostTrade(this.CurrencyPairSettings.Name, "ask", price, amount, limit, string.Empty);

            if (trade.Success == MainWindow.Succeeded)
            {
                this.logTextBox.Text += string.Format("Opened a sell order: {{price: {0}, amount: {1}, limit: {2}}}", price, amount, limit == null ? "--" : limit.ToString()) + Environment.NewLine;
            }
            else
            {
                this.logTextBox.Text += "Failed to open a sell order." + Environment.NewLine;
            }

            this.PutInterval(this.limitSellButton);
        }

        private async void MarketBuyButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.limitedMarketOrderCheckBox.IsChecked == true)
            {
                decimal offset;
                decimal amount;
                decimal? limit = null;
                ZaifNet.Trade.TradeResponse trade;

                if (!decimal.TryParse(this.limitedMarketOrderNumericTextBox.Text, out offset))
                {
                    this.logTextBox.Text += "差額が不正です。" + Environment.NewLine;

                    return;
                }
                if (!decimal.TryParse(this.amountNumericTextBox.Text, out amount))
                {
                    this.logTextBox.Text += "数量が不正です。" + Environment.NewLine;

                    return;
                }
                if (!this.limitNumericTextBox.Text.Equals(string.Empty))
                {
                    decimal d;

                    if (!decimal.TryParse(this.limitNumericTextBox.Text, out d))
                    {

                        this.logTextBox.Text += "LIMITが不正です。" + Environment.NewLine;

                        return;
                    }

                    limit = d;
                }

                trade = await this.tradeApi.PostTrade(this.CurrencyPairSettings.Name, "bid", this.lastPrice - offset, amount, limit, string.Empty);

                if (trade.Success == MainWindow.Succeeded)
                {
                    this.logTextBox.Text += string.Format("Opened a buy order: price={0}, amount={1}, limit={2}", this.lastPrice - offset, amount, limit == null ? "--" : limit.ToString()) + Environment.NewLine;
                }
                else
                {
                    this.logTextBox.Text += "Failed to open a buy order." + Environment.NewLine;
                }
            }
            else
            {
                decimal amount;
                decimal? limit = null;
                decimal bigLimit = MainWindow.Floor(this.lastPrice * 1.8m, this.CurrencyPairSettings.PriceUnitStep);
                ZaifNet.Trade.TradeResponse trade;

                if (!decimal.TryParse(this.amountNumericTextBox.Text, out amount))
                {
                    this.logTextBox.Text += "数量が不正です。" + Environment.NewLine;

                    return;
                }
                if (!this.limitNumericTextBox.Text.Equals(string.Empty))
                {
                    decimal d;

                    if (!decimal.TryParse(this.limitNumericTextBox.Text, out d))
                    {

                        this.logTextBox.Text += "LIMITが不正です。" + Environment.NewLine;

                        return;
                    }

                    limit = d;
                }

                trade = await this.tradeApi.PostTrade(this.CurrencyPairSettings.Name, "bid", bigLimit, amount, limit, string.Empty);

                if (trade.Success == MainWindow.Succeeded)
                {
                    this.logTextBox.Text += string.Format("Opened a buy order: price={0}, amount={1}, limit={2}", bigLimit, amount, limit == null ? "--" : limit.ToString()) + Environment.NewLine;
                }
                else
                {
                    this.logTextBox.Text += "Failed to open a buy order." + Environment.NewLine;
                }
            }

            this.PutInterval(this.marketBuyButton);
        }

        private async void LimitedMarketSellButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.limitedMarketOrderCheckBox.IsChecked == true)
            {
                decimal offset;
                decimal amount;
                decimal? limit = null;
                ZaifNet.Trade.TradeResponse trade;

                if (!decimal.TryParse(this.limitedMarketOrderNumericTextBox.Text, out offset))
                {
                    this.logTextBox.Text += "差額が不正です。" + Environment.NewLine;

                    return;
                }
                if (!decimal.TryParse(this.amountNumericTextBox.Text, out amount))
                {
                    this.logTextBox.Text += "数量が不正です。" + Environment.NewLine;

                    return;
                }
                if (!this.limitNumericTextBox.Text.Equals(string.Empty))
                {
                    decimal d;

                    if (!decimal.TryParse(this.limitNumericTextBox.Text, out d))
                    {

                        this.logTextBox.Text += "LIMITが不正です。" + Environment.NewLine;

                        return;
                    }

                    limit = d;
                }

                trade = await this.tradeApi.PostTrade(this.CurrencyPairSettings.Name, "ask", this.lastPrice + offset, amount, limit, string.Empty);

                if (trade.Success == MainWindow.Succeeded)
                {
                    this.logTextBox.Text += string.Format("Opened a sell order: price={0}, amount={1}, limit={2}", this.lastPrice + offset, amount, limit == null ? "--" : limit.ToString()) + Environment.NewLine;
                }
                else
                {
                    this.logTextBox.Text += "Failed to open a sell order." + Environment.NewLine;
                }
            }
            else
            {
                decimal amount;
                decimal? limit = null;
                decimal smallLimit = MainWindow.Floor(this.lastPrice * 0.6m, this.CurrencyPairSettings.PriceUnitStep);
                ZaifNet.Trade.TradeResponse trade;

                if (!decimal.TryParse(this.amountNumericTextBox.Text, out amount))
                {
                    this.logTextBox.Text += "数量が不正です。" + Environment.NewLine;

                    return;
                }
                if (!this.limitNumericTextBox.Text.Equals(string.Empty))
                {
                    decimal d;

                    if (!decimal.TryParse(this.limitNumericTextBox.Text, out d))
                    {

                        this.logTextBox.Text += "LIMITが不正です。" + Environment.NewLine;

                        return;
                    }

                    limit = d;
                }

                trade = await this.tradeApi.PostTrade(this.CurrencyPairSettings.Name, "ask", smallLimit, amount, limit, string.Empty);

                if (trade.Success == MainWindow.Succeeded)
                {
                    this.logTextBox.Text += string.Format("Opened a sell order: price={0}, amount={1}, limit={2}", smallLimit, amount, limit == null ? "--" : limit.ToString()) + Environment.NewLine;
                }
                else
                {
                    this.logTextBox.Text += "Failed to open a sell order." + Environment.NewLine;
                }
            }

            this.PutInterval(this.marketSellButton);
        }

        private static decimal Floor(decimal d, decimal step) => step * Math.Floor(d / step);

        private void PutInterval(Button button)
        {
            DispatcherTimer dt = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(3d) };

            button.IsEnabled = false;
            dt.Start();
            dt.Tick += (sender, e) =>
            {
                dt.Stop();

                button.IsEnabled = true;
            };
        }

        #endregion

        private void ShortcutListMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ShortcutListWindow slw = new ShortcutListWindow() { Owner = this };

            slw.Show();
        }
    }
}
