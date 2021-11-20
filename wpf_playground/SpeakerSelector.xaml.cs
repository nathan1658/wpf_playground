using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for SpeakerSelector.xaml
    /// </summary>
    public partial class SpeakerSelector : UserControl
    {


        public string ButtonColor
        {
            get { return (string)GetValue(ButtonColorProperty); }
            set { SetValue(ButtonColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ButtonColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ButtonColorProperty =
            DependencyProperty.Register("ButtonColor", typeof(string), typeof(SpeakerSelector),new PropertyMetadata("Purple"));



        public DirectSoundDeviceInfo SelectedSoundDevice
        {
            get { return (DirectSoundDeviceInfo)GetValue(SelectedSoundDeviceProperty); }
            set
            {
                SetValue(SelectedSoundDeviceProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for SelectedSoundDevice.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedSoundDeviceProperty =
            DependencyProperty.Register("SelectedSoundDevice", typeof(DirectSoundDeviceInfo), typeof(SpeakerSelector));


        public String HzValue
        {
            get { return (String)GetValue(HzValueProperty); }
            set
            {
                SetValue(HzValueProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for HzValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HzValueProperty =
            DependencyProperty.Register("HzValue", typeof(String), typeof(SpeakerSelector));




        public ICommand TestLeftCommand
        {
            get { return (ICommand)GetValue(TestLeftCommandProperty); }
            set { SetValue(TestLeftCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TestLeftCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TestLeftCommandProperty =
            DependencyProperty.Register("TestLeftCommand", typeof(ICommand), typeof(SpeakerSelector));





        public ICommand TestRightCommand
        {
            get { return (ICommand)GetValue(TestRightCommandProperty); }
            set { SetValue(TestRightCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TestRightCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TestRightCommandProperty =
            DependencyProperty.Register("TestRightCommand", typeof(ICommand), typeof(SpeakerSelector));



        public string DescriptionText
        {
            get
            {
                return (string)GetValue(DescriptionTextProperty);
            }
            set
            {
                SetValue(DescriptionTextProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for DescriptionText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DescriptionTextProperty =
            DependencyProperty.Register("DescriptionText", typeof(string), typeof(SpeakerSelector));

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private static readonly Regex _regex = new Regex("[^0-9]+"); //regex that matches disallowed text

        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        public SpeakerSelector()
        {
            InitializeComponent();
            TestLeftCommand = genTestCommand(true);
            TestRightCommand = genTestCommand(false);
        }

        DelegateCommand genTestCommand
            (bool isLeft)
        {
            return new DelegateCommand((val) =>
            {
                try
                {
                    if (SelectedSoundDevice == null || string.IsNullOrEmpty(HzValue))
                        return;
                    var control = new AuditorySignal(SelectedSoundDevice, int.Parse(HzValue), isLeft);
                    Dispatcher.InvokeAsync(async () =>
                    {
                        if (control != null)
                        {
                            control.Enable();
                            await Task.Delay(200);
                            control.Disable();
                            control = null;
                        }
                    });
                }
                catch { }
            });
        }
    }
}
