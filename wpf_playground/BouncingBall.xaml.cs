using System;
using System.Collections.Generic;
using System.Linq;
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

namespace wpf_playground
{
    /// <summary>
    /// Interaction logic for BouncingBall.xaml
    /// </summary>
    public partial class BouncingBall : UserControl
    {

        double Xp, Yp, phi;
        double xi, yi, theta, xValue, yValue;
        double currentPointX;
        double currentPointY;
        double xCenter;
        double yCenter;



        public BouncingBall()
        {
            InitializeComponent();
            //Center the ball
            Canvas.SetTop(ball, board.Height / 2 - (ball.Height / 2));
            Canvas.SetLeft(ball, board.Width / 2 - (ball.Width / 2));
            Task.Run(() =>
            {

                while (true)
                {
                    move();
                    Thread.Sleep(5);
                }
            });
        }
        void move()
        {
            xi = xi + 0.01;
            yi = curve(xi);
            theta = Math.Atan(yi / xi) + phi;
            xValue = Math.Sqrt(Math.Pow(xi, 2) + Math.Pow(yi, 2)) * Math.Cos(theta);
            yValue = Math.Sqrt(Math.Pow(xi, 2) + Math.Pow(yi, 2)) * Math.Sin(theta);
            currentPointX = (xValue * 50) + xCenter;
            currentPointY = (yValue * 50) + yCenter;


            Dispatcher.Invoke(() =>
            {
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

         
            });
            
        }
        private double curve(double x)
        {
            return Math.Sin(0.44 * x) + Math.Sin(0.94 * x) + Math.Sin(1.45 * x);
        }
    }
}
