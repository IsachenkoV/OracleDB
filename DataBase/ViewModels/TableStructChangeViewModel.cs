using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    class TableStructChangeViewModel : Screen
    {
        private readonly IDataBaseServiceProvider _provider;
        private List<string> _tableList;
        private string _tableName, _oldName;

        public string TableName
        {
            set
            {
                if (value == null || value == _tableName) return;
                _tableName = value;
                NotifyOfPropertyChange(()=>TableName);
            }
            get
            {
                return _tableName;
            }
        }

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

        public TableStructChangeViewModel(IDataBaseServiceProvider provider, string tableName)
        {
            _oldName = tableName;
            _provider = provider;
            _tableName = tableName;
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
            DisplayName = string.Format("Change table {0}", TableName);

            //TODO здесь вытягивать и отображать текущие столбцы
        }

        public void AddColumn()
        {
            Column c = new Column {Name=null, IsNotNull = false, ForeignKeyTable = null, ForeignKeyColumn = null, IsPrimaryKey = false};
            Columns.Add(c);
            NotifyOfPropertyChange(()=>Columns);
        }

        public void Save()
        {
            //TODO здесь сохранять измененную таблицу
        }

        public void Delete()
        {
            var errorOccured = false;
            try
            {
                _provider.DeleteTable(_oldName);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, Resources.ErrorMessage);
                errorOccured = true;
            }

            if (errorOccured) return;

            _provider.HaveChanges = true;
            MessageBox.Show(Resources.DeleteSuccess, Resources.SuccessMessage);
            TryClose();
        }
    }
}
