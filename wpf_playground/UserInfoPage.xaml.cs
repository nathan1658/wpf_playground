using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using SharpDX.IO;
using SharpDX.MediaFoundation;
using SharpDX.XAudio2;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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



        }
        bool testAudio = false;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            testAudio = !testAudio;
            AudioHelper.Instance.play(4000,testAudio);
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
