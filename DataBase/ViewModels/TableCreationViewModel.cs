using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Caliburn.Micro;
using DataBase.DBLogic;
using DataBase.Models;
using DataBase.Properties;
using Screen = Caliburn.Micro.Screen;

namespace DataBase.ViewModels
{
    class TableCreationViewModel : Screen
    {
        private readonly IDataBaseServiceProvider _provider;
        private List<string> _tableList;

        public string TableName { set; get; }
        public ObservableCollection<Column> Columns { set; get; }

        public string Index { set; get; }

        public List<string> TableList
        {
            set
            {
                _tableList = _provider.GetListOfTables();
                NotifyOfPropertyChange(()=>TableList);
            }
            get { return _tableList;}
        }

        public List<string> FieldList
        {
            get
            {
                int k = -1;
                List<string> res = null;
                if (!int.TryParse(Index, out k)) return res;
                if (k < 0 || k >= Columns.Count) return res;
                string tableName = Columns[k].ForeignKeyTable;
                res = _provider.GetFieldsOfTable(tableName);
                return res;
            }
        }

        public BindableCollection<string> TypeList { set; get; }

        public TableCreationViewModel(IDataBaseServiceProvider provider)
        {
            _provider = provider;
            Columns = new ObservableCollection<Column>();
            TableList = new List<string>();
            TypeList = new BindableCollection<string>
            {
                "binary_double",
                "binary_float",
                "char",
                "date",
                "number",
                "varchar(255)",
                "varchar2(255)"
            };
            DisplayName = "Create your table!";
        }

        public void AddColumn()
        {
            Column c = new Column {Name=null, IsNotNull = false, ForeignKeyTable = null, ForeignKeyColumn = null, IsPrimaryKey = false};
            Columns.Add(c);
            NotifyOfPropertyChange(()=>Columns);
        }

        public void Save()
        {
            string commandText = string.Format("create table {0} ( ", TableName);
            string columnsRes = "";
            string primaryKeys = "";
            string foreignKeys = "";

            foreach (var c in Columns)
            {
                if (string.IsNullOrWhiteSpace(c.Type) || string.IsNullOrWhiteSpace(c.Name))
                    continue;
                if (columnsRes != "")
                    columnsRes = string.Concat(columnsRes, ",");

                columnsRes = string.Concat(columnsRes, c.Name, " ", c.Type, " ");
                if (c.IsPrimaryKey)
                {
                    if (primaryKeys != "")
                        primaryKeys = string.Concat(primaryKeys, ", ");
                    primaryKeys = string.Concat(primaryKeys, c.Name);
                }
                    
                if (c.IsNotNull)
                    columnsRes = string.Concat(columnsRes, "not null ");

                if (c.ForeignKeyTable != null && c.ForeignKeyColumn != null)
                    foreignKeys = string.Concat(foreignKeys, ",foreign key (", c.Name, ") references ",
                        c.ForeignKeyTable, "(", c.ForeignKeyColumn, ") ");
            }
            if (primaryKeys != "")
                primaryKeys = string.Concat(" , primary key (", primaryKeys, ")");

            commandText = string.Concat(commandText, columnsRes, primaryKeys, foreignKeys, " )");

            bool errorOccured = false;
            try
            {
                _provider.ExecuteCommand(commandText);
            }
            catch (Exception e)
            {
                errorOccured = true;
                MessageBox.Show(e.Message, Resources.ErrorMessage);
            }

            if (!errorOccured)
            {
                MessageBox.Show(Resources.Completed_successfully, Resources.SuccessMessage);
                _provider.HaveChanges = true;
            }
        }
    }
}
