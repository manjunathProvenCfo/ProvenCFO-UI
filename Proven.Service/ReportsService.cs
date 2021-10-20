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
        public async Task<List<ReportsVM>> GetDashboardReports(int agencyId)
        {
            return await GetAsync<List<ReportsVM>>($"Reports/GetDashboardReports?agencyId={agencyId}", readResultDataProp: true);
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
        public APIResult<bool> MakeItMonthlySummary(int Id, int Year, string PeriodType)
        {
            return PostAsync<APIResult<bool>>($"Reports/MakeItMonthlySummary?Id={Id}&year={Year}&PeriodType={PeriodType}").Result;
        }
        public APIResult<bool> Rename(int Id, string FileName)
        {
            return PostAsync<APIResult<bool>>($"Reports/Rename?Id={Id}&FileName={FileName}", null).Result;
        }
        public void Dispose()
        {
            //base.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
