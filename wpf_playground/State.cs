using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wpf_playground.Model;

namespace wpf_playground
{
    public class State
    {
        public static UserInfo UserInfo = new UserInfo();
        public static bool DebugMode = false;
        //Default is 6
        public static int ClickCountForEachButton = 6;
    }
}
