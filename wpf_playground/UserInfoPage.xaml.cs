using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using wpf_playground.Model;

namespace wpf_playground
{
    /// <summary>
    /// Interaction logic for UserInfoPage.xaml
    /// </summary>
    public partial class UserInfoPage : Window
    {
        private WaveOut waveOut;
        public UserInfoPage()
        {
            InitializeComponent();
            var vm = new UserInfoPageViewModel();
            vm.CloseAction = () =>
            {
                this.Close();
            };
            this.DataContext = vm;
            versionText.Text = Assembly.GetExecutingAssembly().GetName().Version.ToString();


        }
        bool testAudio = false;
        private void PQButton_Click(object sender, RoutedEventArgs e)
        {

            testAudio = !testAudio;
            AudioHelper.Instance.play(State.PQSpeaker.Guid, 1000, testAudio);
        }

        private void TopSpeakerButton_Click(object sender, RoutedEventArgs e)
        {

            testAudio = !testAudio;
            AudioHelper.Instance.play(State.TopSpeaker.Guid, 1000, testAudio);
        }

        private void BottomSpeakerButton_Click(object sender, RoutedEventArgs e)
        {

            testAudio = !testAudio;
            AudioHelper.Instance.play(State.BottomSpeaker.Guid, 1000, testAudio);
        }


