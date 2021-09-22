using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using wpf_playground.Model;

namespace wpf_playground
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public UserInfo UserInfo
        {
            get
            {
                return State.UserInfo;
            }
        }
        List<MyBaseUserControl> circleList = new List<MyBaseUserControl>();
        static Random rnd = new Random(DateTime.Now.Millisecond);
        Stopwatch reactionSw = new Stopwatch();
        Random random = new Random();

        public event PropertyChangedEventHandler PropertyChanged;
        private void InformPropertyChanged([CallerMemberName] string propName = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
  

        private List<ClickHistory> clickHistoryList { get; set; } = new List<ClickHistory>();


        private ObservableCollection<int> _sequenceList = new ObservableCollection<int>();

        public ObservableCollection<int> SequenceList
        {
            get { return _sequenceList; }
            set
            {
                _sequenceList = value;
                InformPropertyChanged("SequenceList");
            }
        }

        public MainWindow()
        {

            new UserInfoPage().ShowDialog();
            this.DataContext = this;

            InitializeComponent();


            circleList = new List<MyBaseUserControl>
            {
                this.circle1,
                this.circle2,
                this.circle3,
                this.circle4,
            };

            triggerControl();
            Task.Run(() =>
            {
                Stopwatch sw1 = new Stopwatch();
                sw1.Start();
                while (true)
                {
                    Dispatcher.Invoke(() =>
                    {
                        elapsedMs = sw1.ElapsedMilliseconds;
                        counter.Text = reactionSw.ElapsedMilliseconds.ToString() + "ms";
                    });
                }
            });
            Loaded += MainWindow_Loaded;

            //Each button need to be press five times
            var pressCount = 5;
            for (int i = 0; i < pressCount; i++)
            {
                for (int j = 0; j < circleList.Count; j++)
                {
                    SequenceList.Add(j);
                }
            }
            //sequenceList = 0 1 2 3 0 1 2 3 0 1 2 3...... each button need to be press 5 times

            //shuffle it
            SequenceList = new ObservableCollection<int>(SequenceList.OrderBy(a => Guid.NewGuid()).ToList());

        }
        double elapsedMs = 0;




        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            window.KeyDown += MainWindow_KeyDown;        
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            bool isCorrect = false;
            //TODO switch by mapping

            int btnIndex = -1;
            switch (e.Key)
            {
                case System.Windows.Input.Key.NumPad7:
                    btnIndex = 0;
                    break;
                case System.Windows.Input.Key.NumPad9:
                    btnIndex = 1;
                    break;
                case System.Windows.Input.Key.NumPad1:
                    btnIndex = 2;
                    break;
                case System.Windows.Input.Key.NumPad3:
                    btnIndex = 3;
                    break;
            }

            if (btnIndex == -1)//Do nothing
                return;

            isCorrect = circleList[btnIndex].Click();

            //If its a hit
            if (isCorrect)
            {
                //remove the button from the sequence
                var itemToRemove = SequenceList.FirstOrDefault(r => r == btnIndex);
                SequenceList.Remove(itemToRemove);
                SequenceList = new ObservableCollection<int>(SequenceList);
            }
          
            Debug.WriteLine($"{reactionSw.ElapsedMilliseconds}ms");

            //TODO add current config (i.e. difficulty/ dominant hand)
            clickHistoryList.Add(new ClickHistory
            {
                ClickDate = DateTime.Now,
                Distance = bouncingBall.Distance,
                ReactionTime = reactionSw.ElapsedMilliseconds,
                ElapsedTime = elapsedMs,
                IsCorrect = isCorrect
            });

        }

        void saveResult()
        {
            var output = new TestResult
            {
                UserInfo = UserInfo,
                ClickHistoryList = clickHistoryList
            };
            var payload = JsonConvert.SerializeObject(output);
            if (!Directory.Exists("./output"))
                Directory.CreateDirectory("output");
            File.WriteAllText($"output/{DateTime.Now.ToString("yyyyMMddHHmmss")}.json", payload);
            MessageBox.Show("Done");
        }

        //Randomly pick one and trigger it.
        void triggerControl()
        {
            Task.Run(async () =>
            {
                var random = new Random();

                while (true)
                {
                    //Time 
                    await Task.Delay(1000);

                    int index = random.Next(SequenceList.Count);
                    index = SequenceList[index];
                    var targetControl = circleList[index];
                    var targetPQ = index == 0 || index == 2 ? pqCircle1 : pqCircle2;


                    reactionSw.Restart();
                    await Dispatcher.Invoke(async () =>
                       {
                           targetPQ.Enable();
                           await Task.Delay(1000);
                           targetPQ.Disable();
                           targetControl.Enable();
                           await Task.Delay(1000);
                           targetControl.Disable();
                       });


                }
            });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            saveResult();
        }
    }
}
