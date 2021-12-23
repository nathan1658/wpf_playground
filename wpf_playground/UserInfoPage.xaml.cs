using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using wpf_playground.Model;

namespace wpf_playground
{
    /// <summary>
    /// Interaction logic for UserInfoPage.xaml
    /// </summary>
    public partial class UserInfoPage : Window
    {


        void initList()
        {
            State.TestMappingList = new List<TestMapping>();
            //Soa200
            List<SOAEnum> soaList = new List<SOAEnum> { SOAEnum.Soa200, SOAEnum.Soa600, SOAEnum.Soa1000 };
            foreach (var soa in soaList)
            {
                for (int i = 1; i < 4; i++)
                {
                    for (int j = 1; j < 4; j++)
                    {
                        State.TestMappingList.Add(new TestMapping(soa, i, j));
                    }
                }
            }
        }

        public UserInfoPage()
        {
            this.WindowState = WindowState.Maximized;
            InitializeComponent();
            var vm = new UserInfoPageViewModel();
            vm.CloseAction = () =>
            {

                //Reset timer and clean up test history
                State.TestResultList = new List<TestResult>();
                if (State.TestStopwatch.IsRunning) State.TestStopwatch.Reset();

                new MappingSelection().Show();
                this.Close();
            };
            this.DataContext = vm;
            initList();


            comportList.ItemsSource = ComHelper.GetComportList();
            comportList.SelectionChanged += ComportList_SelectionChanged;
            versionText.Text = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        }

        private void ComportList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            State.SelectedCOMPort = comportList.SelectedItem as string;


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
        private const string CONFIG_FILE_NAME = "config.json";


        //try to load config from json file and apply those settings
        void loadConfig()
        {

            try
            {
                var jsonPayload = File.ReadAllText($"./{CONFIG_FILE_NAME}");
                var config = Newtonsoft.Json.JsonConvert.DeserializeObject<Config>(jsonPayload);


                Action<SpeakerConfig, string, string> updateProperty = new Action<SpeakerConfig, string, string>((speakerConfig, hzPropName, guidPropName) =>
                {
                    if (speakerConfig == null) return;
                    if (!string.IsNullOrEmpty(speakerConfig.Hz))
                    {
                        var hzProp = this.GetType().GetProperty(hzPropName);
                        hzProp.SetValue(this, speakerConfig.Hz);
                    }

                    if (!string.IsNullOrEmpty(speakerConfig.SpeakerGuid))
                    {
                        var guidProp = this.GetType().GetProperty(guidPropName);
                        var targetDevice = SoundDeviceList.FirstOrDefault(x => x.Guid.ToString() == speakerConfig.SpeakerGuid);
                        guidProp.SetValue(this, targetDevice);
                    }
                });

                updateProperty(config.TopAuditorySpeaker, nameof(TopSpeakerHz), nameof(SelectedTopSpeakerSoundDevice));
                updateProperty(config.PQAuditorySpeaker, nameof(PQHz), nameof(SelectedPQSoundDevice));
                updateProperty(config.BottomAuditorySpeaker, nameof(BottomSpeakerHz), nameof(SelectedBottomSpeakerSoundDevice));

                updateProperty(config.TopTactileSpeaker, nameof(TactileTopSpeakerHz), nameof(SelectedTactileTopSpeakerSoundDevice));
                updateProperty(config.PQTactileSpeaker, nameof(TactilePQHz), nameof(SelectedTactilePQSoundDevice));
                updateProperty(config.BottomTactileSpeaker, nameof(TactileBottomSpeakerHz), nameof(SelectedTactileBottomSpeakerSoundDevice));

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Failed to load config.");
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        public UserInfoPageViewModel()
        {
            SoundDeviceList = DirectSoundOut.Devices.ToList();
            loadConfig();
        }



        public bool IsDebugMode
        {
            get
            {
                return State.DebugMode;
            }
        }

        private GenderEnum genderEnum = UserInfo.Gender;
        public GenderEnum GenderEnum
        {
            get { return genderEnum; }
            set
            {
                genderEnum = value;
                UserInfo.Gender = value;
                InformPropertyChanged("GenderEnum");
                InformPropertyChanged("FormValid");
            }
        }


        private DominantHandEnum dominantHandEnum = UserInfo.DominantHand;
        public DominantHandEnum DominantHandEnum
        {
            get { return dominantHandEnum; }
            set
            {
                dominantHandEnum = value;
                UserInfo.DominantHand = value;
                InformPropertyChanged("FormValid");
                InformPropertyChanged("DominantHandEnum");
            }
        }

        public bool FormValid
        {
            get
            {
                var list = new List<string>() { UserInfo.Name, UserInfo.SID, UserInfo.Age, };
                var speakerList = new List<DirectSoundDeviceInfo> { SelectedTopSpeakerSoundDevice, SelectedPQSoundDevice, SelectedBottomSpeakerSoundDevice, SelectedTactileTopSpeakerSoundDevice, SelectedTactilePQSoundDevice, SelectedTactileBottomSpeakerSoundDevice };
                if (speakerList.Any(x => x == null || x.Guid == null)) return false;
                //if (!AtLeastOnePQChecked) return false;
                //if (!AtLeastOneSignalChecked) return false;
                return !list.Any(x => string.IsNullOrEmpty(x));
            }
        }


        public static UserInfo UserInfo
        {
            get
            {
                return State.UserInfo;
            }
        }

        private string _name = UserInfo.Name;
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
        private string _sid = UserInfo.SID;
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


        private string _age = UserInfo.Age;
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
                InformPropertyChanged("FormValid");
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
                InformPropertyChanged("FormValid");

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
                InformPropertyChanged("FormValid");
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
                InformPropertyChanged("FormValid");
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
                InformPropertyChanged("FormValid");
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
                InformPropertyChanged("FormValid");
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
                InformPropertyChanged("FormValid");
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
                InformPropertyChanged("FormValid");
                InformPropertyChanged("SelectedTactileBottomSpeakerSoundDevice");
            }
        }



