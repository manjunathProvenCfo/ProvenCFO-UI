using Proven.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xero.NetStandard.OAuth2.Api;
using Xero.NetStandard.OAuth2.Token;

namespace Proven.Service
{
    public static class AccountingApiExtention 
    {
        private static readonly HttpClient xclient = new HttpClient();
        public static Xero.NetStandard.OAuth2.Client.IReadableConfiguration Configuration { get; set; }
        public static Xero.NetStandard.OAuth2.Client.IAsynchronousClient AsynchronousClient { get; set; }
        static AccountingApiExtention()
        {
            xclient.BaseAddress = new Uri(Convert.ToString(ConfigurationManager.AppSettings["xeroapibaseurl"]));
            xclient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            xclient.Timeout = TimeSpan.FromMinutes(10);
            Configuration = Xero.NetStandard.OAuth2.Client.Configuration.MergeConfigurations(
                Xero.NetStandard.OAuth2.Client.GlobalConfiguration.Instance,
                new Xero.NetStandard.OAuth2.Client.Configuration { BasePath = "https://api.xero.com/api.xro/2.0" }
            );
            AsynchronousClient = new Xero.NetStandard.OAuth2.Client.ApiClient(Configuration.BasePath);
        }
        public async static Task<BankStatements> GetBankStatements(this AccountingApi api, string accessToken, string xeroTenantId, string bankAccountID = null, string dateFrom = null, string dateTo = null, string where = null, string order = null, int? page = null,  DateTime? ifModifiedSince = null, string status = null, CancellationToken cancellationToken = default)
        {
            // verify the required parameter 'xeroTenantId' is set
            if (xeroTenantId == null)
                throw new Xero.NetStandard.OAuth2.Client.ApiException(400, "Missing required parameter 'xeroTenantId' when calling AccountingApi->GetPurchaseOrders");


            Xero.NetStandard.OAuth2.Client.RequestOptions requestOptions = new Xero.NetStandard.OAuth2.Client.RequestOptions();

            String[] @contentTypes = new String[] {
            };

            // to determine the Accept header
            String[] @accepts = new String[] {
                "application/json"
            };

            foreach (var cType in @contentTypes)
                requestOptions.HeaderParameters.Add("Content-Type", cType);

            foreach (var accept in @accepts)
                requestOptions.HeaderParameters.Add("Accept", accept);


            if (status != null)
            {
                foreach (var kvp in Xero.NetStandard.OAuth2.Client.ClientUtils.ParameterToMultiMap("", "Status", status))
                {
                    foreach (var value in kvp.Value)
                    {
                        requestOptions.QueryParameters.Add(kvp.Key, value);
                    }
                }
            }

            if (dateFrom != null)
            {
                foreach (var kvp in Xero.NetStandard.OAuth2.Client.ClientUtils.ParameterToMultiMap("", "DateFrom", dateFrom))
                {
                    foreach (var value in kvp.Value)
                    {
                        requestOptions.QueryParameters.Add(kvp.Key, value);
                    }
                }
            }

            if (dateTo != null)
            {
                foreach (var kvp in Xero.NetStandard.OAuth2.Client.ClientUtils.ParameterToMultiMap("", "DateTo", dateTo))
                {
                    foreach (var value in kvp.Value)
                    {
                        requestOptions.QueryParameters.Add(kvp.Key, value);
                    }
                }
            }
            if (where != null)
            {
                foreach (var kvp in Xero.NetStandard.OAuth2.Client.ClientUtils.ParameterToMultiMap("", "where", where))
                {
                    foreach (var value in kvp.Value)
                    {
                        requestOptions.QueryParameters.Add(kvp.Key, value);
                    }
                }
            }

            if (order != null)
            {
                foreach (var kvp in Xero.NetStandard.OAuth2.Client.ClientUtils.ParameterToMultiMap("", "order", order))
                {
                    foreach (var value in kvp.Value)
                    {
                        requestOptions.QueryParameters.Add(kvp.Key, value);
                    }
                }
            }

            if (page != null)
            {
                foreach (var kvp in Xero.NetStandard.OAuth2.Client.ClientUtils.ParameterToMultiMap("", "page", page))
                {
                    foreach (var value in kvp.Value)
                    {
                        requestOptions.QueryParameters.Add(kvp.Key, value);
                    }
                }
            }
            if (xeroTenantId != null)
                requestOptions.HeaderParameters.Add("xero-tenant-id", Xero.NetStandard.OAuth2.Client.ClientUtils.ParameterToString(xeroTenantId)); // header parameter
            if (ifModifiedSince != null)
                requestOptions.HeaderParameters.Add("If-Modified-Since", Xero.NetStandard.OAuth2.Client.ClientUtils.ParameterToString(ifModifiedSince)); // header parameter

            if (bankAccountID != null)
                requestOptions.PathParameters.Add("bankAccountID", bankAccountID);
            
            // authentication (OAuth2) required
            // oauth required
                if (!String.IsNullOrEmpty(accessToken))
            {
                requestOptions.HeaderParameters.Add("Authorization", "Bearer " + accessToken);
            }
            // make the HTTP request

            //BaseService bassss = new BaseService();
            //var response1 = await bassss.GetAsync<BankStatements>("/PurchaseOrders", requestOptions, null, cancellationToken)

            var response = await AsynchronousClient.GetAsync<BankStatements>("/Reports/BankStatement", requestOptions, Configuration);

           

            return response.Data;
        }
    }
}
