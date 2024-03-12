using EmvHelper.Forms.Local.ViewModels;
using EmvHelper.Forms.UI.Views;
using Jamesnet.Wpf.Global.Location;

namespace EmvHelper.Properties
{
    internal class WireDataContext : ViewModelLocationScenario
    {
        protected override void Match(ViewModelLocatorCollection items)
        {
            items.Register<ParseWindow, ParseViewModel>();
        }
    }
}
