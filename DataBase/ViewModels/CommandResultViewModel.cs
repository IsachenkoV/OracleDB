using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Oracle.ManagedDataAccess.Client;

namespace DataBase.ViewModels
{
    class CommandResultViewModel :Screen
    {
        public DbDataReader DataReader
        {
            get; private set; }

        public CommandResultViewModel(DbDataReader dr)
        {
            DataReader = dr;
        }

        ~CommandResultViewModel()
        {
            DataReader.Dispose();
        }
    }
}
