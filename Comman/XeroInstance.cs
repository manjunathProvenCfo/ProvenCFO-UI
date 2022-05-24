using Proven.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Xero.NetStandard.OAuth2;
using Xero.NetStandard.OAuth2.Token;
using Xero.NetStandard.OAuth2.Config;
using Xero.NetStandard.OAuth2.Models;
using Proven.Model;
using QuickBooksSharp;

namespace ProvenCfoUI.Comman
{
    public sealed class AccountingPackageInstance
    {
        private static AccountingPackageInstance _instance = null;
        private static readonly object _syncObject = new object();        
        private AccountingPackageInstance()
        {
            
        }
        public static AccountingPackageInstance Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncObject)
                    {
                        if (_instance == null)
                        {
                            _instance = new AccountingPackageInstance();
                        }
                    }
                }
                return _instance;
            }
        }
        public IXeroToken XeroToken { get; set; }

        public TokenResponse QuickBooksToken { get; set; }
        public string ClientID { get; set; }
        public string ClientSecret { get; set; }
        public string TenentID { get; set; }
        public string XeroAppName { get; set; }
        public string Scope { get; set; }
        public bool ConnectionStatus { get; set; }
        public string ConnectionMessage { get; set; }
        public IAccouningPackage<IXeroToken, TokenInfoVM> XeroService { get; set; }
        public Guid XeroContactIDofProvenCfo { get; set; }
        public ClientModel ClientModel { get; set; }

    }
}
