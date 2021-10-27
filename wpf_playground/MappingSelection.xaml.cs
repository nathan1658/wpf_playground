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

    class TestMapping
    {
        public SOAEnum SoaEnum;
        public bool VisualSignal;
        public bool AuditorySignal;
        public bool TactileSignal;

        public bool VisualPQ;
        public bool AuditoryPQ;
        public bool TactilePQ;

        public TestMapping(SOAEnum soa, int signal, int pq)
        {
            this.SoaEnum = soa;
            if (signal == 1)
            {
                VisualSignal = true;
            }
            if (signal == 2)
            {
                AuditorySignal = true;
            }
            if (signal == 3)
            {
                VisualSignal = true;
                AuditorySignal = true;
            }
            if (signal == 4)
            {
                TactileSignal = true;

            }
            if (signal == 5)
            {
                VisualSignal = true;
                TactileSignal = true;
            }
            if (signal == 6)
            {
                AuditorySignal = true;
                TactileSignal = true;
            }
            if (signal == 7)
            {
                VisualSignal = true;
                AuditorySignal = true;
                TactileSignal = true;
            }
        }
    }


    /// <summary>
    /// Interaction logic for MappingSelection.xaml
    /// </summary>
    public partial class MappingSelection : Window, INotifyPropertyChanged
    {



        public MappingSelection()
        {
            this.WindowState = WindowState.Maximized;

            InitializeComponent();
            this.DataContext = this;

            //genPanelButtons(ref bcPanel, MappingEnum.BC);
            //genPanelButtons(ref tcPanel, MappingEnum.TC);
            //genPanelButtons(ref lcPanel, MappingEnum.LC);
            //genPanelButtons(ref biPanel, MappingEnum.BI);

        }

        //void genPanelButtons(ref StackPanel panel, MappingEnum mapping)
        //{
        //    var practiceButton = new Button();
        //    practiceButton.Click += (s, e) =>
        //    {
        //        openWindowAndCloseThis(true, mapping);
        //    };
        //    practiceButton.Content = "Practice " + mapping.ToString();
        //    practiceButton.Margin = new Thickness(0, 0, 0, 10);


        //    panel.Children.Add(practiceButton);

        //    if (State.FinishedTestMappingList.Contains(mapping))
        //    {
        //        var testButton = new Button();
        //        testButton.Click += (s, e) =>
        //        {
        //            openWindowAndCloseThis(false, mapping);
        //        };
        //        testButton.Content = "Test " + mapping.ToString();

        //        testButton.IsEnabled = !State.FinishedMappingList.Contains(mapping);
        //        panel.Children.Add(testButton);
        //    }

        //}

        void openWindowAndCloseThis(bool practiceMode, MappingEnum mapping)
        {
            new MainWindow(practiceMode, mapping).Show();
            this.Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;


        void InformPropertyChanged([CallerMemberName] string propName = "")
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propName));
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            new UserInfoPage().Show();
            this.Close();
        }

        private void practiceBtnClick(object sender, RoutedEventArgs e)
        {
            openWindowAndCloseThis(true, State.SelectedMapping);
        }

        private void testBtnClick(object sender, RoutedEventArgs e)
        {
            openWindowAndCloseThis(false, State.SelectedMapping);
        }



        private bool _signalVisualChecked = UserInfo.SignalVisualChecked;
        public bool SignalVisualChecked
        {
            get { return _signalVisualChecked; }
            set
            {
                _signalVisualChecked = value;
                UserInfo.SignalVisualChecked = value;
                InformPropertyChanged("FormValid");
                InformPropertyChanged("SignalVisualChecked");
            }
        }

        private bool _signalAuditoryChecked = UserInfo.SignalAuditoryChecked;
        public bool SignalAuditoryChecked
        {
            get { return _signalAuditoryChecked; }
            set
            {
                _signalAuditoryChecked = value;
                UserInfo.SignalAuditoryChecked = value;
                InformPropertyChanged("FormValid");
                InformPropertyChanged("SignalAuditoryChecked");
            }
        }

        private bool _signalTactileChecked = UserInfo.SignalTactileChecked;
        public bool SignalTactileChecked
        {
            get { return _signalTactileChecked; }
            set
            {
                _signalTactileChecked = value;
                UserInfo.SignalTactileChecked = value;
                InformPropertyChanged("FormValid");
                InformPropertyChanged("SignalTactileChecked");
            }
        }

        private bool _pqVisualChecked = UserInfo.PQVisualChecked;
        public bool PQVisualChecked
        {
            get { return _pqVisualChecked; }
            set
            {
                _pqVisualChecked = value;
                UserInfo.PQVisualChecked = value;
                InformPropertyChanged("FormValid");
                InformPropertyChanged("PQVisualChecked");
            }
        }

        private bool _pqAuditoryChecked = UserInfo.PQAuditoryChecked;

        public bool PQAuditoryChecked
        {
            get { return _pqAuditoryChecked; }
            set
            {
                _pqAuditoryChecked = value;
                UserInfo.PQAuditoryChecked = value;
                InformPropertyChanged("FormValid");
                InformPropertyChanged("PQAuditoryChecked");
            }
        }

        private bool _pqTactileChecked = UserInfo.PQTactileChecked;

        public bool PQTactileChecked
        {
            get { return _pqTactileChecked; }
            set
            {
                _pqTactileChecked = value;
                UserInfo.PQTactileChecked = value;
                InformPropertyChanged("FormValid");
                InformPropertyChanged("PQTactileChecked");
            }
        }

        private SOAEnum soaEnum = UserInfo.SOA;
        public SOAEnum SOAEnum
        {
            get { return soaEnum; }
            set
            {
                soaEnum = value;
                UserInfo.SOA = value;
                InformPropertyChanged("SOAEnum");
            }
        }


        public static UserInfo UserInfo
        {
            get
            {
                return State.UserInfo;
            }
        }

        public bool AtLeastOneSignalChecked
        {
            get
            {
                return SignalAuditoryChecked || SignalVisualChecked || SignalTactileChecked;
            }
        }

        public bool AtLeastOnePQChecked
        {
            get
            {
                return PQVisualChecked || PQAuditoryChecked || PQTactileChecked;
            }
        }


        public bool FormValid
        {
            get
            {
                if (!AtLeastOnePQChecked) return false;
                if (!AtLeastOneSignalChecked) return false;
                return true;
            }
        }

    }
}
