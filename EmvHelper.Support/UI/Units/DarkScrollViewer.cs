using System.Windows;
using System.Windows.Controls;

namespace EmvHelper.Support.UI.Units
{
    public class DarkScrollViewer : ScrollViewer
    {
        static DarkScrollViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DarkScrollViewer),
                new FrameworkPropertyMetadata(typeof(DarkScrollViewer)));
        }
    }
}
