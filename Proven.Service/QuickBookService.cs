using System;
using System.IO;
using System.Threading.Tasks;
using Intuit.Ipp.Core;
using Intuit.Ipp.Core.Configuration;
using Intuit.Ipp.Exception;
using Intuit.Ipp.OAuth2PlatformClient;
using Intuit.Ipp.Security;
//using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Proven.Service
{
    public class QuickBookService : BaseService, IQuickBookServices
    {
        //string _ClientId;
        //string _ClientSecret;
        //string _scopes;
        //string _callbackurl;
        //private readonly TokensContext _tokens;
        //private readonly OAuth2Keys _auth2Keys;
        //public QuickBookService(TokensContext tokens, IOptions<OAuth2Keys> auth2Keys)
        //{
        //    _tokens = tokens;
        //    _auth2Keys = auth2Keys.Value;
        //}
        //public Task QBOApiCall(Action<ServiceContext> apiCallFunction)
        //{
        //    throw new NotImplementedException();
        //}
        public Task QBOApiCall(Action<ServiceContext> apiCallFunction)
        {
            throw new NotImplementedException();
        }
    }
}
