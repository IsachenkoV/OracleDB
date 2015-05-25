using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Oracle.ManagedDataAccess.Client;

namespace DataBase.DBLogic
{
    class OracleDataBaseServiceProvider : IDataBaseServiceProvider
    {
        private OracleConnection _connection;
        private string _tablespace;
        private string _userName;
        private List<string> _tableNames;
        private BindableCollection<string> _observableTableCollection;

        public OracleDataBaseServiceProvider(string ip, string tblspace, string user, string pass)
        {
            _tablespace = tblspace;
            _userName = user;
            var b = new OracleConnectionStringBuilder
            {
                DataSource = ip,
                UserID = user,
                Password = pass,
                ValidateConnection = true
            };

            _connection = new OracleConnection(b.ToString());
            _connection.Open();
        }

        public List<string> GetFieldsOfTable(string tableName)
        {
            List<string> fields = null;
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = string.Format("select column_name, owner from all_tab_columns where table_name = '{0}'", tableName);
                using (var reader = command.ExecuteReader())
                {
                    var names = from DbDataRecord row in reader
                        where !row[row.GetOrdinal("owner")].ToString().Equals("SYS")
                        orderby row[row.GetOrdinal("column_name")]
                        select row[row.GetOrdinal("column_name")].ToString();
                    fields = names.ToList();
                }   
            }
            return fields;
        }

        public List<string> GetListOfTables()
        {
            if (_tableNames != null)
                return _tableNames;

            _tableNames = new List<string>();

            using (var command = _connection.CreateCommand())
            {
                command.CommandText = "select * from ALL_TABLES";

                using (var reader = command.ExecuteReader())
                {
                    var names = from DbDataRecord row in reader
                                where !row[row.GetOrdinal("owner")].ToString().Equals("SYS")
                                && (_tablespace == null || row[row.GetOrdinal("tablespace_name")].ToString().Equals(_tablespace))
                                orderby row[row.GetOrdinal("table_name")]
                                select row[row.GetOrdinal("table_name")].ToString();

                    _tableNames = names.ToList();
                }
            }
            return _tableNames;
        }

        public DbDataReader ExecuteCommand(string command)
        {
            using (var c = _connection.CreateCommand())
            {
                c.CommandText = command;
                return c.ExecuteReader();
            }
        }
    }
}
