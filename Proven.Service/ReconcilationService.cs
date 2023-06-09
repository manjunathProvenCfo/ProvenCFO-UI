﻿using CFO.Model.ViewModels;
using Proven.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Proven.Service
{
    public class ReconcilationService : BaseService, IDisposable
    {
        private bool isDisposed = false;
        private HttpResponseMessage response;
        private StringContent content;


        public ReconciliationMainModel GetReconciliation(int ClientId, string type, int Isreconciled, string userId, string userEmail)
        {
            var a = GetAsync<ReconciliationMainModel>("Reconciliation/GetReconciliation?ClientId=" + ClientId + "&type=" + type + "&Isreconciled=" + Isreconciled + $"&userId={userId}&userEmail={userEmail}").Result;
            return a;
        }
        public ReconciliationMainModelPaging GetReconciliationList(ReconciliationFilterVW filter)
        {

            return PostAsync<ReconciliationMainModelPaging,ReconciliationFilterVW>("Reconciliation/GetReconciliationPagingList",filter).Result;
        }
        public ReconciliationMainModel GetFilteredReconcilation(ReconciliationfilterModel Filter)
        {
            return PostAsync<ReconciliationMainModel, ReconciliationfilterModel>("Reconciliation/GetFilteredReconciliation", Filter).Result;
        }
        public ReconciliationCountModel GetReconciliationDataCountAgencyId(string AgencyId)
        {
            return GetAsync<ReconciliationCountModel>("Reconciliation/GetReconciliationDataCountAgencyId?AgencyId=" + AgencyId).Result;
        }
        public ReconciliationCountModel GetReconciliationDashboardDataAgencyId(string AgencyID, string type)
        {
            return GetAsync<ReconciliationCountModel>("Reconciliation/GetReconciliationDashboardDataAgencyId?AgencyId=" + AgencyID + "&type=" + type).Result;
        }
        public ReconciliationCountModel GetReconciliationNegCountAgencyId(string AgencyId)
        {
            return GetAsync<ReconciliationCountModel>("Reconciliation/GetReconciliationNegCountAgencyId?AgencyId=" + AgencyId).Result;
        }
        public ReconciliationCountModel GetReconciliationCountAgencyId(string AgencyId)
        {
            return GetAsync<ReconciliationCountModel>("Reconciliation/GetReconciliationCountAgencyId?AgencyId=" + AgencyId).Result;
        }
        public ReturnStringModel GetDistinctAccount(int ClientId, string Type)
        {
            return GetAsync<ReturnStringModel>("Reconciliation/GetDistinctAccount?ClientId=" + ClientId + "&Type=" + Type).Result;
        }
        public ReturnModel UpdateReconciliation(int AgencyID, string id, int GLAccount, string BankRule, int TrackingCategory, int TrackingCategoryAdditional, int reconciliationActionId, string UserId, bool ? RuleNew)
        {
            string result = string.Format("Reconciliation/UpdateReconciliation?AgencyID={0}&id={1}&GLAccount={2}&BankRule={3}&TrackingCategory={4}&TrackingCategoryAdditional={5}&reconciliationActionId={6}&UserId={7}&RuleNew={8}", AgencyID, id, GLAccount, BankRule, TrackingCategory, TrackingCategoryAdditional, reconciliationActionId, UserId, RuleNew);
            return PostAsync<ReturnModel>(result).Result;
        }
        //public ReturnModelBulk BulkUpdateReconcilation(BulkActionParametersVM BPParameter)
        //{
        //    return PostAsync<ReturnModelBulk, BulkActionParametersVM>("Reconciliation/BulkUpdateReconcilation", BPParameter).Result;
        //}

        public ReturnModel BulkUpdateReconcilation(BulkActionParametersVM BPParameter)
        {
            return PostAsync<ReturnModel, BulkActionParametersVM>("Reconciliation/BulkUpdateReconcilation", BPParameter).Result;
        }

        public XeroReconcilationDataOnDemandRequestMainModel AddNewXeroOnDemandDataRequest(XeroReconcilationDataOnDemandRequestVM model)
        {
            return PostAsync<XeroReconcilationDataOnDemandRequestMainModel, XeroReconcilationDataOnDemandRequestVM>("Xero/AddNewXeroOnDemandDataRequest", model).Result;
        }
        public XeroReconcilationDataOnDemandRequestMainModel GetXeroOnDemandRequestStatus(string CurrentStatus)
        {
            return GetAsync<XeroReconcilationDataOnDemandRequestMainModel>("Reconciliation/GetXeroOnDemandRequestStatus?CurrentStatus=" + CurrentStatus).Result;
        }
        public XeroReconcilationDataOnDemandRequestMainModel GetXeroOnDemandRequestStatus(int AgencyID, string CurrentStatus, string CreaterUserID)
        {
            return GetAsync<XeroReconcilationDataOnDemandRequestMainModel>("Xero/GetXeroOnDemandRequestStatus?AgencyId=" + AgencyID + "&CurrentStatus=" + CurrentStatus + "&CreaterUserID=" + CreaterUserID).Result;
        }
        public XeroReconcilationDataOnDemandRequestMainModel GetXeroOnDemandRequestStatusById(int RequestID)
        {
            return GetAsync<XeroReconcilationDataOnDemandRequestMainModel>("Xero/GetXeroOnDemandRequestStatusById?RequestID=" + RequestID).Result;
        }
        public ReturnModel InsertReconcilationComments(ReconciliationComments comment)
        {
            return PostAsync<ReturnModel, ReconciliationComments>("Reconciliation/InsertReconcilationComments", comment).Result;
        }
        public ReturnNumberModel InsertReconcilationCommentsDetailsForAttachments(ReconciliationComments comment)
        {
            return PostAsync<ReturnNumberModel, ReconciliationComments>("Reconciliation/InsertReconcilationCommentsDetailsForAttachments", comment).Result;
        }
        public ReturnModel InsertReconcilationMentionedUsersDetails(ReconciliationCommentUserMention usermentioned)
        {
            return PostAsync<ReturnModel, ReconciliationCommentUserMention>("Reconciliation/InsertReconcilationMentionedUsersDetails", usermentioned).Result;
        }
        public XeroReconcilationDataOnDemandRequestMainModel getcommentsOnreconcliationId(int reconcliationId)
        {
            return GetAsync<XeroReconcilationDataOnDemandRequestMainModel>("Reconciliation/getcommentsOnreconcliationId?reconcliationId=" + reconcliationId).Result;
        }
        public async Task<ReconciliationOutputModelMainModel> XeroExtractionofManualImportedDatafromHtml(ReconciliationInputModel XeroInput, CancellationToken? cancellationToken = null)
        {
            return await PostAsyncWithCancellationToken<ReconciliationOutputModelMainModel, ReconciliationInputModel>("Reconciliation/XeroExtractionofManualImportedDatafromHtml", XeroInput, cancellationToken);
        }
        public async Task<ReconciliationOutputModelMainModel> QuickBooksExtractionofManualImportedDatafromCSV(ReconciliationInputModel QBOInput, CancellationToken? cancellationToken = null)
        {
            return await PostAsyncWithCancellationToken<ReconciliationOutputModelMainModel, ReconciliationInputModel>("Reconciliation/QuickBooksExtractionofManualImportedDatafromCSV", QBOInput, cancellationToken);
        }

        public async Task InsertReconciliationCommentAttachmentDetails(List<ReconciliationCommentAttachments> attachmentData)
        {
            await client.PostAsync($"Reconciliation/InsertReconciliationCommentAttachmentDetails", PreparePostContent(attachmentData));
        }
        public ReconcilationmainActionOptionVM GetAllReconcilationAction()
        {
            return GetAsync<ReconcilationmainActionOptionVM>("Reconciliation/GetAllReconcilationAction").Result;
        }
        public ReconciliationCommentAttachmentsMain getReconciliationCommentAttachments(int CommentId)
        {
            return GetAsync<ReconciliationCommentAttachmentsMain>("Reconciliation/getReconciliationCommentAttachments?reconiliationCommentId=" + CommentId).Result;
        }
        public ReturnModel DeleteReconciliationCommentAttachment(int CommentId)
        {
            return PostAsync<ReturnModel>("Reconciliation/DeleteReconciliationCommentAttachment?reconiliationCommentId=" + CommentId).Result;
        }
        public async Task<PlaidResponceModel> CreatePlaidReconciliation(List<company_reconciliationVM> attachmentData)
        {
            return await PostAsync<PlaidResponceModel>($"Reconciliation/CreatePlaidReconciliation", PreparePostContent(attachmentData));
        }
        public async Task<PlaidResponceModel> UpdateasReconciledallrecordsofAgency(int AgencyId,int IsReconciled)
        {
            return await PostAsync<PlaidResponceModel>($"Reconciliation/UpdateasReconciledallrecordsofAgency?AgencyId={AgencyId}&IsReconciled={IsReconciled}");
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed) return;

            if (disposing)
            {
                if (response != null)
                    response.Dispose();
                if (content != null)
                {
                    content.Dispose();
                }
                // free managed resources               
            }
            isDisposed = true;
        }
    }
}
