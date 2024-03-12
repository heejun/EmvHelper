using EmvHelper.Support.UI.Units;
using System.Windows;

namespace EmvHelper.Forms.UI.Views
{
    public class ParseWindow : DarkWindow
    {
        static ParseWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ParseWindow), new FrameworkPropertyMetadata(typeof(ParseWindow)));
        }
    }
}
