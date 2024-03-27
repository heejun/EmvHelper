using EmvHelper.Support.Local.Helpers;
using Prism.Ioc;
using Prism.Modularity;

namespace EmvHelper.Properties
{
    internal class HelperModules : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<HjParser>();
        }
    }
}
