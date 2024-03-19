using EmvHelper.Forms.Local.ViewModels;
using EmvHelper.Forms.UI.Views;
using EmvHelper.Main.Local.ViewModels;
using EmvHelper.Main.UI.Views;
using Jamesnet.Wpf.Global.Location;

namespace EmvHelper.Properties
{
    internal class WireDataContext : ViewModelLocationScenario
    {
        protected override void Match(ViewModelLocatorCollection items)
        {
            items.Register<MainContent, MainContentViewModel>();
            items.Register<MainWindow, MainViewModel>();
        }
    }
}
