using Jamesnet.Wpf.Controls;
using Prism.Ioc;
using Prism.Modularity;
using EmvHelper.Main.UI.Views;

namespace EmvHelper.Properties
{
    internal class ViewModules : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IViewable, MainContent>("MainContent");
        }
    }
}
