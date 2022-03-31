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


        public FootTracking()
        {
            InitializeComponent();

            redBar.Width = 0;
            blueBar.Width = 0;

            this.Loaded += FootTracking_Loaded;
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
            if (e.Key == Key.Space)
            {
                isPressed = false;

                holdVal = 0;
            }
        }

        private void FootTracking_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                if (!isPressed)
                    holdVal = 0;

                isPressed = true;
            }
        }

        public void stop()
        {
            this.stopped = true;
        }

        bool stopped = false;
        bool isPressed = false;
        bool inc = true;
        int holdVal = 0;
        int i = 0;
        Random r = new Random();

        int f;
        double fsumm;

        public void start()
        {
            Task.Run(async () =>
            {
                while (true && !stopped)
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
                        double fDist = Math.Pow(blueBar.Width - redBar.Width,2);
                        fsumm += fDist;
                        double fdivide = fsumm / f++;
                        double fRMS = Math.Sqrt(fdivide);


                        debugVal.Text = fRMS.ToString();
                    });
                    await Task.Delay(100);
                }
            });
        }

    }
}
