using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using DataBase.DBLogic;
using DataBase.Properties;

namespace DataBase.ViewModels
{
    class MainWindowViewModel : Screen
    {
        private readonly IWindowManager _manager;
        private readonly IDataBaseServiceProvider _provider;
        private string _curTableName;
 
        public string CurTableName {
            set
            {
                if (value == _curTableName)
                    return;
                _curTableName = value;
                NotifyOfPropertyChange(() => CurTableName);
                NotifyOfPropertyChange(() => TableNames);
            }
            get { return _curTableName; }
        }

        public IEnumerable<string> TableNames
        {
            get
            {
                return _curTableName == null ? _provider.GetListOfTables() : _provider.GetListOfTables().Where(s => s.ToLower().Contains(_curTableName.ToLower())).ToList();
            }
        }

        public MainWindowViewModel(IWindowManager manager, IDataBaseServiceProvider provider, string name, string tablespace)
        {
            _manager = manager;
            _provider = provider;
            DisplayName = string.Concat("User: ", name, "        ", "Tablespace: ", tablespace);
        }

        public void Logout()
        {
            _manager.ShowWindow(new LoginViewModel(_manager));
            TryClose();
        }

        public void CreateNewTable()
        {
            _manager.ShowWindow(new TableCreationViewModel(_provider));
        }

        public void ChangeTableData()
        {
            if (CurTableName!= null && TableNames.Contains(CurTableName.ToUpper()))
            {
                _manager.ShowWindow(new TableDataChangeViewModel(_manager, _provider, CurTableName));
            }
            else
            {
                MessageBox.Show(Resources.NoThatTable, Resources.ErrorMessage);
            }
        }

        public void ChangeTableStruct()
        {
            if (CurTableName != null && TableNames.Contains(CurTableName.ToUpper()))
            {
                _manager.ShowWindow(new TableStructChangeViewModel());
            }
            else
            {
                MessageBox.Show(Resources.NoThatTable, Resources.ErrorMessage);
            }            
        }

        public void ExecuteCommand()
        {
            _manager.ShowWindow(new CommandExecutionViewModel(_manager, _provider));
        }
    }
}
