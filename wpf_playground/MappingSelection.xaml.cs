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
using System.Windows.Shapes;
using wpf_playground.Model;

namespace wpf_playground
{
    /// <summary>
    /// Interaction logic for MappingSelection.xaml
    /// </summary>
    public partial class MappingSelection : Window
    {


        private MappingEnum _mappingEnum;

        public MappingEnum MappingEnum
        {
            get { return _mappingEnum; }
            set { _mappingEnum = value;

                State.UserInfo.Mapping = value; 
            }
        }


        public MappingSelection()
        {
            InitializeComponent();
            this.DataContext = this;
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
