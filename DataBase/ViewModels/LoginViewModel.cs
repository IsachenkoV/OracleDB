using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using DataBase.DBLogic;
using DataBase.Views;

namespace DataBase.ViewModels
{
    class LoginViewModel : Screen
    {
        private readonly IWindowManager _manager;

        public LoginViewModel(IWindowManager manager)
        {
            _manager = manager;
            DisplayName = "Login form";
        }

        public string IpAddress { set; get; }
        public string Tablespace { set; get; }
        public string Name { set; get; }

        public string Password
        {
            get
            {
                var view = GetView() as LoginView;
                return view != null ? view.PassBox.Password : null;
            }
        }

        public void LogIn()
        {
            //try to connect to database
            IDataBaseServiceProvider provider = null;
            try
            {
                provider = new OracleDataBaseServiceProvider(IpAddress, Tablespace, Name, Password);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Ошибка входа");
                return;
            }

            _manager.ShowWindow(new MainWindowViewModel(_manager, provider, Name, Tablespace));
            TryClose();
        }
    }
}
