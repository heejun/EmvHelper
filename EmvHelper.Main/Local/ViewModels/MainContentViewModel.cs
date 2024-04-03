using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EmvHelper.Support.Local.Helpers;
using EmvHelper.Support.Local.Helpers.BerTlv;
using Jamesnet.Wpf.Mvvm;

namespace EmvHelper.Main.Local.ViewModels
{
    public partial class MainContentViewModel : ObservableBase
    {
        private readonly HjParser _vivopayParser;

        public MainContentViewModel(HjParser vivopayParser)
        {
            _vivopayParser = vivopayParser;
        }

        private static readonly string FailureResponse = "56 69 56 4F 74 65 63 68 32 00 02 0A 00 22 27 90 00 01 9F 1E 08 37 33 31 54 38 32 36 33 9F 41 04 00 00 04 18 DF EE 4C 01 00 9F 2A 00 DF EC 1C 00 7F B6";
        private static readonly string SuccessResponse = "56 69 56 4F 74 65 63 68 32 00 02 23 01 CE 3A 42 34 37 36 31 37 33 31 30 30 30 30 30 30 30 31 39 5E 4C 33 54 45 53 54 2F 43 41 52 44 31 30 31 38 5E 33 31 31 32 32 30 31 31 31 32 38 30 31 30 30 38 37 30 30 33 30 30 30 30 25 34 37 36 31 37 33 31 30 30 30 30 30 30 30 31 39 3D 33 31 31 32 32 30 31 31 31 32 38 30 38 37 30 30 30 30 33 31 00 50 0B 56 49 53 41 20 43 52 45 44 49 54 57 13 47 61 73 10 00 00 00 19 D3 11 22 01 11 28 08 70 00 03 1F 84 07 A0 00 00 00 03 10 10 9F 06 07 A0 00 00 00 03 10 10 9F 35 01 14 9F 39 01 91 9F 33 03 60 40 00 9F 37 04 6D 37 2E 26 9F 15 02 00 00 9F 4E 07 48 59 4F 53 55 4E 47 9F 40 05 EF 80 A0 50 00 9F 66 04 A7 80 40 00 9F 34 03 3F 00 00 FF EE 01 08 DF 30 01 00 DF 5B 01 08 DF EF 7F 01 00 DF 81 29 08 30 F0 F0 F0 A0 F0 FF 00 FF 81 05 4D 5F 20 0F 4C 33 54 45 53 54 2F 43 41 52 44 31 30 31 38 5F 2A 02 04 17 5F 2D 02 65 6E 82 02 00 80 9A 03 24 03 13 9C 01 01 9F 02 06 00 00 00 00 00 00 9F 03 06 00 00 00 00 00 00 9F 1A 02 04 17 9F 21 03 13 31 08 9F 5A 05 11 08 40 08 40 DF 81 16 16 1B 04 00 00 00 65 6E 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 9F 1E 08 37 33 31 54 38 32 36 33 9F 41 04 00 00 04 24 DF EE 4C 01 06 9F 2A 01 03 DF EC 1C 1E 08 03 00 04 FF 00 1B 04 00 00 00 FF 00 00 00 00 00 00 00 00 65 6E 00 00 00 00 00 00 FF 00 DF ED 4B 20 86 5E B3 B3 AF 96 13 BE 24 87 F8 D0 82 F2 F9 93 7F AE 25 DB 04 46 15 EB 7A 83 7E 6C A5 BB 97 AB DF ED 5D 0A 47 61 73 0B 03 F7 A0 F9 00 19 DF EC 0E 03 31 12 31 41 91";

        [ObservableProperty]
        private string _rawData = SuccessResponse;

        [ObservableProperty]
        private string _parsedData = string.Empty;

        [ObservableProperty]
        private bool _isTlvOnly = false;

        [ObservableProperty]
        private CardBrandType _cardBrandType = CardBrandType.Visa;

        [RelayCommand]
        private void Parse()
        {
            if (IsTlvOnly)
            {
                ICollection<Tlv> tlvs = HjParser.ParseTlv(RawData);
                ParsedData = TlvParser.ToString(tlvs, CardBrandType);
            }
            else
            {
                VivoMessage? vivoMessage = HjParser.ParseVivoMessage(RawData, VivoMessageType.Response);
                if (vivoMessage != null)
                {
                    ParsedData = vivoMessage.ToString(CardBrandType);
                }
                else
                {
                    ParsedData = "Invalid data";
                }
            }

        }

        [RelayCommand]
        private void Clear()
        {
            RawData = string.Empty;
            ParsedData = string.Empty;
        }
    }
}
