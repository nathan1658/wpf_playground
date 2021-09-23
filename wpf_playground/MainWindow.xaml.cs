﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
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

        /// <summary>
        /// Count the time between red color and click
        /// </summary>
        Stopwatch reactionSw = new Stopwatch();
        int delayMS;
        /// <summary>
        /// The timer of overall game
        /// </summary>
        Stopwatch gameSw = new Stopwatch();
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


        CancellationTokenSource tokenSource = new CancellationTokenSource();


        public MainWindow()
        {

            new UserInfoPage().ShowDialog();
            this.DataContext = this;

            InitializeComponent();

            //Init the control list here
            circleList = new List<MyBaseUserControl>
            {
                this.circle1,
                this.circle2,
                this.circle3,
                this.circle4,
            };

            gameSw.Start();



            Task.Run(() =>
            {
                while (true)
                {
                    //update game time
                    Dispatcher.Invoke(() =>
                    {
                        gameCounter.Text = gameSw.ElapsedMilliseconds.ToString() + "ms";
                    });

                }
            });
            initSequenceList();

            Task.Run(async () =>
            {
                while (true)
                {
                    if (SequenceList.Count <= 0)
                    {
                        MessageBox.Show("GG");
                        saveResult();
                        Dispatcher.Invoke(() =>
                        {
                            Application.Current.Shutdown();
                        });
                    }

                    //start the red ball logic here
                    var task = triggerControl(tokenSource);
                    try
                    {
                        await task;
                    }
                    catch { }
                    finally
                    {
                        task.Dispose();
                        cleanUp();
                        tokenSource = new CancellationTokenSource();
                    }
                }
            });

            Loaded += MainWindow_Loaded;

        }


        void initSequenceList()
        {
            //Each button need to be press five times
            var pressCount = 1;
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

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            window.KeyDown += MainWindow_KeyDown;
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            bool isCorrect = false;
            reactionSw.Stop();
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
                hit();
            }
            else
            {
                wrong();
            }

            Debug.WriteLine($"{reactionSw.ElapsedMilliseconds}ms");

            tokenSource.Cancel();

        }


        int getSoa()
        {
            int ms = -1;
            if (UserInfo.SOA == SOAEnum.Soa200)
                ms = 200; if (UserInfo.SOA == SOAEnum.Soa600)
                ms = 600; if (UserInfo.SOA == SOAEnum.Soa1000)
                ms = 1000;
            return ms;
        }

        void saveResult()
        {
            var output = new TestResult
            {
                UserInfo = UserInfo,
                ClickHistoryList = clickHistoryList
            };

            //Write to CSV

            var header = new List<String> { "Name", "SID", "Age", "Gender", "Group", "DominantHand", "Level", "SignalMode", "PQMode", "SOA", "ClickDate", "ElapsedTime", "ReactionTime", "Distance", "ClickState","Delay" };
            var csvOutput = String.Join(",", header) + "\n";
            for (int i = 0; i < clickHistoryList.Count; i++)
            {
                var element = clickHistoryList[i];
                var tmp = new List<String>
                {
                    UserInfo.Name,
                    UserInfo.SID,
                    UserInfo.Age,
                    UserInfo.Gender.ToString(),
                    UserInfo.Group.ToString(),
                    UserInfo.DominantHand.ToString(),
                    UserInfo.Level.ToString(),
                    UserInfo.SignalMode.ToString(),
                    UserInfo.PQMode.ToString(),
                    UserInfo.SOA.ToString(),
                    element.ClickDate.ToString(),
                    element.ElapsedTime.ToString(),
                    element.ReactionTime.ToString(),
                    element.Distance.ToString(),
                    element.ClickState.ToString(),
                    element.Delay.ToString()
                };
                csvOutput += String.Join(",", tmp) + "\n";
            }
            if (!Directory.Exists("./output"))
                Directory.CreateDirectory("output");
            File.WriteAllText($"output/{DateTime.Now.ToString("yyyyMMddHHmmss")}.csv", csvOutput);
            MessageBox.Show("Done");
        }

        //Randomly pick one and trigger it.
        Task triggerControl(CancellationTokenSource source)
        {
            return Task.Run(async () =>
            {
                Stopwatch triggerSw = new Stopwatch();
                triggerSw.Start();

                var delayIntervalList = new List<int> { 1000, 2000, 3000, 4000 };
                var delayIndex = random.Next(0, delayIntervalList.Count );
                delayMS = delayIntervalList[delayIndex];
                int delayPQMS = -1;
                //Time 
                await Task.Delay(delayMS);
                System.Diagnostics.Debug.WriteLine("After delay: " + triggerSw.ElapsedMilliseconds);
                int index = random.Next(SequenceList.Count);
                index = SequenceList[index];
                var targetControl = circleList[index];
                var targetPQ = index == 0 || index == 2 ? pqCircle1 : pqCircle2;


                await Dispatcher.Invoke(async () =>
                 {
                     targetPQ.Enable();
                     delayPQMS = getSoa();
                     await Task.Delay(delayPQMS);
                     targetPQ.Disable();
                     System.Diagnostics.Debug.WriteLine("PQ disable: " + triggerSw.ElapsedMilliseconds);

                     targetControl.Enable();
                     reactionSw.Restart();
                 });

                while (true)
                {
                    if (tokenSource.Token.IsCancellationRequested)
                    {
                        //clicked
                        return;
                    }

                    //TODO pq time (0.2,0.6,0.8) + 1000 (redball visible time) + delay (1-4s)
                    if (triggerSw.ElapsedMilliseconds >= (1000 + delayPQMS + delayMS))
                    {
                        System.Diagnostics.Debug.WriteLine("Missed: " + triggerSw.ElapsedMilliseconds);

                        Dispatcher.Invoke(() =>
                        {
                            targetControl.Disable();

                        });
                        //miss
                        miss();
                        return;
                    }
                }
            });
        }

        void miss()
        {
            clickHistoryList.Add(new ClickHistory
            {
                ClickDate = DateTime.Now,
                Distance = bouncingBall.Distance,
                ReactionTime = reactionSw.ElapsedMilliseconds,
                ElapsedTime = gameSw.ElapsedMilliseconds,
                ClickState = ClickState.Miss,
                Delay = delayMS,

            });
        }

        void hit()
        {
            clickHistoryList.Add(new ClickHistory
            {
                ClickDate = DateTime.Now,
                Distance = bouncingBall.Distance,
                ReactionTime = reactionSw.ElapsedMilliseconds,
                ElapsedTime = gameSw.ElapsedMilliseconds,
                ClickState = ClickState.Correct,
                Delay = delayMS,

            });
        }

        void wrong()
        {
            clickHistoryList.Add(new ClickHistory
            {
                ClickDate = DateTime.Now,
                Distance = bouncingBall.Distance,
                ReactionTime = reactionSw.ElapsedMilliseconds,
                ElapsedTime = gameSw.ElapsedMilliseconds,
                ClickState = ClickState.Incorrect,
                Delay = delayMS,

            });
        }

        void cleanUp()
        {
            Dispatcher.Invoke(() =>
            {

                circleList.ForEach((x) =>
                {
                    x.Disable();
                });
                pqCircle1.Disable();
                pqCircle2.Disable();
            });
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            saveResult();
        }
    }
}
