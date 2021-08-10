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
    /// Interaction logic for Circle.xaml
    /// </summary>
    public partial class Circle : MyBaseUserControl
    {
        public Circle()
        {
            InitializeComponent();
            this.DataContext = this;

        }

        public override void Trigger(int delayInMS)
        {

            //Do nothing if not triggered
            if (this.Triggered) return;
            this.Triggered = true;
            //this.control.Fill = new SolidColorBrush(Colors.Red);
            Task.Delay(delayInMS).ContinueWith((obj) =>
            {
                Dispatcher.Invoke(() =>
                {
                    //this.control.Fill = new SolidColorBrush(Colors.White);
                });
                this.Triggered = false;
            });
        }
        public override bool Click()
        {
            if (this.Triggered)
            {
                this.Triggered = false;
                return true;
            }
            return false;
        }
    }
}
