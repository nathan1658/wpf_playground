﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
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
using wpf_playground.Model;

namespace wpf_playground
{
    /// <summary>
    /// Interaction logic for UserInfoPage.xaml
    /// </summary>
    public partial class UserInfoPage : Window
    {
        public UserInfoPage()
        {
            InitializeComponent();
            var vm = new UserInfoPageViewModel();
            vm.CloseAction = () =>
            {
                this.Close();
            };
            this.DataContext = vm;
        }


    }
    public class UserInfoPageViewModel : INotifyPropertyChanged
    {

        public UserInfoPageViewModel()
        {
            this.MaleChecked = true;
            this.L01Checked = true;
            this.LeftHandChecked = true;
        }

        public UserInfo UserInfo
        {
            get
            {
                return State.UserInfo;
            }
        }

        private bool _maleChecked;
        public bool MaleChecked
        {
            get { return _maleChecked; }
            set
            {
                _maleChecked = value;
                if (value)
                    UserInfo.Gender = "Male";
                InformPropertyChanged("UserInfo");
                InformPropertyChanged("FormValid");
            }
        }

        private bool _femaleChecked;
        public bool FemaleChecked
        {
            get { return _femaleChecked; }
            set
            {
                _femaleChecked = value;
                if (value)
                    UserInfo.Gender = "Female";
                InformPropertyChanged("UserInfo");
                InformPropertyChanged("FormValid");


            }
        }

        private bool _l01Checked;

        public bool L01Checked
        {
            get { return _l01Checked; }
            set
            {
                _l01Checked = value;
                if (value) UserInfo.Group = "L01";
                InformPropertyChanged("UserInfo");
                InformPropertyChanged("FormValid");

            }
        }
        private bool _l02Checked;

        public bool L02Checked
        {
            get { return _l02Checked; }
            set
            {
                _l02Checked = value;
                if (value) UserInfo.Group = "L02";
                InformPropertyChanged("UserInfo");
                InformPropertyChanged("FormValid");

            }
        }
        private bool _l03Checked;

        public bool L03Checked
        {
            get { return _l03Checked; }
            set
            {
                _l03Checked = value;
                if (value) UserInfo.Group = "L03";
                InformPropertyChanged("UserInfo");
                InformPropertyChanged("FormValid");

            }
        }

        private bool _leftHandChecked;
        public bool LeftHandChecked
        {
            get { return _leftHandChecked; }
            set
            {
                _leftHandChecked = value;
                if (value) UserInfo.DominantHand = "Left";
                InformPropertyChanged("UserInfo");
                InformPropertyChanged("FormValid");


            }
        }

        private bool _rightHandChecked;
        public bool RightHandChecked
        {
            get { return _rightHandChecked; }
            set
            {
                _rightHandChecked = value;
                if (value) UserInfo.DominantHand = "Right";
                InformPropertyChanged("UserInfo");
                InformPropertyChanged("FormValid");

            }
        }

        public bool FormValid
        {
            get
            {
                var list = new List<String>() {
                    UserInfo.Name, UserInfo.SID, UserInfo.Age, UserInfo.Gender, UserInfo.Group, UserInfo.DominantHand};
                return !list.Any(x => String.IsNullOrEmpty(x));
            }
        }



        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                UserInfo.Name = value;
                InformPropertyChanged("UserInfo");
                InformPropertyChanged("FormValid");

            }
        }
        private string _sid;
        public string SID
        {
            get { return _sid; }
            set
            {
                _sid = value;
                UserInfo.SID = value;
                InformPropertyChanged("UserInfo");
                InformPropertyChanged("FormValid");

            }
        }

        private string _age;

        public string Age
        {
            get { return _age; }
            set
            {
                _age = value;
                UserInfo.Age = value;
                InformPropertyChanged("UserInfo");
                InformPropertyChanged("FormValid");

            }
        }
        private ICommand _clickCommand;
        public ICommand ClickCommand
        {
            get
            {
                return _clickCommand ?? (_clickCommand = new CommandHandler(() =>
                {
                    CloseAction();
                }, () => true));
            }
        }
        public Action CloseAction { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void InformPropertyChanged([CallerMemberName] String propName = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
