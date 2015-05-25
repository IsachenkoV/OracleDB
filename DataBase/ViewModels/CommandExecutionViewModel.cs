using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using DataBase.DBLogic;

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
        }

        public string Command { set; get; }

        public void Execute()
        {
            
            try
            {
                DbDataReader reader = _provider.ExecuteCommand(Command);
                if (reader.VisibleFieldCount == 0)
                    MessageBox.Show("Command completed successfully", "Success!");
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
