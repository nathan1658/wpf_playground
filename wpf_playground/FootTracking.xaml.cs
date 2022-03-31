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

        }


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
                            if (currentWidth + INC_VALUE < MAX_WIDTH)
                                redBar.Width += INC_VALUE;
                        }
                        else
                        {
                            if (currentWidth - INC_VALUE > 0)
                                redBar.Width -= INC_VALUE;
                        }
                    });
                    await Task.Delay(100);
                }
            });
        }

    }
}
