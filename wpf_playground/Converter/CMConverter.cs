using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace wpf_playground.Converter
{
    public class CMConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = double.Parse(parameter.ToString());
            return (double)new LengthConverter().ConvertFrom(CmToPx(val));
            //Get current screen raw dpi
            var dpi = ScreenInformations.RawDpi;
            var result = (Math.Sqrt(dpi) * 2.54 * val) + "px";
            return (double)new LengthConverter().ConvertFrom(result);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }


        private struct PixelUnitFactor
        {
            public const double Px = 1.0;
            public const double Inch = 96.0;
            public const double Cm = 37.7952755905512;
            public const double Pt = 1.33333333333333;
        }
        public double CmToPx(double cm)
        {
            return cm * PixelUnitFactor.Cm;
        }

        public double PxToCm(double px)
        {
            return px / PixelUnitFactor.Cm;
        }
    }
}
