using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using DataBase.DBLogic;

namespace DataBase.Models
{
    class Column : PropertyChangedBase
    {
        private string _name;
        private bool _isPrimaryKey;
        private bool _isNotNull;
        private string _foreignKeyTable;
        private string _foreignKeyColumn;
        private string _type;

        public string Name
        {
            set
            {
                if (_name == value)
                    return;
                _name = value;
                NotifyOfPropertyChange(() => Name);
            }
            get { return _name; }
        }

        public bool IsPrimaryKey
        {
            set
            {
                if (_isPrimaryKey == value)
                    return;
                _isPrimaryKey = value;
                NotifyOfPropertyChange(() => IsPrimaryKey);
            }
            get { return _isPrimaryKey; }
        }
        public bool IsNotNull
        {
            set
            {
                if (_isNotNull == value)
                    return;
                _isNotNull = value;
                NotifyOfPropertyChange(() => IsNotNull);
            }
            get { return _isNotNull; }
        }

        //public BindableCollection<string> ForeignKeyTable { set; get; }
        //public BindableCollection<string> ForeignKeyColumn { set; get; } 

        public string ForeignKeyTable
        {
            set
            {
                if (_foreignKeyTable == value)
                    return;
                _foreignKeyTable = value;
                NotifyOfPropertyChange(() => ForeignKeyTable);
            }
            get { return _foreignKeyTable; }
        }
        public string ForeignKeyColumn
        {
            set
            {
                if (_foreignKeyColumn == value)
                    return;
                _foreignKeyColumn = value;
                NotifyOfPropertyChange(() => ForeignKeyColumn);
            }
            get { return _foreignKeyColumn; }
        }

        public string Type
        {
            set
            {
                if (_type == value)
                    return;
                _type = value;
                NotifyOfPropertyChange(() => Type);
            }
            get { return _type; }
        }
    }
}
