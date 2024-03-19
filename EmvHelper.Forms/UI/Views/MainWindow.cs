using EmvHelper.Support.UI.Units;
using System.Windows;

namespace EmvHelper.Forms.UI.Views
{
    public class MainWindow : DarkWindow
    {
        static MainWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MainWindow), new FrameworkPropertyMetadata(typeof(MainWindow)));
        }
    }
}
