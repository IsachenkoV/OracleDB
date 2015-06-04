using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Caliburn.Micro;
using DataBase.DBLogic;
using DataBase.Properties;
using Screen = Caliburn.Micro.Screen;

namespace DataBase.ViewModels
{
    class TableDataChangeViewModel:Screen
    {
        private IDataBaseServiceProvider _provider;
        private string _tableName;
        private DataTable _content;

        public DataTable Content
        {
            set
            {
                if (!value.Equals(_content))
                    _content = value;
                NotifyOfPropertyChange(() => Content);
            }
            get
            {
                return _content;
            }
        }

        public TableDataChangeViewModel(IDataBaseServiceProvider provider, String tableName)
        {
            _provider = provider;
            _tableName = tableName;
            DisplayName = string.Concat("Change table: ", tableName);
            try
            {
                Content = _provider.GetContentOfTable(_tableName);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, Resources.ErrorMessage);
                TryClose();
            }
        }

        public void Commit()
        {
            bool success = true;
            try
            {
                _provider.UpdateContentOfTable(_tableName, Content);
            }
            catch (Exception e)
            {
                success = false;
                MessageBox.Show(e.Message, Resources.ErrorMessage);
            }

            if (success)
                MessageBox.Show(Resources.CompletedSuccessfully, Resources.SuccessMessage);
        }

        ~TableDataChangeViewModel()
        {
            _content.Dispose();
        }
    }
}
