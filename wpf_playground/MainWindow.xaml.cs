using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
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
        List<MyBaseUserControl> targetList = new List<MyBaseUserControl>();
        static Random rnd = new Random(DateTime.Now.Millisecond);

        /// <summary>
        /// Count the time between red color and click
        /// </summary>
        Stopwatch reactionSw = new Stopwatch();
        int delayIntervalInMs;
        /// <summary>
        /// The timer of overall game
        /// </summary>
        Stopwatch gameSw = new Stopwatch();
        Random random = new Random();
        public const double SIGNAL_VISIBLE_TIME = 1000;

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

        private MyBaseUserControl leftPQ, rightPQ;
        CancellationTokenSource tokenSource = new CancellationTokenSource();

        private string _mappingImageSrc;

        public string MappingImageSrc
        {
            get { return _mappingImageSrc; }
            set
            {
                _mappingImageSrc = value;

                InformPropertyChanged("MappingImageSrc");
            }
        }

        public bool IsDebugMode
        {
            get
            {
                return State.DebugMode;
            }
        }

        private bool _isGameStarted = false;

        public bool IsGameStarted
        {
            get { return _isGameStarted; }
            set
            {
                _isGameStarted = value;
                InformPropertyChanged("IsGameStarted");
            }
        }

        bool gameEnd = false;

        void start()
        {
            gameSw.Start();
            Task.Run(() =>
               {
                   while (!gameEnd)
                   {
                       try
                       {
                           //update game time
                           Dispatcher.Invoke(() =>
                              {
                                  gameCounter.Text = gameSw.ElapsedMilliseconds.ToString() + "ms";
                              });
                       }
                       finally
                       {
                           Thread.Sleep(10);
                       }
                   }
               });

            Task.Run(async () =>
               {
                   while (!gameEnd)
                   {
                       if (SequenceList.Count <= 0)
                       {
                           finishedAllSequence();
                       }

                       //start the red ball logic here
                       var task = triggerControl();
                       try
                       {
                           await task;
                       }
                       catch { }
                       finally
                       {
                           task.Dispose();
                           tokenSource = new CancellationTokenSource();
                       }
                   }
               });

        }
        bool practiceMode = false;
        MappingEnum mapping = MappingEnum.NONE;
        public MainWindow(bool isPracticeMode, MappingEnum mapping)
        {
            this.practiceMode = isPracticeMode;
            this.mapping = mapping;
            initSequenceList();


            this.WindowState = WindowState.Maximized;
            this.DataContext = this;
            InitializeComponent();


            if (UserInfo.SignalMode == SignalModeEnum.Visual)
            {
                initVisualSignal();
            }

            if (UserInfo.SignalMode == SignalModeEnum.Auditory)
            {
                initAuditorySignal();
            }

            if (UserInfo.SignalMode == SignalModeEnum.Tactile)
            {
                initTactileSignal();
            }


            if (UserInfo.PQMode == PQModeEnum.Visual)
            {
                initVisualPQ();
            }
            if (UserInfo.PQMode == PQModeEnum.Auditory)
            {
                initAuditoryPQ();
            }
            if(UserInfo.PQMode == PQModeEnum.Tactile)
            {
                initTactilePQ();
            }


            Loaded += MainWindow_Loaded;

            //update Mapping Image Src
            MappingImageSrc = $"/Resources/{mapping}Box.Image.bmp";

        }

        #region init signal region

        void initAuditorySignal()
        {
            //Init the control list here
            targetList = new List<MyBaseUserControl>
            {
                new AuditoryTarget(State.TopSpeaker, State.UserInfo.TopSpeakerHz, true),
                new AuditoryTarget(State.TopSpeaker, State.UserInfo.TopSpeakerHz, false),
                new AuditoryTarget(State.BottomSpeaker, State.UserInfo.BottomSpeakerHz, true),
                new AuditoryTarget(State.BottomSpeaker, State.UserInfo.BottomSpeakerHz, false),
            };
        }

        void initTactileSignal()
        {
            //Init the control list here
            targetList = new List<MyBaseUserControl>
            {
                new AuditoryTarget(State.TactileTopSpeaker, State.UserInfo.TactileTopSpeakerHz, true),
                new AuditoryTarget(State.TactileTopSpeaker, State.UserInfo.TactileTopSpeakerHz, false),
                new AuditoryTarget(State.TactileBottomSpeaker, State.UserInfo.TactileBottomSpeakerHz, true),
                new AuditoryTarget(State.TactileBottomSpeaker, State.UserInfo.TactileBottomSpeakerHz, false),
            };
        }

        void initVisualSignal()
        {
            Func<int, int, String, MyBaseUserControl> genCircle = (row, column, text) =>
               {
                   var grid = new Grid
                   {
                   };
                   Grid.SetRow(grid, row);
                   Grid.SetColumn(grid, column);

                   var circle = new Circle();
                   grid.Children.Add(circle);

                   //If its debug mode, add a text letter at center of circle
                   if (IsDebugMode)
                   {
                       grid.Children.Add(new TextBlock
                       {
                           HorizontalAlignment = HorizontalAlignment.Center,
                           VerticalAlignment = VerticalAlignment.Center,
                           Text = text
                       });
                   }
                   gameBoard.Children.Add(grid);
                   return circle;
               };

            //Init the control list here
            targetList = new List<MyBaseUserControl>
            {
                genCircle(0, 0, "0"),
                genCircle(0, 2, "1"),
                genCircle(2, 0, "2"),
                genCircle(2, 2, "3"),
            };
        }

        #endregion

        #region  PQ sections

        void initVisualPQ()
        {
            Func<int, int, String, PQCircle> genCircleAndAddtoBoard = (row, column, val) =>
               {
                   var grid1 = new Grid();
                   Grid.SetRow(grid1, row);
                   Grid.SetColumn(grid1, column);
                   grid1.VerticalAlignment = VerticalAlignment.Center;
                   grid1.HorizontalAlignment = HorizontalAlignment.Center;

                   PQCircle circle1 = new PQCircle();

                   grid1.Children.Add(circle1);

                   if (IsDebugMode)
                   {
                       TextBlock tb1 = new TextBlock();
                       tb1.HorizontalAlignment = HorizontalAlignment.Center;
                       tb1.VerticalAlignment = VerticalAlignment.Center;
                       tb1.Text = val;
                       grid1.Children.Add(tb1);
                   }

                   gameBoard.Children.Add(grid1);
                   return circle1;
               };
            var leftCircle = genCircleAndAddtoBoard(1, 0, "1");
            var rightCircle = genCircleAndAddtoBoard(1, 2, "2");
            leftPQ = leftCircle;
            rightPQ = rightCircle;
        }

        void initAuditoryPQ()
        {
            var guid = State.PQSpeaker.Guid;
            var hz = State.UserInfo.PQHz;
            leftPQ = new AuditoryPQ(true,hz,guid );
            rightPQ = new AuditoryPQ(false,hz,guid);
        }

        void initTactilePQ()
        {
            var guid = State.TactilePQSpeaker.Guid;
            var hz = State.UserInfo.TactilePQHz;
            leftPQ = new AuditoryPQ(true, hz, guid);
            rightPQ = new AuditoryPQ(false, hz, guid);
        }

        #endregion

        void initSequenceList()
        {
            //The number of each button required to be pressed
            var pressCount = State.ClickCountForEachButton;

            //If is practice mode, press count = 1;
            if (practiceMode)
                pressCount = 1;

            for (int i = 0; i < pressCount; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    SequenceList.Add(j);
                }
            }
            //sequenceList = 0 1 2 3 0 1 2 3 0 1 2 3...... each button need to be press 5 times

            //shuffle it
            SequenceList = new ObservableCollection<int>(SequenceList.OrderBy(a => Guid.NewGuid()).ToList());
        }

        void finishedAllSequence()
        {
            Dispatcher.Invoke(() =>
            {

                if (practiceMode)
                {
                    MessageBox.Show("Finished Practice mode! Now back to mapping selection.");

                }
                else
                {
                    //Add current mapping to finished state
                    State.FinishedMappingList.Add(mapping);
                    saveResult();
                }
                gameEnd = true;
                new MappingSelection().Show();
                this.Close();
            });
        }


        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            window.KeyDown += MainWindow_KeyDown;
        }

        bool canClick = false;

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {

            if (!IsGameStarted)
            {
                bouncingBall.start();
                start();
                IsGameStarted = true;
                return;
            }
            if (!canClick)
                return;
            bool isCorrect = false;
            reactionSw.Stop();

            int btnIndex = -1;
            var mapping = this.mapping;

            if (e.Key == State.TopLeftKey)
            {
                if (mapping == MappingEnum.BC)
                    btnIndex = 0;

                if (mapping == MappingEnum.TC)
                    btnIndex = 2;

                if (mapping == MappingEnum.LC)
                    btnIndex = 1;

                if (mapping == MappingEnum.BI)
                    btnIndex = 3;
            }

            if (e.Key == State.TopRightKey)
            {
                if (mapping == MappingEnum.BC)
                    btnIndex = 1;

                if (mapping == MappingEnum.TC)
                    btnIndex = 3;

                if (mapping == MappingEnum.LC)
                    btnIndex = 0;

                if (mapping == MappingEnum.BI)
                    btnIndex = 2;
            }

            if (e.Key == State.BottomLeftKey)
            {
                if (mapping == MappingEnum.BC)
                    btnIndex = 2;

                if (mapping == MappingEnum.TC)
                    btnIndex = 0;

                if (mapping == MappingEnum.LC)
                    btnIndex = 3;

                if (mapping == MappingEnum.BI)
                    btnIndex = 1;
            }

            if (e.Key == State.BottomRightKey)
            {
                if (mapping == MappingEnum.BC)
                    btnIndex = 3;

                if (mapping == MappingEnum.TC)
                    btnIndex = 1;

                if (mapping == MappingEnum.LC)
                    btnIndex = 2;

                if (mapping == MappingEnum.BI)
                    btnIndex = 0;
            }

            if (btnIndex == -1)//Do nothing
                return;

            Debug.WriteLine($"***Clicked***");
            Debug.WriteLine($"***{reactionSw.ElapsedMilliseconds}ms");

            tokenSource.Cancel();
            isCorrect = targetList[btnIndex].Click();

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

        }


        int getSoa()
        {
            int ms = -1;
            if (UserInfo.SOA == SOAEnum.Soa200)
                ms = 200;
            if (UserInfo.SOA == SOAEnum.Soa600)
                ms = 600;
            if (UserInfo.SOA == SOAEnum.Soa1000)
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

            var header = new List<String> { "Name", "SID", "Age", "Gender", "DominantHand", "Level", "SignalMode", "PQMode", "SOA", "Mapping", "ClickDate", "ElapsedTime", "ReactionTime", "Distance", "ClickState", "Delay" };
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
                    UserInfo.DominantHand.ToString(),
                    UserInfo.Level.ToString(),
                    UserInfo.SignalMode.ToString(),
                    UserInfo.PQMode.ToString(),
                    UserInfo.SOA.ToString(),
                    this.mapping.ToString(),
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
            var fileName = $"output/{ DateTime.Now.ToString("yyyyMMddHHmmss") }.csv";
            File.WriteAllText(fileName, csvOutput);
            MessageBox.Show($"Saved test result to {fileName}");
        }

        int getDelayInterval()
        {
            var delayIntervalList = new List<int> { 1000, 2000, 3000, 4000 };
            var delayIndex = random.Next(0, delayIntervalList.Count);
            return delayIntervalList[delayIndex];
        }

        //Randomly pick one and trigger it.
        Task triggerControl()
        {
            return Task.Run(async () =>
            {
                try
                {
                    Stopwatch triggerSw = new Stopwatch();
                    triggerSw.Start();

                    delayIntervalInMs = getDelayInterval();
                    //Time 
                    await Task.Delay(delayIntervalInMs);

                    int index = random.Next(SequenceList.Count);
                    index = SequenceList[index];
                    var targetControl = targetList[index];

                    int delayPQMS = -1;
                    delayPQMS = getSoa();

                    var targetPQ = index == 0 || index == 2 ? leftPQ : rightPQ;

                    Dispatcher.Invoke(() =>
                    {
                        targetPQ.Enable();
                    });


                    var pqEnded = false;
                    while (true)
                    {
                        if (tokenSource.Token.IsCancellationRequested)
                        {
                            break;
                        }

                        //pq time  1000 (redball visible time) + (0.2/0.6/0.8) + + delay (1-4s)
                        if (triggerSw.ElapsedMilliseconds >= (SIGNAL_VISIBLE_TIME + delayPQMS + delayIntervalInMs))
                        {
                            //miss
                            Debug.WriteLine("***Missed***");
                            Debug.WriteLine("***" + triggerSw.ElapsedMilliseconds + "ms");

                            Dispatcher.Invoke(() =>
                            {
                                targetControl.Disable();
                            });
                            miss();
                            break;
                        }

                        if (!pqEnded && triggerSw.ElapsedMilliseconds >= (delayPQMS + delayIntervalInMs))
                        {
                            pqEnded = true;
                            Dispatcher.Invoke(() =>
                            {
                                targetPQ.Disable();
                                Debug.WriteLine("PQ disable: " + triggerSw.ElapsedMilliseconds);
                                targetControl.Enable();
                                canClick = true;
                                reactionSw.Restart();
                            });
                        }
                        await Task.Delay(10);
                    }
                    cleanUp();
                }
                catch { }
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
                Delay = delayIntervalInMs,

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
                Delay = delayIntervalInMs,

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
                Delay = delayIntervalInMs,

            });
        }

        void cleanUp()
        {
            Dispatcher.Invoke(() =>
            {
                canClick = false;
                targetList.ForEach((x) =>
                {
                    x.Disable();
                });

                leftPQ.Disable();
                rightPQ.Disable();

            });
        }

    }
}
