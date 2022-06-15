﻿using System;
using System.Linq;
using System.Net.Http;

namespace QuickBooksSharp
{
    public class QuickBooksException : Exception
    {
        public HttpResponseMessage Response { get; }

        public string ResponseContent { get; }

        /// <summary>
        /// HTTP 401
        /// </summary>
        public bool IsUnauthorized => (int)Response.StatusCode == 401;

        /// <summary>
        /// HTTP 403
        /// </summary>
        public bool IsForbidden => (int)Response.StatusCode == 403;

        /// <summary>
        /// HTTP 429
        /// </summary>
        public bool IsRateLimit => (int)Response.StatusCode == 429;

        public string? IntuitTId => GetHeaderValue(Response, "intuit_tid");

        public string? QBOVersion => GetHeaderValue(Response, "QBO-Version");

        private static string? GetHeaderValue(HttpResponseMessage r, string headerName) => r.Headers.TryGetValues(headerName, out var values) ? values.FirstOrDefault() : null;

        public QuickBooksException(HttpResponseMessage response, string responseContent)
            : base($@"QuickBooks API call failed with code: {response.StatusCode}
IntuiTId: {GetHeaderValue(response, "intuit_tid")},
Reason: {response.ReasonPhrase}
Content: {responseContent}")
        {
            this.Response = response;
            this.ResponseContent = responseContent;
        }
    }
}