        private LevelEnum _levelEnum = UserInfo.Level;

        public LevelEnum LevelEnum
        {
            get { return _levelEnum; }
            set
            {
                _levelEnum = value;
                UserInfo.Level = value;
                InformPropertyChanged("LevelEnum");
                InformPropertyChanged("FormValid");

            }
        }



        private MappingEnum _selectedMapping = State.SelectedMapping;

        public MappingEnum SelectedMapping
        {
            get { return _selectedMapping; }
            set
            {
                _selectedMapping = value;
                InformPropertyChanged("SelectedMapping");
                InformPropertyChanged("FormValid");
                State.SelectedMapping = value;
            }
        }



        void saveConfig()
        {
            try
            {
                var config = new Config();

                Action<string, string, DirectSoundDeviceInfo> updateConfig = new Action<string, string, DirectSoundDeviceInfo>((configName, hzValue, deviceInfo) =>
                {
                    var configProp = config.GetType().GetProperty(configName);
                    configProp.SetValue(config, new SpeakerConfig
                    {
                        Hz = hzValue,
                        SpeakerGuid = deviceInfo?.Guid.ToString()
                    });
                });

                updateConfig(nameof(Config.TopAuditorySpeaker), TopSpeakerHz, SelectedTopSpeakerSoundDevice);
                updateConfig(nameof(Config.PQAuditorySpeaker), PQHz, SelectedPQSoundDevice);
                updateConfig(nameof(Config.BottomAuditorySpeaker), BottomSpeakerHz, SelectedBottomSpeakerSoundDevice);

                updateConfig(nameof(Config.TopTactileSpeaker), TactileTopSpeakerHz, SelectedTactileTopSpeakerSoundDevice);
                updateConfig(nameof(Config.PQTactileSpeaker), TactilePQHz, SelectedTactilePQSoundDevice);
                updateConfig(nameof(Config.BottomTactileSpeaker), TactileBottomSpeakerHz, SelectedTactileBottomSpeakerSoundDevice);

                //Write it to file
                var jsonPayload = Newtonsoft.Json.JsonConvert.SerializeObject(config);
                File.WriteAllText($"./{CONFIG_FILE_NAME}", jsonPayload);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error when writing config file.");
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

        }




        private ICommand _clickCommand;
        public ICommand ClickCommand
        {
            get
            {
                return _clickCommand ?? (_clickCommand = new CommandHandler(() =>
                {
                    saveConfig();

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
