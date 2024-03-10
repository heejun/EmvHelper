using EmvHelper.Support.UI.Units;
using Jamesnet.Wpf.Controls;
using System.Windows;

namespace EmvHelper
{
    internal class App : JamesApplication
    {
        protected override Window CreateShell()
        {
            return new DarkWindow();
        }
    }
}
