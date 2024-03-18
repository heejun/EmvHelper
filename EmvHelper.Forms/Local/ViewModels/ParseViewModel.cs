using CommunityToolkit.Mvvm.Input;
using Jamesnet.Wpf.Controls;
using Jamesnet.Wpf.Mvvm;
using Prism.Ioc;
using Prism.Regions;
using System.Windows;

namespace EmvHelper.Forms.Local.ViewModels
{
    public partial class ParseViewModel : ObservableBase, IViewLoadable
    {
        private readonly IContainerProvider _containerProvider;
        private readonly IRegionManager _regionManager;

        public ParseViewModel(IContainerProvider containerProvider, IRegionManager regionManager)
        {
            _containerProvider = containerProvider;
            _regionManager = regionManager;
        }

        public void OnLoaded(IViewable view)
        {
            IRegion mainRegion = _regionManager.Regions["MainRegion"];
            IViewable mainContent = _containerProvider.Resolve<IViewable>("MainContent");

            if (!mainRegion.Views.Contains(mainContent))
            {
                mainRegion.Add(mainContent);
            }

            mainRegion.Activate(mainContent);
        }

        [RelayCommand]
        private void Minimize(object value)
        {
            Window.GetWindow((UIElement)value).WindowState = WindowState.Minimized;
        }

        [RelayCommand]
        private void Maximize(object value)
        {
            Window.GetWindow((UIElement)value).WindowState = WindowState.Maximized;
        }

        [RelayCommand]
        private void Close(object value)
        {
            Window.GetWindow((UIElement)value).Close();
        }
    }
}
