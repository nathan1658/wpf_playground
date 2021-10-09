using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private void Button_Click(object sender, RoutedEventArgs e)
        {

            testAudio = !testAudio;
            AudioHelper.Instance.play(1000, testAudio);
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

        private string _hz = State.UserInfo.Hz.ToString();
        public string Hz
        {
            get { return _hz; }
            set
            {

                _hz = value;
                UserInfo.Hz = string.IsNullOrEmpty(value) ? 0 : int.Parse(value);
                InformPropertyChanged("Hz");
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