        private static readonly Regex _regex = new Regex("[^0-9]+"); //regex that matches disallowed text
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }
    }
    public class UserInfoPageViewModel : INotifyPropertyChanged
    {
        List<AuditoryTarget> aa;
        public UserInfoPageViewModel()
        {

            TestCommand = new DelegateCommand((val) =>
            {
                try
                {
                    MyBaseUserControl ctrl;
                    ctrl = aa[int.Parse(val.ToString())];
                    Dispatcher.CurrentDispatcher.Invoke(async () =>
                    {
                        ctrl.Enable();
                        await Task.Delay(1000);
                        ctrl.Disable();
                    });
                }
                catch (Exception)
                {

                }
            });
        }

        public UserInfo UserInfo
        {
            get
            {
                return State.UserInfo;
            }
        }

        AuditoryTarget TopLeftTarget { get; set; }
        AuditoryTarget TopRightTarget { get; set; }
        AuditoryTarget MiddleLeftTarget { get; set; }
        AuditoryTarget MiddleRightTarget { get; set; }
        AuditoryTarget BottomLeftTarget { get; set; }
        AuditoryTarget BottomRightTarget { get; set; }

        public ICommand TestCommand { get; set; }

        public bool IsDebugMode
        {
            get
            {
                return State.DebugMode;
            }
        }

        private GenderEnum genderEnum = GenderEnum.Male;

        public GenderEnum GenderEnum
        {
            get { return genderEnum; }
            set
            {
                genderEnum = value;
                UserInfo.Gender = value;
                InformPropertyChanged("GenderEnum");
            }
        }

        private GroupEnum groupEnum = GroupEnum.L01;
        public GroupEnum GroupEnum
        {
            get { return groupEnum; }
            set
            {
                groupEnum = value;
                UserInfo.Group = value;
                InformPropertyChanged("GroupEnum");
            }
        }


        private DominantHandEnum dominantHandEnum = DominantHandEnum.Left;
        public DominantHandEnum DominantHandEnum
        {
            get { return dominantHandEnum; }
            set
            {
                dominantHandEnum = value;
                UserInfo.DominantHand = value;
                InformPropertyChanged("DominantHandEnum");
            }
        }

        public bool FormValid
        {
            get
            {
                var list = new List<string>() { UserInfo.Name, UserInfo.SID, UserInfo.Age, };
                return !list.Any(x => string.IsNullOrEmpty(x));
            }
        }



        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                UserInfo.Name = value;
                InformPropertyChanged("FormValid");

            }
        }
        private string _sid;
        public string SID
        {
            get { return _sid; }
            set
            {
                _sid = value;
                UserInfo.SID = value;
                InformPropertyChanged("FormValid");

            }
        }

        private string _age;

        public string Age
        {
            get { return _age; }
            set
            {
                _age = value;
                UserInfo.Age = value;
                InformPropertyChanged("FormValid");
            }
        }

        private string _pqHz = State.UserInfo.PQHz.ToString();
        public string PQHz
        {
            get { return _pqHz; }
            set
            {

                _pqHz = value;
                UserInfo.PQHz = string.IsNullOrEmpty(value) ? 0 : int.Parse(value);
                updateHz();
                InformPropertyChanged("PQHz");
            }

        }

        private string _topSpeakerHz = State.UserInfo.TopSpeakerHz.ToString();
        public string TopSpeakerHz
        {
            get { return _topSpeakerHz; }
            set
            {

                _topSpeakerHz = value;
                UserInfo.TopSpeakerHz = string.IsNullOrEmpty(value) ? 0 : int.Parse(value);
                updateHz();
                InformPropertyChanged("TopSpeakerHz");
            }

        }

        private string _bottomSpeakerHz = State.UserInfo.BottomSpeakerHz.ToString();
        public string BottomSpeakerHz
        {
            get { return _bottomSpeakerHz; }
            set
            {

                _bottomSpeakerHz = value;
                UserInfo.BottomSpeakerHz = string.IsNullOrEmpty(value) ? 0 : int.Parse(value);
                updateHz();
                InformPropertyChanged("BottomSpeakerHz");
            }

        }

        void updateSpeakerList()
        {
            aa = new List<AuditoryTarget>
            {
             TopLeftTarget,
             TopRightTarget,
             BottomLeftTarget,
             BottomRightTarget,
             MiddleLeftTarget,
             MiddleRightTarget
            };
        }

        void updateHz()
        {
            for (int i = 0; i < aa.Count; i++)
            {
                if (aa[i] == null) continue;
                if(i==0||i==1)
                {
                    aa[i].Frequency = float.Parse(TopSpeakerHz);
                }
                if (i == 2 || i == 3)
                {
                    aa[i].Frequency = float.Parse(BottomSpeakerHz);
                }
                if (i == 4 || i ==5)
                {
                    aa[i].Frequency = float.Parse(PQHz);
                }
            }
        }

        public List<DirectSoundDeviceInfo> SoundDeviceList { get; set; } = DirectSoundOut.Devices.ToList();
        private DirectSoundDeviceInfo _selectedPQSoundDevice;

        public DirectSoundDeviceInfo SelectedPQSoundDevice
        {
            get { return _selectedPQSoundDevice; }
            set
            {
                _selectedPQSoundDevice = value;
                State.PQSpeaker = value;
                MiddleLeftTarget = new AuditoryTarget(value, State.UserInfo.PQHz, true);
                MiddleRightTarget = new AuditoryTarget(value, State.UserInfo.PQHz, false);
                updateSpeakerList();
                InformPropertyChanged("SelectedPQSoundDevice");
            }
        }

        private DirectSoundDeviceInfo _selectedTopSpeakerSoundDevice;

        public DirectSoundDeviceInfo SelectedTopSpeakerSoundDevice
        {
            get { return _selectedTopSpeakerSoundDevice; }
            set
            {
                _selectedTopSpeakerSoundDevice = value;
                State.TopSpeaker = value;

                TopLeftTarget = new AuditoryTarget(value, State.UserInfo.TopSpeakerHz, true);
                TopRightTarget = new AuditoryTarget(value, State.UserInfo.TopSpeakerHz, false);
                updateSpeakerList();
                InformPropertyChanged("SelectedTopSpeakerSoundDevice");
            }
        }

        private DirectSoundDeviceInfo _selectedBottomSpeakerSoundDevice;

        public DirectSoundDeviceInfo SelectedBottomSpeakerSoundDevice
        {
            get { return _selectedBottomSpeakerSoundDevice; }
            set
            {
                _selectedBottomSpeakerSoundDevice = value;
                State.BottomSpeaker = value;

                BottomLeftTarget = new AuditoryTarget(value, State.UserInfo.BottomSpeakerHz, true);
                BottomRightTarget = new AuditoryTarget(value, State.UserInfo.BottomSpeakerHz, false);
                updateSpeakerList();
                InformPropertyChanged("SelectedBottomSpeakerSoundDevice");
            }
        }



        private LevelEnum _levelEnum = LevelEnum.L50;

        public LevelEnum LevelEnum
        {
            get { return _levelEnum; }
            set
            {
                _levelEnum = value;
                UserInfo.Level = value;
                InformPropertyChanged("LevelEnum");
            }
        }

        private SignalModeEnum signalModeEnum = SignalModeEnum.Visual;

        public SignalModeEnum SignalModeEnum
        {
            get { return signalModeEnum; }
            set
            {
                signalModeEnum = value;
                UserInfo.SignalMode = value;
                InformPropertyChanged("SignalModeEnum");
            }
        }



        private PQModeEnum pQModeEnum = PQModeEnum.Visual;

        public PQModeEnum PQModeEnum
        {
            get { return pQModeEnum; }
            set
            {
                pQModeEnum = value;
                UserInfo.PQMode = value;
                InformPropertyChanged("PQModeEnum");
            }
        }

        private SOAEnum soaEnum = SOAEnum.Soa200;

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





        private ICommand _clickCommand;
        public ICommand ClickCommand
        {
            get
            {
                return _clickCommand ?? (_clickCommand = new CommandHandler(() =>
                {
                    CloseAction();
                }, () => true));
            }
        }
        public Action CloseAction { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void InformPropertyChanged([CallerMemberName] String propName = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
