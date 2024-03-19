using CommunityToolkit.Mvvm.Input;
using Jamesnet.Wpf.Controls;
using Jamesnet.Wpf.Mvvm;
using Prism.Ioc;
using Prism.Regions;
using System.Windows;

namespace EmvHelper.Forms.Local.ViewModels
{
    public partial class MainViewModel : ObservableBase, IViewLoadable
    {
        private readonly IContainerProvider _containerProvider;
        private readonly IRegionManager _regionManager;

        public MainViewModel(IContainerProvider containerProvider, IRegionManager regionManager)
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
    }
}
