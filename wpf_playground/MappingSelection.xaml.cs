using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using wpf_playground.Model;

namespace wpf_playground
{
    /// <summary>
    /// Interaction logic for MappingSelection.xaml
    /// </summary>
    public partial class MappingSelection : Window, INotifyPropertyChanged
    {


        private MappingEnum _mappingEnum;

        public MappingEnum MappingEnum
        {
            get { return _mappingEnum; }
            set
            {
                _mappingEnum = value;
                FormValid = value != MappingEnum.NONE;
                State.UserInfo.Mapping = value;
            }
        }


        public MappingSelection()
        {
            InitializeComponent();
            this.DataContext = this;
            BCRadioButton.IsEnabled = !State.FinishedMappingList.Contains(MappingEnum.BC);
            TCRadioButton.IsEnabled = !State.FinishedMappingList.Contains(MappingEnum.TC);
            LCRadioButton.IsEnabled = !State.FinishedMappingList.Contains(MappingEnum.LC);
            BIRadioButton.IsEnabled = !State.FinishedMappingList.Contains(MappingEnum.BI);

        }

        public event PropertyChangedEventHandler PropertyChanged;

        private bool _formValid;
        public bool FormValid
        {
            get { return _formValid; }
            set
            {
                _formValid = value;
                InformPropertyChanged();
            }
        }

        void InformPropertyChanged([CallerMemberName] string propName = "")
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propName));
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow(false).Show();
            this.Close();
        }
    }
}
