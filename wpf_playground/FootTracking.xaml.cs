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

        const int MAX_WIDTH = 800;
        const int INC_VALUE = 10;


        public FootTracking()
        {
            InitializeComponent();
            redBar.Width = 0;
            blueBar.Width = 0;


            this.KeyDown += FootTracking_KeyDown;
            this.Loaded += FootTracking_Loaded;
        }

        private void FootTracking_Loaded(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            window.KeyDown += FootTracking_KeyDown;
            window.KeyUp += FootTracking_KeyUp;
        }

        private void FootTracking_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                isPressed = false;
            }
        }

        private void FootTracking_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                isPressed = true;
            }
        }

        bool isPressed = false;
        bool inc = true;
        int i = 0;
        Random r = new Random();
        public void start()
        {
            Task.Run(async () =>
            {
                while (true)
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
                            if (currentWidth + INC_VALUE <= MAX_WIDTH)
                                redBar.Width += INC_VALUE;
                        }
                        else
                        {
                            if (currentWidth - INC_VALUE >= 0)
                                redBar.Width -= INC_VALUE;
                        }

                        if (isPressed)
                        {
                            if (blueBar.Width + INC_VALUE <= MAX_WIDTH)
                            {
                                blueBar.Width += INC_VALUE;
                            }
                        }
                        else
                        {
                            if (blueBar.Width - INC_VALUE >= 0)
                            {
                                blueBar.Width -= INC_VALUE;
                            }
                        }

                        debugVal.Text = (blueBar.Width - redBar.Width).ToString();
                    });
                    await Task.Delay(100);
                }
            });
        }

    }
}
