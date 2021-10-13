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

        //Init settings from app config
        void initSettingsFromConfig()
        {
            //set debug Mode
            string isDebugString = ConfigurationManager.AppSettings["DebugMode"];
            State.DebugMode = isDebugString.ToLower() == "true";

            //set number of click required for each button
            string numOfClickString = ConfigurationManager.AppSettings["ClickForEachButton"];
            State.ClickCountForEachButton = int.Parse(numOfClickString);

            //set key mapping for each button
            List<String> keyButtonList = new List<string> { "TopLeftKey", "TopRightKey", "BottomLeftKey", "BottomRightKey" };
            try
            {
                State.TopLeftKey = (Key)Enum.Parse(typeof(Key), ConfigurationManager.AppSettings["TopLeftKey"]);
                State.TopRightKey = (Key)Enum.Parse(typeof(Key), ConfigurationManager.AppSettings["TopRightKey"]);
                State.BottomLeftKey = (Key)Enum.Parse(typeof(Key), ConfigurationManager.AppSettings["BottomLeftKey"]);
                State.BottomRightKey = (Key)Enum.Parse(typeof(Key), ConfigurationManager.AppSettings["BottomRightKey"]);
            }
            catch (Exception ex)
            {
                //in case any of them is empty/ exception, throw and set with default mapping

                ///[7]  [9]
                ///
                ///[1]  [3]
                State.TopLeftKey = Key.NumPad7;
                State.TopRightKey = Key.NumPad9;
                State.BottomLeftKey = Key.NumPad1;
                State.BottomRightKey = Key.NumPad3;
            }
        }


        public MainWindow()
        {
            initSettingsFromConfig();


            new UserInfoPage().ShowDialog();
            new MappingSelection().ShowDialog();



            this.DataContext = this;



            InitializeComponent();


            if (UserInfo.SignalMode == SignalModeEnum.Visual)
            {
                initVisualCircle();
            }

            if (UserInfo.SignalMode == SignalModeEnum.Auditory)
            {
                initAuditoryTarget();
            }




            if (UserInfo.PQMode == PQModeEnum.Visual)
            {
                initPqCircle();
            }
            if (UserInfo.PQMode == PQModeEnum.Auditory)
            {
                initAuditoryCircle();
            }

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
                    Thread.Sleep(10);
                }
            });
            initSequenceList();

            Task.Run(async () =>
            {
                while (true)
                {
                    if (SequenceList.Count <= 0)
                    {
                        saveResult();
                        Dispatcher.Invoke(() =>
                        {
                            Application.Current.Shutdown();
                        });
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
                        cleanUp();
                        tokenSource = new CancellationTokenSource();
                    }
                }
            });

            Loaded += MainWindow_Loaded;


            //update Mapping Image Src
            MappingImageSrc = $"/Resources/{UserInfo.Mapping}Box.Image.bmp";

        }
        void initAuditoryTarget()
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

        void initVisualCircle()
        {
            Func<HorizontalAlignment, VerticalAlignment, String, MyBaseUserControl> genCircle = (horizontalAlignment, verticalAlignment, text) =>
               {
                   var grid = new Grid
                   {
                       HorizontalAlignment = horizontalAlignment,
                       VerticalAlignment = verticalAlignment
                   };
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
                genCircle(HorizontalAlignment.Left, VerticalAlignment.Top, "0"),
                genCircle(HorizontalAlignment.Right, VerticalAlignment.Top, "1"),
                genCircle(HorizontalAlignment.Left, VerticalAlignment.Bottom, "2"),
                genCircle(HorizontalAlignment.Right, VerticalAlignment.Bottom, "3"),
            };
        }

        #region  PQ sections

        void initPqCircle()
        {
            Func<HorizontalAlignment, String, PQCircle> genCircleAndAddtoBoard = (alignment, val) =>
              {
                  var grid1 = new Grid();
                  grid1.HorizontalAlignment = alignment;
                  grid1.VerticalAlignment = VerticalAlignment.Center;

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
            var leftCircle = genCircleAndAddtoBoard(HorizontalAlignment.Left, "1");
            var rightCircle = genCircleAndAddtoBoard(HorizontalAlignment.Right, "2");
            leftPQ = leftCircle;
            rightPQ = rightCircle;
        }

        void initAuditoryCircle()
        {
            leftPQ = new AuditoryPQ(true);
            rightPQ = new AuditoryPQ(false);
        }
        #endregion

        void initSequenceList()
        {
            //The number of each button required to be pressed
            var pressCount = State.ClickCountForEachButton;
            for (int i = 0; i < pressCount; i++)
            {
                for (int j = 0; j < targetList.Count; j++)
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

            bouncingBall.start();

            bool isCorrect = false;
            reactionSw.Stop();

            int btnIndex = -1;
            var mapping = UserInfo.Mapping;

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

            Debug.WriteLine($"***Clicked***");
            Debug.WriteLine($"***{reactionSw.ElapsedMilliseconds}ms");

            tokenSource.Cancel();

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

            var header = new List<String> { "Name", "SID", "Age", "Gender", "Group", "DominantHand", "Level", "SignalMode", "PQMode", "SOA", "Mapping", "ClickDate", "ElapsedTime", "ReactionTime", "Distance", "ClickState", "Delay" };
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
                    UserInfo.Mapping.ToString(),
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
        Task triggerControl()
        {
            return Task.Run(async () =>
            {
                Stopwatch triggerSw = new Stopwatch();
                triggerSw.Start();

                var delayIntervalList = new List<int> { 1000, 2000, 3000, 4000 };
                var delayIndex = random.Next(0, delayIntervalList.Count);
                delayMS = delayIntervalList[delayIndex];
                //Time 
                await Task.Delay(delayMS);
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
                        //clicked
                        return;
                    }

                    //pq time  1000 (redball visible time) + (0.2/0.6/0.8) + + delay (1-4s)
                    if (triggerSw.ElapsedMilliseconds >= (1000 + delayPQMS + delayMS))
                    {
                        //miss
                        Debug.WriteLine("***Missed***");
                        Debug.WriteLine("***" + triggerSw.ElapsedMilliseconds + "ms");

                        Dispatcher.Invoke(() =>
                        {
                            targetControl.Disable();
                        });
                        miss();
                        return;
                    }

                    if (!pqEnded && triggerSw.ElapsedMilliseconds >= (delayPQMS + delayMS))
                    {
                        pqEnded = true;
                        Dispatcher.Invoke(() =>
                        {

                            targetPQ.Disable();

                            Debug.WriteLine("PQ disable: " + triggerSw.ElapsedMilliseconds);
                            targetControl.Enable();
                            reactionSw.Restart();
                        });
                    }
                    await Task.Delay(10);
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

                targetList.ForEach((x) =>
                {
                    x.Disable();
                });

                leftPQ.Disable();
                rightPQ.Disable();

            });
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            saveResult();
        }
    }
}
