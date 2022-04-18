using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace wpf_playground
{
    /// <summary>
    /// Interaction logic for BouncingBall.xaml
    /// </summary>
    public partial class BouncingBall : UserControl, INotifyPropertyChanged
    {

        double phi;
        double xi, yi, theta, xValue, yValue;
        double currentPointX;
        double _cursorX, _cursorY = 0;

        double cursorX
        {
            get
            {
                return _cursorX;
            }
            set
            {
                //prevent the blue ball go out of bound
                if (value < jBall.Width / 2)
                    value = jBall.Width / 2;

                if (value > board.Width - jBall.Width / 2)
                    value = board.Width - jBall.Width / 2;

                _cursorX = value;
                Dispatcher.Invoke(() =>
                {
                    Canvas.SetLeft(jBall, value - jBall.Width / 2);
                });
            }
        }
        double cursorY
        {
            get
            {
                return _cursorY;
            }
            set
            {

                //prevent the blue ball go out of bound
                if (value < jBall.Height / 2)
                    value = jBall.Height / 2;

                if (value > board.Height - jBall.Height / 2)
                    value = board.Height - jBall.Height / 2;

                _cursorY = value;
                Dispatcher.Invoke(() =>
                {
                    Canvas.SetTop(jBall, value - jBall.Height / 2);
                });
            }
        }

        bool useJoystick = true;

        private void Board_MouseMove(object sender, MouseEventArgs e)
        {
            if (useJoystick) return;
            // Get the x and y coordinates of the mouse pointer.
            System.Windows.Point position = e.GetPosition(this);
            cursorX = position.X;
            cursorY = position.Y;
        }

        double currentPointY;
        double xCenter;
        double yCenter;
        bool stopped = false;
        bool started = false;
        private int randomizeCounter = 0;
        Random random = new Random();
        int movingLevel;

        private string _xVal;

        public string XVal
        {
            get { return _xVal; }
            set
            {
                _xVal = value;
                InformPropertyChanged("XVal");
            }
        }

        void InformPropertyChanged([CallerMemberName] string propName = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }


        public bool IsDebugMode
        {
            get
            {
                return State.DebugMode;
            }
        }

        private string _yVal;

        public string YVal
        {
            get { return _yVal; }
            set
            {
                _yVal = value;
                InformPropertyChanged("YVal");
            }
        }


        private string _deviceType;

        public string DeviceType
        {
            get { return _deviceType; }
            set
            {
                _deviceType = value;
                InformPropertyChanged("DeviceType");
            }
        }

        public void stop()
        {
            this.stopped = true;
        }

        public BouncingBall()
        {
            InitializeComponent();
            this.DataContext = this;

            var diffLevel = State.UserInfo.Level;
            if (diffLevel == Model.LevelEnum.L50)
            {
                movingLevel = 50;
            }

            if (diffLevel == Model.LevelEnum.L75)
            {
                movingLevel = 75;
            }

            if (diffLevel == Model.LevelEnum.L100)
            {
                movingLevel = 100;
            }

            this.Loaded += BouncingBall_Loaded;
        }

        public void start()
        {
            if (!started)
            {
                started = true;
                Task.Run(() =>
                {
                    while (!stopped)
                    {
                        move();
                        Thread.Sleep(10);
                    }
                });
            }
        }


        string getLog(DeviceInstance di)
        {
            return $"{di.InstanceName}:{di.InstanceGuid}:{di.ProductName}:{di.ProductGuid}:{di.Type}";
        }

        private void BouncingBall_Loaded(object sender, RoutedEventArgs e)
        {
            xCenter = board.Width / 2 - (ball.Width / 2);
            yCenter = board.Height / 2 - (ball.Height / 2);

            //Center both ball and jBall
            foreach (var tmpBall in new List<Grid> { ball, jBall })
            {
                Canvas.SetTop(tmpBall, yCenter);
                Canvas.SetLeft(tmpBall, xCenter);
            }

            //Accquire controller
            new Thread(() =>
            {
                // Initialize DirectInput
                var directInput = new DirectInput();

                // Find a Joystick Guid
                var joystickGuid = Guid.Empty;

                string log = "";
                log += "GameControl count: " + directInput.GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AllDevices).Count + "\n";
                foreach (var deviceInstance in directInput.GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AllDevices))
                {
                    log += getLog(deviceInstance) + "\n";
                    joystickGuid = deviceInstance.InstanceGuid;
                    Dispatcher.Invoke(() =>
                    {
                        DeviceType = deviceInstance.Type.ToString();

                    });
                }


                log += "Joystick count: " + directInput.GetDevices(SharpDX.DirectInput.DeviceType.Joystick,
                            DeviceEnumerationFlags.AllDevices).Count + "\n";

                // If Gamepad not found, look for a Joystick
                if (joystickGuid == Guid.Empty)
                    foreach (var deviceInstance in directInput.GetDevices(SharpDX.DirectInput.DeviceType.Joystick,
                            DeviceEnumerationFlags.AllDevices))
                    {
                        log += getLog(deviceInstance) + "\n";
                        joystickGuid = deviceInstance.InstanceGuid;
                        Dispatcher.Invoke(() =>
                        {
                            DeviceType = deviceInstance.Type.ToString();

                        });
                    }

                var fileName = $"output/log_{ DateTime.Now.ToString("yyyyMMddHHmmss") }.txt";
                File.WriteAllText(fileName, log);

                // If Joystick not found, throws an error
                if (joystickGuid == Guid.Empty)
                {
                    //MessageBox.Show("No joystick found, will use mouse as fallback.");

                    System.Diagnostics.Debug.WriteLine("No joystick found, will use mouse as fallback.");
                    useJoystick = false;
                    return;
                }

                // Instantiate the joystick
                var joystick = new Joystick(directInput, joystickGuid);

                System.Diagnostics.Debug.WriteLine("Found Joystick/Gamepad with GUID: {0}", joystickGuid);

                // Query all suported ForceFeedback effects
                //var allEffects = joystick.GetEffects();
                //foreach (var effectInfo in allEffects)
                //    System.Diagnostics.Debug.WriteLine("Effect available {0}", effectInfo.Name);

                // Set BufferSize in order to use buffered data.
                joystick.Properties.BufferSize = 128;

                // Acquire the joystick
                joystick.Acquire();

                // Poll events from joystick
                while (true && !stopped)
                {
                    joystick.Poll();
                    var datas = joystick.GetBufferedData();
                    foreach (var state in datas)
                    {

                        if (state.Offset == JoystickOffset.X)
                        {
                            Dispatcher.Invoke(() =>
                            {
                                XVal = state.Value.ToString();
                                this.cursorX = state.Value / 65535.0 * this.board.Width;

                            });
                        }
                        if (state.Offset == JoystickOffset.Y)
                        {
                            Dispatcher.Invoke(() =>
                            {
                                YVal = state.Value.ToString();
                                this.cursorY = state.Value / 65535.0 * this.board.Height;

                            });
                        }
                    }
                    Thread.Sleep(10);
                }

            }).Start();
        }



        private void randomize()
        {
            var testInt = (8 - 2 + 1) * random.NextDouble() + 2;
            if (xi > testInt)
            {
                phiTranslation();
                xi = 0;
                xCenter = currentPointX;
                yCenter = currentPointY;
            }
        }

        private void phiTranslation()
        {
            var Xp = this.ActualWidth - 50 + 1 * random.NextDouble() - this.ActualWidth / 2;
            var Yp = this.ActualHeight - 50 + 1 * random.NextDouble() - this.ActualHeight / 2;
            if (random.NextDouble() > 0.5)
            {
                phi = Math.Atan(Yp / Xp) + 3.1415926;
            }
            else
            {
                phi = Math.Atan(Yp / Xp);
            }

        }

        void move()
        {


            xi = xi + 0.01;
            yi = curve(xi);
            theta = Math.Atan(yi / xi) + phi;
            xValue = Math.Sqrt(Math.Pow(xi, 2) + Math.Pow(yi, 2)) * Math.Cos(theta);
            yValue = Math.Sqrt(Math.Pow(xi, 2) + Math.Pow(yi, 2)) * Math.Sin(theta);


            currentPointX = (xValue * movingLevel) + xCenter;
            currentPointY = (yValue * movingLevel) + yCenter;

            Dispatcher.Invoke(() =>
            {
                //2sec as move() will be delay every 10ms
                if (randomizeCounter++ >= 200)
                {
                    randomize();
                    randomizeCounter = 0;
                }


                if (currentPointX < 0) currentPointX = 0;
                if (currentPointY < 0) currentPointY = 0;
                if (currentPointY > (board.Height - ball.Height)) currentPointY = board.Height - ball.Height;
                if (currentPointX > (board.Width - ball.Width)) currentPointX = board.Width - ball.Width;

                Canvas.SetTop(ball, currentPointY);
                Canvas.SetLeft(ball, currentPointX);

                var top = currentPointY;
                var left = currentPointX;


                //hit right edge
                if (left >= (board.Width - ball.Width))
                {
                    xi = 0;
                    phi = 3.14;
                    xCenter = left;
                    yCenter = top;
                }

                //hit left edge
                if (left <= 0)
                {
                    xi = 0;
                    phi = 0;
                    xCenter = left;
                    yCenter = top;
                }

                //hit bottom edge
                if (top >= (board.Height - ball.Height))
                {
                    xi = 0;
                    phi = -2;
                    xCenter = left;
                    yCenter = top;
                }

                //hit top edge
                if (top <= 0)
                {
                    xi = 0;
                    phi = 1.5;
                    xCenter = left;
                    yCenter = top;
                }


                //get ball center
                var ballLeft = left + ball.Width / 2;
                var ballTop = top + ball.Height / 2;




                var value = Math.Sqrt(Math.Pow((ballLeft - cursorX), 2) + Math.Pow((ballTop - cursorY), 2));
                value = Math.Round(value, 0);
                Distance = value;
            });

        }
        private double _distance;

        public event PropertyChangedEventHandler PropertyChanged;

        public double Distance
        {
            get
            {
                return _distance;
            }
            set
            {
                _distance = value;
                Dispatcher.Invoke(() =>
                {
                    cursorValue.Text = value.ToString();
                });
            }
        }
        private double curve(double x)
        {
            return Math.Sin(0.44 * x) + Math.Sin(0.94 * x) + Math.Sin(1.45 * x);
        }
    }
}
