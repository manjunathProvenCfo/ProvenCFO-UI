using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proven.Service
{
    public abstract class AccountPackageFactory<T, V>
    {
        public abstract IAccouningPackage<T,V> GetVehicle(string Vehicle);
    }
}
