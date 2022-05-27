using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proven.Service.AccountingPackage
{
    class XeroData : AccountingPackageData
    {
        //private dynamic _Token;
        public string _ClientID;
        public  string _ClientSecret;
        public  string _TenentID;
        public readonly string _AppName;
        public readonly string _AppType;
        public  string _Scope;
        //public  bool _ConnectionStatus;
        //public  string _ConnectionMessage;
        public XeroData(string ClientID, string ClientSecret, string Scope, string TenentID)
        {
            _AppType = "Xero";
            _AppName = "ProvenCFO";
            _ClientID = ClientID;
            _ClientSecret = ClientSecret;
            _TenentID = TenentID;
            _Scope = Scope;                           
        }
        public override string AppType
        {
            get { return _AppType; }
        }
        public override string AppName
        {
            get { return _AppName; }
        }
        public override string ClientID
        {
            get { return _ClientID; }
            set { _ClientID = value; }
        }
        public override string ClientSecret
        {
            get { return _ClientSecret; }
            set { _ClientSecret = value; }
        }
        public override string TenentID
        {
            get { return _TenentID; }
            set { _TenentID = value; }
        }
        public override string Scope
        {
            get { return _Scope; }
            set { _Scope = value; }
        }

        
    }
}
