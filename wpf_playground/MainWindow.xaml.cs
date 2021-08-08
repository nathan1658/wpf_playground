using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        List<MyBaseUserControl> circleList = new List<MyBaseUserControl>();
        static Random rnd = new Random(DateTime.Now.Millisecond);
        public MainWindow()
        {
            InitializeComponent();
            circleList = new List<MyBaseUserControl>
            {
                this.circle1,
                this.circle2,
                this.circle3,
                this.circle4,
            };
            triggerControl();
            Task.Run(() =>
            {
                Stopwatch sw1 = new Stopwatch();
                sw1.Start();
                while (true)
                {
                    Dispatcher.Invoke(() =>
                    {
                        counter.Text = sw1.ElapsedMilliseconds.ToString();
                    });
                }
            });
        }


        void triggerControl()
        {
            Task.Run(async () =>
            {
                int lastIndex = -1;
                while (true)
                {
                    await Task.Delay(1000);

                    int index = rnd.Next(circleList.Count);
                    //Prevent same index show continusly
                    while (lastIndex == index)
                    {
                        index = rnd.Next(circleList.Count);
                    }
                    lastIndex = index;
                    var targetControl = (circleList[index]);
                    if (!targetControl.Triggered)
                        Dispatcher.Invoke(() =>
                        {
                            targetControl.Trigger(1000);
                        });
                }
            });
        }

    }
}
