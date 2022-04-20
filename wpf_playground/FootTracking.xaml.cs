using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace wpf_playground
{
    /// <summary>
    /// Interaction logic for FootTracking.xaml
    /// </summary>
    public partial class FootTracking : UserControl
    {

        double mainGridWidth = -1;
        double incrementalValue = -1;

        public double fRMS = 0;
        double f = 1;
        double fSumm = 0;


        public FootTracking()
        {
            InitializeComponent();

            redBar.Width = 0;
            blueBar.Width = 0;

            this.Loaded += FootTracking_Loaded;
            this.Unloaded += FootTracking_Unloaded;
        }

        private void FootTracking_Unloaded(object sender, RoutedEventArgs e)
        {
            this.stopped = true;
        }

        private void FootTracking_Loaded(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            window.KeyDown += FootTracking_KeyDown;
            window.KeyUp += FootTracking_KeyUp;

            mainGridWidth = mainGrid.ActualWidth;
            incrementalValue = mainGridWidth / 100;
        }

        private void FootTracking_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.NumPad2 || e.Key == Key.D2)
            {
                isPressed = false;

                holdVal = 0;
            }
        }

        private void FootTracking_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.NumPad2 || e.Key == Key.D2)
            {
                if (!isPressed)
                    holdVal = 0;

                isPressed = true;
            }
        }

        bool stopped = false;
        bool isPressed = false;
        bool inc = true;
        int holdVal = 0;
        int i = 0;
        Random r = new Random();


        public void start()
        {
            Task.Run(async () =>
            {
                while (!stopped)
                {

                    if (i++ == 5)
                    {
                        i = 0;
                        var val = r.NextDouble();
                        inc = (val > 0.5);
                    }

                    Dispatcher.Invoke(() =>
                    {
                        var currentWidth = redBar.Width;
                        if (inc)
                        {
                            if (currentWidth + incrementalValue <= mainGridWidth)
                                redBar.Width += incrementalValue;
                        }
                        else
                        {
                            if (currentWidth - incrementalValue >= 0)
                                redBar.Width -= incrementalValue;
                        }

                        if (isPressed)
                        {
                            ++holdVal;
                            if (blueBar.Width + holdVal <= mainGridWidth)
                            {
                                blueBar.Width += holdVal;
                            }
                        }
                        else
                        {
                            ++holdVal;
                            if (blueBar.Width - holdVal >= 0)
                            {
                                blueBar.Width -= holdVal;
                            }
                        }
                        double fDist = 0;
                        double fDivide = 0;
                        fDist = getfDistance();
                        fSumm += fDist;
                        fDivide = fSumm / f;
                        fRMS = Math.Round(Math.Sqrt(fDivide), 5);

                        f++;

                        debugVal.Text = getfDistance() + ":" + fRMS.ToString();
                    });
                    await Task.Delay(100);
                }
            });
        }


        public double getfDistance()
        {
            return Dispatcher.Invoke<double>(() =>
              {
                  var tmp = blueBar.Width - redBar.Width;
                  return Math.Round(Math.Pow(tmp, 2), 2);
              });
        }

    }
}
