using Jamesnet.Wpf.Controls;
using System.Windows;
using EmvHelper.Forms.UI.Views;

namespace EmvHelper
{
    internal class App : JamesApplication
    {
        protected override Window CreateShell()
        {
            return new MainWindow();
        }
    }
}
