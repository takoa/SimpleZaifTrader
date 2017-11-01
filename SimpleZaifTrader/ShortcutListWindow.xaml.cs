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

namespace SimpleZaifTrader
{
    public partial class ShortcutListWindow : Window
    {
        public ShortcutListWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            CurrencyPairSettings currencyPairSettings = ((MainWindow)this.Owner).CurrencyPairSettings;

            sb.AppendFormat("Left Ctrl + 1/2/3: 価格+{0}/+{1}/+{2}", currencyPairSettings.PriceUnitStep, currencyPairSettings.PriceUnitStep * 10m, currencyPairSettings.PriceUnitStep * 100m).AppendLine();
            sb.AppendFormat("Left Ctrl + F1/F2/F3: 価格{0}/{1}/{2}", -currencyPairSettings.PriceUnitStep, currencyPairSettings.PriceUnitStep * -10m, currencyPairSettings.PriceUnitStep * -100m).AppendLine();
            sb.AppendFormat("Left Alt + 1/2/3: 数量+{0}/+{1}/+{2}", currencyPairSettings.AmountUnitStep, currencyPairSettings.AmountUnitStep * 10m, currencyPairSettings.AmountUnitStep * 100m).AppendLine();
            sb.AppendFormat("Left Alt + F1/F2/F3: 数量{0}/{1}/{2}", -currencyPairSettings.AmountUnitStep, currencyPairSettings.AmountUnitStep * -10m, currencyPairSettings.AmountUnitStep * -100m).AppendLine();
            sb.AppendLine("Left Ctrl + L: 最終価格を価格欄にコピー");
            sb.AppendLine("Left Ctrl + R: 価格をクリア");
            sb.AppendLine("Left Alt + R: 数量をクリア");
            this.textBlock.Text = sb.ToString();
        }
    }
}
