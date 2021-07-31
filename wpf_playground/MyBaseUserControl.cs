using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace wpf_playground
{
    [TypeDescriptionProvider(typeof(AbstractControlDescriptionProvider<MyBaseUserControl, UserControl>))]
    public abstract class MyBaseUserControl : UserControl, INotifyPropertyChanged
    {

        public bool Triggered;

        public event PropertyChangedEventHandler PropertyChanged;

        public abstract void Trigger(int delayInMS);

    }
}
