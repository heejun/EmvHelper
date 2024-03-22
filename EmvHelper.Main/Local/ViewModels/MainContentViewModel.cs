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
        private string _rawData = "56 69 56 4F 74 65 63 68 32 00 02 0A 00 22 27 90 00 01 9F 1E 08 37 33 31 54 38 32 36 33 9F 41 04 00 00 04 18 DF EE 4C 01 00 9F 2A 00 DF EC 1C 00 7F B6";

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
