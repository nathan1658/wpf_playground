using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace wpf_playground
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
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
            catch
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

        App()
        {
            initSettingsFromConfig();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            new LandingPage().Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            ComHelper.closePort();
            base.OnExit(e);
        }
    }
}
