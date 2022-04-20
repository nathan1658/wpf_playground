using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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




        void outputCSV()
        {

            //Write to CSV
            var mappingDict = new Dictionary<string, string>();
            mappingDict.Add("Name", nameof(UserInfo.Name));
            mappingDict.Add("SID", nameof(UserInfo.SID));
            mappingDict.Add("Age", nameof(UserInfo.Age));
            mappingDict.Add("Gender", nameof(UserInfo.Gender));
            mappingDict.Add("DominantHand", nameof(UserInfo.DominantHand));
            mappingDict.Add("Level", nameof(UserInfo.Level));
            mappingDict.Add("VisualSignalEnabled", nameof(UserInfo.SignalVisualChecked));
            mappingDict.Add("AuditorySignalEnabled", nameof(UserInfo.SignalAuditoryChecked));
            mappingDict.Add("TactileSignalEnabled", nameof(UserInfo.SignalTactileChecked));
            mappingDict.Add("VisualPQEnabled", nameof(UserInfo.PQVisualChecked));
            mappingDict.Add("AuditoryPQEnabled", nameof(UserInfo.PQAuditoryChecked));
            mappingDict.Add("TactilePQEnabled", nameof(UserInfo.PQTactileChecked));
            mappingDict.Add("SOA", nameof(UserInfo.SOA));
            mappingDict.Add("Mapping", "");

            var historyMappingDict = new Dictionary<string, string>();
            historyMappingDict.Add("HistoryType", nameof(ExperimentLog.HistoryType));
            historyMappingDict.Add("SignalIndex", nameof(ExperimentLog.SignalIndex));
            historyMappingDict.Add("ButtonPositionIndex", nameof(ExperimentLog.ButtonPositionIndex));
            historyMappingDict.Add("PQPositionIndex", nameof(ExperimentLog.PQPositionIndex));
            historyMappingDict.Add("ElapsedTime", nameof(ExperimentLog.ElapsedTime));
            historyMappingDict.Add("ReactionTime", nameof(ExperimentLog.ReactionTime));
            historyMappingDict.Add("Distance", nameof(ExperimentLog.Distance));
            historyMappingDict.Add("FDistance", nameof(ExperimentLog.FDistance));
            historyMappingDict.Add("FRMS", nameof(ExperimentLog.FRms));
            historyMappingDict.Add("ClickState", nameof(ExperimentLog.ClickState));
            historyMappingDict.Add("Delay", nameof(ExperimentLog.Delay));


            var headers = new List<String>();
            //concat the headers
            foreach (var kvp in mappingDict)
            {
                headers.Add(kvp.Key);
            }
            foreach (var kvp in historyMappingDict)
            {
                headers.Add(kvp.Key);
            }

            var csvOutput = String.Join(",", headers) + "\n";
            for (int i = 0; i < State.TestResultList.Count; i++)
            {
                var testResult = State.TestResultList[i];
                foreach (var clickHistory in testResult.ClickHistoryList)
                {
                    var tmpList = new List<string>();
                    foreach (var kvp in mappingDict)
                    {

                        //Hardcode here for mapping..
                        if (kvp.Key == "Mapping")
                        {
                            tmpList.Add(testResult.Mapping.ToString());
                        }
                        else
                        {
                            //resolve it via reflection
                            var val = GetPropValue(testResult.UserInfo, kvp.Value);
                            tmpList.Add(val.ToString());
                        }
                    }


                    foreach (var kvp in historyMappingDict)
                    {
                        //resolve it via reflection
                        var val = GetPropValue(clickHistory, kvp.Value);
                        tmpList.Add(val.ToString());
                    }
                    csvOutput += String.Join(",", tmpList) + "\n";
                }
            }
            if (!Directory.Exists("./output"))
                Directory.CreateDirectory("output");
            var fileName = $"output/{ DateTime.Now.ToString("yyyyMMddHHmmss") }.csv";
            File.WriteAllText(fileName, csvOutput);
            MessageBox.Show($"Saved test result to {fileName}");
        }


        public static object GetPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }

        void openWindowAndCloseThis(bool practiceMode, MappingEnum mapping)
        {
            new MainWindow(practiceMode, mapping, TestMapping).Show();
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
            outputCSV();
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

        public TestMapping TestMapping
        {
            get
            {
                return State.TestMappingList.FirstOrDefault(x =>
                    x.SOA == soaEnum &&
                    x.VisualSignal == SignalVisualChecked &&
                    x.AuditorySignal == SignalAuditoryChecked &&
                    x.TactileSignal == SignalTactileChecked &&
                    x.VisualPQ == PQVisualChecked &&
                    x.AuditoryPQ == PQAuditoryChecked &&
                    x.TactilePQ == PQTactileChecked
                );
            }
        }

        public bool AllTestCompleted
        {
            get
            {
                return State.TestMappingList.All(x => x.TestDone == true);
            }
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
                InformPropertyChanged("TestMapping");
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
                InformPropertyChanged("TestMapping");
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
                InformPropertyChanged("TestMapping");
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
                InformPropertyChanged("TestMapping");
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
                InformPropertyChanged("TestMapping");
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
                InformPropertyChanged("TestMapping");
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
                InformPropertyChanged("TestMapping");
                InformPropertyChanged("SOAEnum");
            }
        }


        private Boolean _footTrackingEnabled = State.EnableFootTracking;

        public Boolean FootTrackingEnabled
        {
            get
            {
                return _footTrackingEnabled;
            }
            set
            {
                _footTrackingEnabled = value;
                State.EnableFootTracking = value;
                InformPropertyChanged("FootTrackingEnabled");

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
