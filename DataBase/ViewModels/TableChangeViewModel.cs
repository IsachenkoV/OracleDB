using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using DataBase.DBLogic;

namespace DataBase.ViewModels
{
    class TableChangeViewModel:Screen
    {
        private IWindowManager _manager;
        private IDataBaseServiceProvider _provider;
        private string _tableName;
        public TableChangeViewModel(IWindowManager manager, IDataBaseServiceProvider provider, String tableName)
        {
            _manager = manager;
            _provider = provider;
            _tableName = tableName;
        }

        //TODO Вьюха для изменения таблицы. я ничего не сделяль.
    }
}
