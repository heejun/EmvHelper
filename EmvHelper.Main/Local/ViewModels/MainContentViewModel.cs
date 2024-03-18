using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EmvHelper.Support.Local.Helpers;
using Jamesnet.Wpf.Mvvm;

namespace EmvHelper.Main.Local.ViewModels
{
    public partial class MainContentViewModel : ObservableBase
    {
        private readonly VivopayParser _vivopayParser;

        public MainContentViewModel(VivopayParser vivopayParser)
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
            ParsedData = _vivopayParser.Parse(RawData);
        }
    }
}
