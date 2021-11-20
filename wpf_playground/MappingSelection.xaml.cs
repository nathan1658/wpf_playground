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
            }
        }


        public MappingSelection()
        {
            this.WindowState = WindowState.Maximized;

            InitializeComponent();
            this.DataContext = this;

            genPanelButtons(ref bcPanel, MappingEnum.BC);
            genPanelButtons(ref tcPanel, MappingEnum.TC);
            genPanelButtons(ref lcPanel, MappingEnum.LC);
            genPanelButtons(ref biPanel, MappingEnum.BI);

        }

        void genPanelButtons(ref StackPanel panel, MappingEnum mapping)
        {
            var practiceButton = new Button();
            practiceButton.Click += (s, e) =>
            {
                openWindowAndCloseThis(true, mapping);
            };
            practiceButton.Content = "Practice " + mapping.ToString();
            practiceButton.Margin = new Thickness(0, 0, 0, 10);

            var testButton = new Button();
            testButton.Click += (s, e) =>
            {
                openWindowAndCloseThis(false, mapping);
            };
            testButton.Content = "Test " + mapping.ToString();

            testButton.IsEnabled = !State.FinishedMappingList.Contains(mapping);
            panel.Children.Add(practiceButton);
            panel.Children.Add(testButton);

        }

        void openWindowAndCloseThis(bool practiceMode, MappingEnum mapping)
        {
            new MainWindow(practiceMode, mapping).Show();
            this.Close();
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
            MessageBox.Show("Program will exit now.", "Alert");
            this.Close();
        }
    }
}
