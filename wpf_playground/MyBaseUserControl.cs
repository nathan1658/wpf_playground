﻿using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace wpf_playground
{
    [TypeDescriptionProvider(typeof(AbstractControlDescriptionProvider<MyBaseUserControl, UserControl>))]
    public abstract class MyBaseUserControl : UserControl, INotifyPropertyChanged
    {

        internal bool _triggered;

        public virtual bool Triggered
        {
            get
            {
                return _triggered;
            }
            set
            {
                _triggered = value;
                InformPropertyChanged("Triggered");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void InformPropertyChanged([CallerMemberName] string propName = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
        public abstract void Enable();
        public abstract void Disable();
        public abstract bool Click();

    }
}
