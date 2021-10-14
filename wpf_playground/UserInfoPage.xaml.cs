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
        public UserInfoPageViewModel()
        {
             SoundDeviceList= DirectSoundOut.Devices.ToList();
        }

        public UserInfo UserInfo
        {
            get
            {
                return State.UserInfo;
            }
        }


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


        private DominantHandEnum dominantHandEnum = DominantHandEnum.Right;
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


        private List<DirectSoundDeviceInfo> _soundDeviceList;
        public List<DirectSoundDeviceInfo> SoundDeviceList
        {
            get
            {
                return _soundDeviceList;
            }
            set
            {
                _soundDeviceList = value;
                InformPropertyChanged("SoundDeviceList");
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
                InformPropertyChanged("BottomSpeakerHz");
            }

        }

        

        private DirectSoundDeviceInfo _selectedPQSoundDevice;
        public DirectSoundDeviceInfo SelectedPQSoundDevice
        {
            get { return _selectedPQSoundDevice; }
            set
            {
                _selectedPQSoundDevice = value;
                State.PQSpeaker = value;
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
                InformPropertyChanged("SelectedBottomSpeakerSoundDevice");
            }
        }

        private string _tactilePqHz = State.UserInfo.TactilePQHz.ToString();
        public string TactilePQHz
        {
            get { return _tactilePqHz; }
            set
            {

                _tactilePqHz = value;
                UserInfo.TactilePQHz = string.IsNullOrEmpty(value) ? 0 : int.Parse(value);
                InformPropertyChanged("TactilePQHz");
            }

        }

        private string _tactileTopSpeakerHz = State.UserInfo.TactileTopSpeakerHz.ToString();
        public string TactileTopSpeakerHz
        {
            get { return _tactileTopSpeakerHz; }
            set
            {

                _tactileTopSpeakerHz = value;
                UserInfo.TactileTopSpeakerHz = string.IsNullOrEmpty(value) ? 0 : int.Parse(value);
                InformPropertyChanged("TactileTopSpeakerHz");
            }

        }

        private string _tactilebottomSpeakerHz = State.UserInfo.TactileBottomSpeakerHz.ToString();
        public string TactileBottomSpeakerHz
        {
            get { return _tactilebottomSpeakerHz; }
            set
            {

                _tactilebottomSpeakerHz = value;
                UserInfo.TactileBottomSpeakerHz = string.IsNullOrEmpty(value) ? 0 : int.Parse(value);
                InformPropertyChanged("TactileBottomSpeakerHz");
            }

        }

        private DirectSoundDeviceInfo _selectedTactilePQSoundDevice;
        public DirectSoundDeviceInfo SelectedTactilePQSoundDevice
        {
            get { return _selectedTactilePQSoundDevice; }
            set
            {
                _selectedTactilePQSoundDevice = value;
                State.TactilePQSpeaker = value;
                InformPropertyChanged("SelectedTactilePQSoundDevice");
            }
        }

        private DirectSoundDeviceInfo _selectedTactileTopSpeakerSoundDevice;
        public DirectSoundDeviceInfo SelectedTactileTopSpeakerSoundDevice
        {
            get { return _selectedTactileTopSpeakerSoundDevice; }
            set
            {
                _selectedTactileTopSpeakerSoundDevice = value;
                State.TactileTopSpeaker = value;
                InformPropertyChanged("SelectedTactileTopSpeakerSoundDevice");
            }
        }

        private DirectSoundDeviceInfo _selectedTactileBottomSpeakerSoundDevice;
        public DirectSoundDeviceInfo SelectedTactileBottomSpeakerSoundDevice
        {
            get { return _selectedTactileBottomSpeakerSoundDevice; }
            set
            {
                _selectedTactileBottomSpeakerSoundDevice = value;
                State.TactileBottomSpeaker = value;
                InformPropertyChanged("SelectedTactileBottomSpeakerSoundDevice");
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
