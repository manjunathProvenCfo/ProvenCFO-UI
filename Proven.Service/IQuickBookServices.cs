using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intuit.Ipp.Core;

namespace Proven.Service
{
    public interface IQuickBookServices
    {
        Task QBOApiCall(Action<ServiceContext> apiCallFunction);
    }
}
