using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace wpf_playground
{
    public partial class PQCircle : MyBaseUserControl
    {
        public PQCircle()
        {
            InitializeComponent();
            this.DataContext = this;
        }


        public override void Enable()
        {
            //Do nothing if not triggered
            if (this.Triggered) return;
            this.Triggered = true;
        }

        public override void Disable()
        {
            this.Triggered = false;
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