using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace wpf_playground
{
    public class ScreenInformations
    {
        public static uint RawDpi { get; private set; }

        static ScreenInformations()
        {
            uint dpiX;
            uint dpiY;
            GetDpi(DpiType.RAW, out dpiX, out dpiY);
            RawDpi = dpiX;
        }

        /// <summary>
        /// Returns the scaling of the given screen.
        /// </summary>
        /// <param name="dpiType">The type of dpi that should be given back..</param>
        /// <param name="dpiX">Gives the horizontal scaling back (in dpi).</param>
        /// <param name="dpiY">Gives the vertical scaling back (in dpi).</param>
        private static void GetDpi(DpiType dpiType, out uint dpiX, out uint dpiY)
        {
            var point = new System.Drawing.Point(1, 1);
            var hmonitor = MonitorFromPoint(point, _MONITOR_DEFAULTTONEAREST);

            switch (GetDpiForMonitor(hmonitor, dpiType, out dpiX, out dpiY).ToInt32())
            {
                case _S_OK: return;
                case _E_INVALIDARG:
                    throw new ArgumentException("Unknown error. See https://msdn.microsoft.com/en-us/library/windows/desktop/dn280510.aspx for more information.");
                default:
                    throw new COMException("Unknown error. See https://msdn.microsoft.com/en-us/library/windows/desktop/dn280510.aspx for more information.");
            }
        }

        //https://msdn.microsoft.com/en-us/library/windows/desktop/dd145062.aspx
        [DllImport("User32.dll")]
        private static extern IntPtr MonitorFromPoint([In]System.Drawing.Point pt, [In]uint dwFlags);

        //https://msdn.microsoft.com/en-us/library/windows/desktop/dn280510.aspx
        [DllImport("Shcore.dll")]
        private static extern IntPtr GetDpiForMonitor([In]IntPtr hmonitor, [In]DpiType dpiType, [Out]out uint dpiX, [Out]out uint dpiY);

        const int _S_OK = 0;
        const int _MONITOR_DEFAULTTONEAREST = 2;
        const int _E_INVALIDARG = -2147024809;
    }

    /// <summary>
    /// Represents the different types of scaling.
    /// </summary>
    /// <seealso cref="https://msdn.microsoft.com/en-us/library/windows/desktop/dn280511.aspx"/>
    public enum DpiType
    {
        EFFECTIVE = 0,
        ANGULAR = 1,
        RAW = 2,
    }
}
