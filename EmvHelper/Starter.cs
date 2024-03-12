using EmvHelper.Properties;
using System;

namespace EmvHelper
{
    internal class Starter
    {
        [STAThread]
        private static void Main(string[] args)
        {
            _ = new App()
                .AddInversionModule<HelperModules>()
                .AddWireDataContext<WireDataContext>()
                .Run();
        }
    }
}
