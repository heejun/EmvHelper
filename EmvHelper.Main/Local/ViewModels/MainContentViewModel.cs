using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EmvHelper.Support.Local.Helpers;
using Jamesnet.Wpf.Mvvm;

namespace EmvHelper.Main.Local.ViewModels
{
    public partial class MainContentViewModel : ObservableBase
    {
        private readonly VivoParser _vivopayParser;

        public MainContentViewModel(VivoParser vivopayParser)
        {
            _vivopayParser = vivopayParser;
        }

        [ObservableProperty]
        private string _rawData = string.Empty;

        [ObservableProperty]
        private string _parsedData = string.Empty;

        [RelayCommand]
        private void Parse()
        {
            VivoMessage? vivoMessage = VivoParser.Parse(RawData, VivoMessageType.Response);
            if (vivoMessage != null)
            {
                ParsedData = vivoMessage.ToString();
            }
        }
    }
}
