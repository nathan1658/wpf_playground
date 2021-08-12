using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
        public event PropertyChangedEventHandler PropertyChanged;
        private void InformPropertyChanged([CallerMemberName] string propName = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
        private int _score;

        public int Score
        {
            get { return _score; }
            set
            {
                _score = value;
                InformPropertyChanged("Score");
            }
        }

        private List<ClickHistory> clickHistoryList { get; set; } = new List<ClickHistory>();

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
                        //counter.Text = sw1.ElapsedMilliseconds.ToString() + "ms";
                        counter.Text = reactionSw.ElapsedMilliseconds.ToString() + "ms";
                    });
                }
            });
            Loaded += MainWindow_Loaded;

        }
        double elapsedMs = 0;

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            window.KeyDown += MainWindow_KeyDown;
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            bool result = false;
            switch (e.Key)
            {
                case Key.NumPad7:
                    result = circleList[0].Click();
                    break;
                case Key.NumPad9:
                    result = circleList[1].Click();
                    break;
                case Key.NumPad1:
                    result = circleList[2].Click();
                    break;
                case Key.NumPad3:
                    result = circleList[3].Click();
                    break;
            }
            if (result)
            {
                Score++;
            }
            else
            {
                Score--;
            }
            Debug.WriteLine($"{reactionSw.ElapsedMilliseconds}ms");
            clickHistoryList.Add(new ClickHistory
            {
                ClickDate = DateTime.Now,
                Distance = bouncingBall.Distance,
                ReactionTime = reactionSw.ElapsedMilliseconds,
                ElapsedTime = elapsedMs,
                IsCorrect = result
            });
            //if (Score >= 5)
            //{
            //    MessageBox.Show("GG");
            //    //output file to json
            //    saveJson();
            //}
        }

        void saveJson()
        {
            var output = new TestResult
            {
                UserInfo = UserInfo,
                ClickHistoryList = clickHistoryList
            };
            var payload = JsonConvert.SerializeObject(output);
            File.WriteAllText("./output.txt", payload);
            MessageBox.Show("Done");
        }


        void triggerControl()
        {
            Task.Run(async () =>
            {
                int lastIndex = -1;
                while (true)
                {
                    //Time 
                    await Task.Delay(1000);

                    int index = rnd.Next(circleList.Count);
                    //Prevent same index show continusly
                    while (lastIndex == index)
                    {
                        index = rnd.Next(circleList.Count);
                    }
                    lastIndex = index;
                    var targetControl = (circleList[index]);
                    if (!targetControl.Triggered)
                    {
                               reactionSw.Restart();
                        await Dispatcher.Invoke(async () =>
                           {
                               targetControl.Enable();
                               await Task.Delay(1000);
                               targetControl.Disable();
                           });
                    }
                }
            });
        }

    }
}
