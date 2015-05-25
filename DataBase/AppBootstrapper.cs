using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro.Autofac;
using DataBase.ViewModels;

namespace DataBase
{
    class AppBootstrapper : AutofacBootstrapper<LoginViewModel>
    {
    }
}
