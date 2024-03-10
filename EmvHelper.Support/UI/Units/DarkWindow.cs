using Jamesnet.Wpf.Controls;
using System.Windows;

namespace EmvHelper.Support.UI.Units
{
    public class DarkWindow : JamesWindow
    {
        static DarkWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DarkWindow),
                new FrameworkPropertyMetadata(typeof(DarkWindow)));
        }
    }
}
