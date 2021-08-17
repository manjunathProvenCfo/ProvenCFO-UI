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


namespace ProvenCfoUI.Comman
{
    public sealed class XeroInstance
    {
        private static XeroInstance _instance = null;
        private static readonly object _syncObject = new object();        
        private XeroInstance()
        {
            
        }
        public static XeroInstance Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncObject)
                    {
                        if (_instance == null)
                        {
                            _instance = new XeroInstance();
                        }
                    }
                }
                return _instance;
            }
        }
        public IXeroToken XeroToken { get; set; }
        public XeroService XeroService { get; set; }
    }
}