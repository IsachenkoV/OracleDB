using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

using System.Windows;

namespace Test.WPF_Caliburn.ViewModels
{
    public delegate void Delegate( );

	class MainViewModel : PropertyChangedBase
	{
        private string _stub;

        public string Stub
        {
            set
            {
                if (value == _stub)
                    return;
                _stub = value;
                NotifyOfPropertyChange(() => Stub);
            }
            get
            {
                return _stub;
            }
        }

        public UserViewModel User
        {
            set;
            get;
        }

        public void DoMagic( )
        {
            _manager.ShowDialog( User );
        }

        private IWindowManager _manager;

        public MainViewModel( IWindowManager manager )
        {
            _manager = manager;
            User = new UserViewModel() { Name = "Petya", Surname = "Vasechkin" };
        }
	}
}
