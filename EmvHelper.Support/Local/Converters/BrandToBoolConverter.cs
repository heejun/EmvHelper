using EmvHelper.Support.Local.Helpers;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace EmvHelper.Support.Local.Converters
{
    public class BrandToBoolConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CardBrandType cardBrand && parameter is CardBrandType targetBrand)
            {
                return cardBrand == targetBrand;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isSelected && parameter is CardBrandType targetBrand)
            {
                if (isSelected)
                {
                    return targetBrand;
                }
            }

            throw new NotSupportedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
