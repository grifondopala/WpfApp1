using System;
using System.ComponentModel;

namespace WpfApp1
{
    class ProcessProperty: INotifyPropertyChanged
    {
        private string _process;
        private string _loginProperty;
        private string _mailProperty;
        private string _passwordProperty;
        private string _signInProperty;
        public string Process
        {
            get
            {
                return _process;
            }
            set
            {
                _process = value;
                OnPropertyChanged("Process");
            }
        }

        public string LoginProperty
        {
            get
            {
                return _loginProperty;
            }
            set
            {
                _loginProperty = value;
                OnPropertyChanged("LoginProperty");
            }
        }

        public string MailProperty
        {
            get
            {
                return _mailProperty;
            }
            set
            {
                _mailProperty = value;
                OnPropertyChanged("MailProperty");
            }
        }

        public string PasswordProperty
        {
            get
            {
                return _passwordProperty;
            }
            set
            {
                _passwordProperty = value;
                OnPropertyChanged("PasswordProperty");
            }
        }

        public string SignInProperty
        {
            get
            {
                return _signInProperty;
            }
            set
            {
                _signInProperty = value;
                OnPropertyChanged("SignInProperty");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string p_propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(p_propertyName));
            }
        }
    }
}
