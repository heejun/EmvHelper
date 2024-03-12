using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EmvHelper.Support.Local.Helpers;
using Jamesnet.Wpf.Mvvm;

namespace EmvHelper.Forms.Local.ViewModels
{
    public partial class ParseViewModel : ObservableBase
    {
        [ObservableProperty]
        private string _rawData = string.Empty;

        [ObservableProperty]
        private string _parsedData = string.Empty;

        [RelayCommand]
        private void Parse()
        {
            ParsedData = _vivopayParser.Parse(RawData);
        }

        private readonly VivopayParser _vivopayParser;

        public ParseViewModel(VivopayParser vivopayParser)
        {
            _vivopayParser = vivopayParser;
        }
    }
}
