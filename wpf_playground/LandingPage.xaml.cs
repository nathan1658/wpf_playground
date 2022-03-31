using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace wpf_playground
{
    /// <summary>
    /// Interaction logic for LandingPage.xaml
    /// </summary>
    public partial class LandingPage : Window
    {
        public LandingPage()
        {
            this.WindowState = WindowState.Maximized;

            InitializeComponent();
            //footTracking.start();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            new UserInfoPage().Show();
            this.Close();
        }
    }
}
