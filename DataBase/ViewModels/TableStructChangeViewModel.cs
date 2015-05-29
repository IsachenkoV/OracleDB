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
    class TableStructChangeViewModel : Screen
    {
        private readonly IDataBaseServiceProvider _provider;
        private List<string> _tableList;
        private string _tableName, _oldName;
        private ObservableCollection<Column> _columns;
        private ObservableCollection<Column> _oldColumns;
        private int _wasColumnsLength;
        private bool[] _wasColumns;

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

        public ObservableCollection<Column> Columns {
            set
            {
                if (_columns==null || !_columns.Equals(value))
                {
                    _columns = value;
                    NotifyOfPropertyChange(()=>Columns);
                }
            }
            get
            {
                return _columns;
            }
        }

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
            #region gettingData
            using (var dataReader = _provider.GetInfoOfTable(TableName))
            {
                var names = from DbDataRecord row in dataReader
                    select new Column()
                    {
                        Name = row["column_name"].ToString(),
                        Type = row["data_type"].ToString(),
                        ForeignKeyColumn = row["source_column"].ToString(),
                        ForeignKeyTable = row["source_table"].ToString(),
                        IsNotifying = true,
                        IsNotNull = !row["nullable"].ToString().Equals("Y"),
                        IsPrimaryKey = row["primary_key"].ToString().Equals("P"),
                        ForeignKey = row["foreign_key"].ToString().Equals("R"),
                        ForKeyName = row["fk_symbol"].ToString()
                    };
                var collection = names as Column[] ?? names.ToArray();
                _wasColumnsLength = collection.Length;
                for (int i = 0; i < _wasColumnsLength; i++)
                {
                    collection[i].Id = i;
                }
                _wasColumns = new bool[_wasColumnsLength];

                Columns = new ObservableCollection<Column>(collection);

                _oldColumns = new ObservableCollection<Column>();
                foreach (var c in collection)
                {
                    _oldColumns.Add(new Column(c));
                }
            }
            #endregion
        }

        public void AddColumn()
        {
            Column c = new Column {Name=null, IsNotNull = false, ForeignKeyTable = null, ForeignKeyColumn = null, IsPrimaryKey = false, Id = -1};
            Columns.Add(c);
            NotifyOfPropertyChange(()=>Columns);
        }

        public void Save()
        {
            bool wasPK = _oldColumns.Any(c => c.IsPrimaryKey);

            //Изменяем имя
            if (TableName != _oldName)
            {
                var text = string.Concat("alter table ", _oldName, " rename to ", TableName);
                try
                {
                    _provider.ExecuteCommand(text);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, Resources.ErrorMessage);
                    return;
                }
                _provider.HaveChanges = true;
            }

            bool pKeyHaveChanges = false;
            List<string> primaryKeys = new List<string>();
            List<Column> foreignKeys = new List<Column>();

            //Добавляем новые колонки
            foreach (var c in Columns.Where(c => c.Id == -1))
            {
                if (string.IsNullOrWhiteSpace(c.Name) || string.IsNullOrWhiteSpace(c.Type))
                    continue;
                var text = string.Concat("alter table ", TableName, " add ", c.Name, " ", c.Type);
                if (c.IsNotNull)
                    text = string.Concat(text, " not null");

                try
                {
                    _provider.ExecuteCommand(text);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, Resources.ErrorMessage);
                    return;
                }

                if (c.IsPrimaryKey)
                {
                    primaryKeys.Add(c.Name);
                    pKeyHaveChanges = true;
                }
                if (!string.IsNullOrWhiteSpace(c.ForeignKeyTable) && !string.IsNullOrWhiteSpace(c.ForeignKeyColumn))
                    foreignKeys.Add(c);
            }

            //Изменяем старые колонки а) есть изменения б) колонку удалили

            //a)
            foreach (var c in Columns.Where(c => c.Id != -1))
            {
                _wasColumns[c.Id] = true;
                Column prev = _oldColumns[c.Id];
                if (c.Name != prev.Name)
                {
                    var text = string.Concat("alter table ", TableName, " rename column ", prev.Name, " to ", c.Name);
                    try
                    {
                        _provider.ExecuteCommand(text);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message, Resources.ErrorMessage);
                        return;
                    }
                }

                if (prev.Type != c.Type || prev.IsNotNull != c.IsNotNull)
                {
                    var text = string.Concat("alter table ", TableName, " modify ", c.Name, " ", c.Type);

                    if (prev.IsNotNull != c.IsNotNull)
                    {
                        text = string.Concat(text, c.IsNotNull ? " not null" : " null");
                    }

                    try
                    {
                        _provider.ExecuteCommand(text);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message, Resources.ErrorMessage);
                        return;
                    }
                }

                if (c.IsPrimaryKey != prev.IsPrimaryKey)
                    pKeyHaveChanges = true;

                if (c.IsPrimaryKey)
                    primaryKeys.Add(c.Name);

                if (prev.ForeignKeyTable != c.ForeignKeyTable || prev.ForeignKeyColumn != c.ForeignKeyColumn)
                    foreignKeys.Add(c);
            }

            for (int i = 0; i < _wasColumnsLength; i++)
            {
                if (!_wasColumns[i])
                {
                    Column c = _oldColumns[i];
                    if (c.IsPrimaryKey)
                        pKeyHaveChanges = true;
                    if (!string.IsNullOrWhiteSpace(c.ForeignKeyTable))
                    {
                        var text = string.Concat("alter table ", TableName, " drop constraint ", c.ForKeyName);
                        try
                        {
                            _provider.ExecuteCommand(text);
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.Message, Resources.ErrorMessage);
                            return;
                        }
                    }
                }
            }

            //здесь мы должны удалить старый первичный ключ и сделать новый
            if (pKeyHaveChanges)
            {
                string text;
                if (wasPK)
                {
                    text = string.Concat("alter table ", TableName, " drop primary key");
                    try
                    {
                        _provider.ExecuteCommand(text);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message, Resources.ErrorMessage);
                        return;
                    }
                }
                if (primaryKeys.Count > 0)
                {
                    text = string.Concat("alter table ", TableName, " add primary key ( ");
                    var pks = "";
                    foreach (var key in primaryKeys)
                    {
                        if (!string.IsNullOrWhiteSpace(pks))
                            pks = string.Concat(pks, ", ");
                        pks = string.Concat(pks, key);
                    }
                    text = string.Concat(text, pks, ")");
                    try
                    {
                        _provider.ExecuteCommand(text);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message, Resources.ErrorMessage);
                        return;
                    }
                }
            }

            //а также удалить старые ссылки и добавить новые
            foreach (var c in foreignKeys)
            {
                if (c.Id == -1) // new
                {
                    var text = string.Concat("alter table ", TableName, " add foreign key ( ", c.Name, " ) references ",
                        c.ForeignKeyTable, "( ", c.ForeignKeyColumn, " ) ");
                    try
                    {
                        _provider.ExecuteCommand(text);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message, Resources.ErrorMessage);
                        return;
                    }
                }
                else //change
                {
                    //delete old
                    string text;
                    if (!string.IsNullOrWhiteSpace(c.ForeignKeyTable))
                    {
                        text = string.Concat("alter table ", TableName, " drop constraint ", c.ForKeyName);
                        try
                        {
                            _provider.ExecuteCommand(text);
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.Message, Resources.ErrorMessage);
                            return;
                        }
                    }

                    //create new
                    if (!string.IsNullOrWhiteSpace(c.ForeignKeyTable) && !string.IsNullOrWhiteSpace(c.ForeignKeyColumn))
                    {
                        text = string.Concat("alter table ", TableName, " add foreign key ( ", c.Name, " ) references ",
                            c.ForeignKeyTable, "( ", c.ForeignKeyColumn, " ) ");
                        try
                        {
                            _provider.ExecuteCommand(text);
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.Message, Resources.ErrorMessage);
                            return;
                        }
                    }
                }
            }

            //b)
            for (int i = 0; i < _wasColumnsLength; i++)
            {
                if (!_wasColumns[i])
                {
                    Column c = _oldColumns[i];
                    var text = string.Concat("alter table ", TableName, " drop column ", c.Name);
                    try
                    {
                        _provider.ExecuteCommand(text);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message, Resources.ErrorMessage);
                        return;
                    }
                }
            }
            MessageBox.Show(Resources.CompletedSuccessfully, Resources.SuccessMessage);
            TryClose();
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
