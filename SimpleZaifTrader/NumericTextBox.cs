using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SimpleZaifTrader
{
    public class NumericTextBox : TextBox
    {
        static NumericTextBox()
        {
            InputMethod.IsInputMethodEnabledProperty.OverrideMetadata(typeof(NumericTextBox), new FrameworkPropertyMetadata(false));
        }

        public NumericTextBox()
        {
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.D0:
                case Key.D1:
                case Key.D2:
                case Key.D3:
                case Key.D4:
                case Key.D5:
                case Key.D6:
                case Key.D7:
                case Key.D8:
                case Key.D9:
                case Key.NumPad0:
                case Key.NumPad1:
                case Key.NumPad2:
                case Key.NumPad3:
                case Key.NumPad4:
                case Key.NumPad5:
                case Key.NumPad6:
                case Key.NumPad7:
                case Key.NumPad8:
                case Key.NumPad9:
                    if ((Keyboard.Modifiers & ModifierKeys.Shift) > 0)
                    {
                        e.Handled = true;
                    }

                    break;
                case Key.Back:
                case Key.Delete:
                case Key.OemPeriod:
                case Key.Tab:
                case Key.Left:
                case Key.Right:
                    break;
                case Key.C:
                    if (!((Keyboard.Modifiers & ModifierKeys.Control) > 0))
                    {
                        e.Handled = true;
                    }

                    break;
                case Key.V:
                    if (!((Keyboard.Modifiers & ModifierKeys.Control) > 0))
                    {
                        e.Handled = true;
                    }
                    else
                    {
                        string str = Clipboard.GetText();
                        decimal d;

                        if (!decimal.TryParse(str, out d))
                        {
                            e.Handled = true;
                        }
                    }

                    break;
                default:
                    e.Handled = true;

                    break;
            }

            base.OnKeyDown(e);
        }

        protected override void OnPreviewMouseRightButtonUp(MouseButtonEventArgs e) => e.Handled = true;
    }
}
