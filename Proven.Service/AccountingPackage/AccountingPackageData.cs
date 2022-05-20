using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proven.Service.AccountingPackage
{
    abstract class AccountingPackageData
    {
        //public abstract dynamic Token { get; set; }
        public abstract string ClientID { get; set; }
        public abstract string ClientSecret { get; set; }
        public abstract string TenentID { get; set; }
        public abstract string AppName { get;  }
        public abstract string AppType { get;  }
        public abstract string Scope { get; set; }
        //public abstract bool ConnectionStatus { get; set; }
        //public abstract string ConnectionMessage { get; set; }
    }
}
