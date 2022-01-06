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
        static Random rnd = new Random(DateTime.Now.Millisecond);

        /// <summary>
        /// Count the time between red color and click
        /// </summary>
        Stopwatch reactionSw = new Stopwatch();
        int delayIntervalInMs;

        Random random = new Random();
        public const double SIGNAL_VISIBLE_TIME = 1000;
        public const double PQ_VISIBLE_TIME = 200;

        public event PropertyChangedEventHandler PropertyChanged;
        private void InformPropertyChanged([CallerMemberName] string propName = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private List<ExperimentLog> clickHistoryList { get; set; } = new List<ExperimentLog>();

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

        List<MyBaseUserControl> signalList = new List<MyBaseUserControl>();

        private List<VisualSignal> visualSignalList = new List<VisualSignal>();
        private List<AuditorySignal> auditorySignalList = new List<AuditorySignal>();
        private List<TactileSignal> tactileSignalList = new List<TactileSignal>();

        List<MyBaseUserControl> pqList = new List<MyBaseUserControl>();

        private VisualPQ leftVisualPQ, rightVisualPQ;
        private AuditoryPQ leftAuditoryPQ, rightAuditoryPQ;
        private TactilePQ leftTactilePQ, rightTactilePQ;

        CancellationTokenSource tokenSource = new CancellationTokenSource();

        int signalIndex = -1;
        int pqIndex = -1;
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

            //Only Start the timer when first time start the game.
            if (!State.TestStopwatch.IsRunning)
                State.TestStopwatch.Restart();

            Task.Run(async () =>
               {
                   while (!gameEnd)
                   {
                       try
                       {
                           //update game time
                           Dispatcher.Invoke(() =>
                              {
                                  gameCounter.Text = ElapsedTime.ToString() + "ms";
                              });
                       }
                       finally
                       {
                           await Task.Delay(10);
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
        TestMapping testMapping;

        #region comport part
        ComSignalConfig comSignalConfig;
        #endregion

        void initComPort()
        {

            SignalModeEnum? signalMode = null;
            PQModeEnum? pQMode = null;

            if (UserInfo.SignalVisualChecked) signalMode = SignalModeEnum.Visual;
            if (UserInfo.SignalAuditoryChecked) signalMode = SignalModeEnum.Auditory;
            if (UserInfo.SignalTactileChecked) signalMode = SignalModeEnum.Tactile;

            if (UserInfo.PQVisualChecked) pQMode = PQModeEnum.Visual;
            if (UserInfo.PQAuditoryChecked) pQMode = PQModeEnum.Auditory;
            if (UserInfo.PQTactileChecked) pQMode = PQModeEnum.Tactile;

            comSignalConfig = new ComSignalConfig(signalMode.Value, pQMode.Value, UserInfo.SOA);

        }

        public MainWindow(bool isPracticeMode, MappingEnum mapping, TestMapping testMapping)
        {
            this.practiceMode = isPracticeMode;
            this.testMapping = testMapping;
            this.mapping = mapping;
            initSequenceList();


            initComPort();

            this.WindowState = WindowState.Maximized;
            this.DataContext = this;
            InitializeComponent();


            if (UserInfo.SignalVisualChecked)
            {
                initVisualSignal();
            }

            if (UserInfo.SignalAuditoryChecked)
            {
                initAuditorySignal();
            }

            if (UserInfo.SignalTactileChecked)
            {
                initTactileSignal();
            }


            if (UserInfo.PQVisualChecked)
            {
                initVisualPQ();
            }

            if (UserInfo.PQAuditoryChecked)
            {
                initAuditoryPQ();
            }

            if (UserInfo.PQTactileChecked)
            {
                initTactilePQ();
            }


            Loaded += MainWindow_Loaded;

            //update Mapping Image Src
            MappingImageSrc = $"/Resources/{mapping}Box.Image.bmp";


            //Init Signal List
            signalList.AddRange(visualSignalList);
            signalList.AddRange(auditorySignalList);
            signalList.AddRange(tactileSignalList);

            //Init PQ List
            pqList.AddRange(new List<MyBaseUserControl> { leftVisualPQ, rightVisualPQ });
            pqList.AddRange(new List<MyBaseUserControl> { leftAuditoryPQ, rightAuditoryPQ });
            pqList.AddRange(new List<MyBaseUserControl> { leftTactilePQ, rightTactilePQ });
        }

        #region init signal region

        void initAuditorySignal()
        {
            //Init the control list here
            auditorySignalList = new List<AuditorySignal>
            {
                new AuditorySignal(State.TopSpeaker, State.UserInfo.TopSpeakerHz, true),
                new AuditorySignal(State.TopSpeaker, State.UserInfo.TopSpeakerHz, false),
                new AuditorySignal(State.BottomSpeaker, State.UserInfo.BottomSpeakerHz, true),
                new AuditorySignal(State.BottomSpeaker, State.UserInfo.BottomSpeakerHz, false),
            };
        }

        void initTactileSignal()
        {
            //Init the control list here
            tactileSignalList = new List<TactileSignal>
            {
                new TactileSignal(State.TactileTopSpeaker, State.UserInfo.TactileTopSpeakerHz, true),
                new TactileSignal(State.TactileTopSpeaker, State.UserInfo.TactileTopSpeakerHz, false),
                new TactileSignal(State.TactileBottomSpeaker, State.UserInfo.TactileBottomSpeakerHz, true),
                new TactileSignal(State.TactileBottomSpeaker, State.UserInfo.TactileBottomSpeakerHz, false),
            };
        }

        void initVisualSignal()
        {
            Func<int, int, String, VisualSignal> genCircle = (row, column, text) =>
               {
                   var grid = new Grid
                   {
                   };
                   Grid.SetRow(grid, row);
                   Grid.SetColumn(grid, column);

                   var circle = new VisualSignal();
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
            visualSignalList = new List<VisualSignal>
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
            Func<int, int, String, VisualPQ> genCircleAndAddtoBoard = (row, column, val) =>
               {
                   var grid1 = new Grid();
                   Grid.SetRow(grid1, row);
                   Grid.SetColumn(grid1, column);
                   grid1.VerticalAlignment = VerticalAlignment.Center;
                   grid1.HorizontalAlignment = HorizontalAlignment.Center;

                   VisualPQ circle1 = new VisualPQ();

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
            leftVisualPQ = genCircleAndAddtoBoard(1, 0, "1");
            rightVisualPQ = genCircleAndAddtoBoard(1, 2, "2");

        }

        void initAuditoryPQ()
        {
            var guid = State.PQSpeaker.Guid;
            var hz = State.UserInfo.PQHz;
            leftAuditoryPQ = new AuditoryPQ(true, hz, guid);
            rightAuditoryPQ = new AuditoryPQ(false, hz, guid);
        }

        void initTactilePQ()
        {
            var guid = State.TactilePQSpeaker.Guid;
            var hz = State.UserInfo.TactilePQHz;
            leftTactilePQ = new TactilePQ(true, hz, guid);
            rightTactilePQ = new TactilePQ(false, hz, guid);
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
                    testMapping.PracticeDone = true;
                    //State.FinishedTestMappingList.Add(mapping);
                    MessageBox.Show("Finished Practice mode! Now back to mapping selection.");
                }
                else
                {
                    testMapping.TestDone = true;
                    ComHelper.send(101);
                    MessageBox.Show("Finished Test! Now back to mapping selection.");

                    //Add current mapping to finished state
                    //State.FinishedMappingList.Add(mapping);
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
                if (!practiceMode)
                {
                    ComHelper.send(100);
                }
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
            int buttonPositionIndex = -1;
            var mapping = this.mapping;

            if (e.Key == State.TopLeftKey)
            {
                buttonPositionIndex = 0;
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
                buttonPositionIndex = 1;
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
                buttonPositionIndex = 2;
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
                buttonPositionIndex = 3;
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
            var targetSignalList = new List<MyBaseUserControl>();
            if (UserInfo.SignalVisualChecked) targetSignalList.Add(visualSignalList[btnIndex]);
            if (UserInfo.SignalAuditoryChecked) targetSignalList.Add(auditorySignalList[btnIndex]);
            if (UserInfo.SignalTactileChecked) targetSignalList.Add(tactileSignalList[btnIndex]);

            //click the first one anyway.
            isCorrect = targetSignalList[0].Click();

            //If its a hit
            if (isCorrect)
            {
                //remove the button from the sequence
                var itemToRemove = SequenceList.FirstOrDefault(r => r == btnIndex);
                SequenceList.Remove(itemToRemove);
                SequenceList = new ObservableCollection<int>(SequenceList);
                hit(buttonPositionIndex);
            }
            else
            {
                wrong(buttonPositionIndex);
            }

        }



        int getSoa()
        {
            int ms = -1;
            if (UserInfo.SOA == SOAEnum.Soa200)
                ms = 200;
            if (UserInfo.SOA == SOAEnum.Soa600)
                ms = 600;
            if (UserInfo.SOA == SOAEnum.Soa400)
                ms = 400;
            return ms;
        }

        void saveResult()
        {
            var output = new TestResult(this.mapping)
            {
                UserInfo = JsonConvert.DeserializeObject<UserInfo>(JsonConvert.SerializeObject(UserInfo)),
                ClickHistoryList = clickHistoryList
            };
            State.TestResultList.Add(output);

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
                    pqIndex = -1;
                    signalIndex = -1;
                    delayIntervalInMs = -1;

                    Stopwatch triggerSw = new Stopwatch();
                    triggerSw.Start();

                    delayIntervalInMs = getDelayInterval();
                    //Time 
                    await Task.Delay(delayIntervalInMs);

                    int index = random.Next(SequenceList.Count);
                    index = SequenceList[index];

                    signalIndex = index;

                    var targetControlList = new List<MyBaseUserControl>();
                    if (UserInfo.SignalVisualChecked)
                        targetControlList.Add(visualSignalList[index]);
                    if (UserInfo.SignalAuditoryChecked)
                        targetControlList.Add(auditorySignalList[index]);
                    if (UserInfo.SignalTactileChecked)
                        targetControlList.Add(tactileSignalList[index]);

                    int soa = getSoa();

                    var targetPQList = new List<MyBaseUserControl>();


                    bool isLeft = index == 0 || index == 2;
                    pqIndex = isLeft ? 0 : 1;
                    if (UserInfo.PQVisualChecked)
                        targetPQList.Add(isLeft ? leftVisualPQ : rightVisualPQ);
                    if (UserInfo.PQAuditoryChecked)
                        targetPQList.Add(isLeft ? leftAuditoryPQ : rightAuditoryPQ);
                    if (UserInfo.PQTactileChecked)
                        targetPQList.Add(isLeft ? leftTactilePQ : rightTactilePQ);

                    Dispatcher.Invoke(() =>
                    {
                        targetPQList.ForEach(x => x.Enable());
                    });


                    var pqTriggered = false;
                    var signalTriggred = false;
                    while (true)
                    {
                        if (tokenSource.Token.IsCancellationRequested)
                        {
                            break;
                        }

                        if (!pqTriggered)
                        {
                            pqTriggered = true;
                            addPQRecord();

                            Dispatcher.Invoke(() =>
                            {
                                targetPQList.ForEach(x => x.Enable());
                            });
                        }

                        if (pqTriggered && triggerSw.ElapsedMilliseconds >= (delayIntervalInMs + PQ_VISIBLE_TIME))
                        {
                            Dispatcher.Invoke(() =>
                            {
                                targetPQList.ForEach(x => x.Disable());
                            });
                        }

                        if (!signalTriggred && triggerSw.ElapsedMilliseconds >= (delayIntervalInMs + soa))
                        {
                            signalTriggred = true;

                            if (!practiceMode)
                            {
                                //Send COM Port
                                var comVal = ComHelper.MappingDict[comSignalConfig];
                                System.Diagnostics.Debug.WriteLine("Sending " + comVal);
                                ComHelper.send(comVal);
                            }
                            addSignalRecord();
                            Dispatcher.Invoke(() =>
                            {
                                targetControlList.ForEach(x => x.Enable());
                            });
                            canClick = true;
                            reactionSw.Restart();
                        }


                        //pq time  1000 (Signal visible time) + SOA (0.2/0.6/0.8) + + delay (1-4s)
                        if (signalTriggred && triggerSw.ElapsedMilliseconds >= (SIGNAL_VISIBLE_TIME + soa + delayIntervalInMs))
                        {
                            //miss
                            Debug.WriteLine("***Missed***");
                            Debug.WriteLine("***" + triggerSw.ElapsedMilliseconds + "ms");

                            Dispatcher.Invoke(() =>
                            {
                                targetControlList.ForEach(x => x.Disable());
                            });
                            miss();
                            break;
                        }

                        //await Task.Delay(10);
                    }
                    cleanUp();
                }
                catch (Exception ex)
                {

                    System.Diagnostics.Debug.WriteLine(ex);
                }
            });
        }

        private double BouncingBallDistance
        {
            get
            {
                return this.bouncingBall.Distance;
            }
        }

        private double ElapsedTime
        {
            get
            {
                return State.TestStopwatch.ElapsedMilliseconds;
            }
        }


        void addSignalRecord()
        {
            var clickHistory = new ExperimentLog(HistoryType.Signal, signalIndex, -1, ElapsedTime, -1, BouncingBallDistance, ClickState.NA, delayIntervalInMs, pqPositionIndex: pqIndex);
            clickHistoryList.Add(clickHistory);
        }

        void addPQRecord()
        {
            var clickHistory = new ExperimentLog(HistoryType.PQ, signalIndex, -1, ElapsedTime, -1, BouncingBallDistance, ClickState.NA, delayIntervalInMs, pqPositionIndex: pqIndex);
            clickHistoryList.Add(clickHistory);
        }

        void miss()
        {
            var history = new ExperimentLog(HistoryType.Click, signalIndex, -1, ElapsedTime, reactionSw.ElapsedMilliseconds, BouncingBallDistance, ClickState.Miss, delayIntervalInMs, pqPositionIndex: pqIndex);
            clickHistoryList.Add(history);
        }

        void hit(int pressedButtonIndex)
        {
            var history = new ExperimentLog(HistoryType.Click, signalIndex, pressedButtonIndex, ElapsedTime, reactionSw.ElapsedMilliseconds, BouncingBallDistance, ClickState.Correct, delayIntervalInMs, pqPositionIndex: pqIndex);
            clickHistoryList.Add(history);
        }

        void wrong(int pressedButtonIndex)
        {
            var history = new ExperimentLog(HistoryType.Click, signalIndex, pressedButtonIndex, ElapsedTime, reactionSw.ElapsedMilliseconds, BouncingBallDistance, ClickState.Incorrect, delayIntervalInMs, pqPositionIndex: pqIndex);
            clickHistoryList.Add(history);
        }

        void cleanUp()
        {
            Dispatcher.Invoke(() =>
            {
                canClick = false;

                signalList.ForEach((x) =>
                {
                    x?.Disable();
                });

                pqList.ForEach((x) =>
                {
                    x?.Disable();
                });

            });
        }

    }
}
