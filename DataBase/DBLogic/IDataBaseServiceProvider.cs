using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace DataBase.DBLogic
{
    interface IDataBaseServiceProvider
    {
        List<string> GetListOfTables();
        DbDataReader ExecuteCommand(string command);
        List<string> GetFieldsOfTable(string tableName);
        bool HaveChanges { set; get; }
    }
}
