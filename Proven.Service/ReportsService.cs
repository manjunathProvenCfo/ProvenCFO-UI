using Proven.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proven.Service
{
    public class ReportsService : BaseService, IDisposable
    {
        private bool isDisposed = false;

        public ReportsVM SaveReport(ReportsVM report)
        {
            return PostAsync<ReportsVM, ReportsVM>("Reports/SaveReport", report, readResultDataProp: true).Result;
        }
        public List<ReportsVM> GetReports(int agencyId, int year, string periodType)
        {
            return GetAsync<List<ReportsVM>>($"Reports/GetReports?agencyId={agencyId}&year={year}&periodType={periodType}", readResultDataProp: true).Result;
        }
        public void Dispose()
        {
            //base.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
