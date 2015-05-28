using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using DataBase.DBLogic;
using DataBase.Properties;

namespace DataBase.ViewModels
{
    class CommandExecutionViewModel:Screen
    {
        private IDataBaseServiceProvider _provider;
        private IWindowManager _manager;

        public CommandExecutionViewModel(IWindowManager manager, IDataBaseServiceProvider provider)
        {
            _provider = provider;
            _manager = manager;
            DisplayName = "Execute command:";
        }

        public string Command { set; get; }

        public void Execute()
        {
            
            try
            {
                DbDataReader reader = _provider.ExecuteCommand(Command);
                if (reader.VisibleFieldCount == 0)
                {
                    MessageBox.Show(Resources.CompletedSuccessfully, Resources.SuccessMessage);
                    _provider.HaveChanges = true;
                }
                else
                    _manager.ShowWindow(new CommandResultViewModel(reader));
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
            
        }
    }
}
