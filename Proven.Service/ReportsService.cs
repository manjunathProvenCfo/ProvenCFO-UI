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
        public APIResult<bool> SoftDeleteFile(int id)
        {
            return PostAsync<APIResult<bool>>($"Reports/SoftDeleteFile?Id={id}", null).Result;
        }
        public APIResult<bool> UpdatePositions(int[] Ids, int[] Positions)
        {
            var positions = new { Ids, Positions };
            return PostAsync<APIResult<bool>, object>("Reports/UpdatePositions", positions).Result;
        }
        public APIResult<bool> Delete(int[] Ids)
        {
            return PostAsync<APIResult<bool>, int[]>("Reports/Delete", Ids).Result;
        }
        public void Dispose()
        {
            //base.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
