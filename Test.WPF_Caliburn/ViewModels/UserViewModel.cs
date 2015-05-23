using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.WPF_Caliburn.ViewModels
{
    class UserViewModel : PropertyChangedBase
    {
        public string Name
        {
            set;
            get;
        }

        private string _surname;

        public string Surname
        {
            set
            {
                if (value == _surname)
                    return;
                _surname = value;
                NotifyOfPropertyChange( ( ) => Surname );
            }
            get
            {
                return _surname;
            }
        }
    }
}
