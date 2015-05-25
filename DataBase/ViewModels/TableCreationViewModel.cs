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
using Screen = Caliburn.Micro.Screen;

namespace DataBase.ViewModels
{
    class TableCreationViewModel : Screen
    {
        private IWindowManager _manager;
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
                if (k <= 0 || k >= Columns.Count) return res;
                string tableName = Columns[k].ForeignKeyTable;
                res = _provider.GetFieldsOfTable(tableName);
                return res;
            }
        }

        public BindableCollection<string> TypeList { set; get; }

        public TableCreationViewModel(IWindowManager manager, IDataBaseServiceProvider provider)
        {
            _manager = manager;
            _provider = provider;
            Columns = new ObservableCollection<Column>();
            TableList = new List<string>();
            TypeList = new BindableCollection<string>
            {
                "binary_double",
                "binary_float",
                "char",
                "date",
                "long",
                "number",
                "varchar",
                "varchar2"
            };
        }

        public void AddColumn()
        {
            Column c = new Column {Name=null, IsNotNull = false, ForeignKeyTable = null, ForeignKeyColumn = null, IsPrimaryKey = false};
            Columns.Add(c);
            NotifyOfPropertyChange(()=>Columns);
        }

        public void Save()
        {
            //TODO формируем запрос и кидаемся в _provider
        }
    }
}
