﻿using Newtonsoft.Json;
using Proven.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Proven.Service
{
    public class NotesService : BaseService, IDisposable
    {
        private bool isDisposed = false;
        private HttpResponseMessage response;
        private StringContent content;



        public NotesCategoriesMainModel GetAllNotesCategories(string Status, int AgencyID)
        {
            return GetAsync<NotesCategoriesMainModel>("Notes/GetAllNotesCategories?Status=" + Status + "&AgencyID=" + AgencyID).Result;
        }


        public NotesDescriptionMainModel GetAllNotesDescription(string Status)
        {
            return GetAsync<NotesDescriptionMainModel>("Notes/GetAllNotesDescription?Status=" + Status).Result;
        }

        public NotesDescriptionModel GetNotesDescriptionById(int NotesDescriptionId)
        {
            return GetAsync<NotesDescriptionModel>("Notes/GetNotesDescriptionById?NotesDescriptionId=" + NotesDescriptionId, true).Result;
        }

        public NotesDescriptionMainModel1 CreateNewNotes(NotesDescriptionModel notesDescription)
        {
            //NotesDescriptionModel ndm = new NotesDescriptionModel();

            //ndm.Title = notesDescription.Title;
            //ndm.NoteCatId = notesDescription.NoteCatId;
            //ndm.AgencyId = notesDescription.AgencyId;
            //ndm.Description = notesDescription.Description;
            //ndm.Status = notesDescription.Status;
            //ndm.CreatedBy = LoginUserID;
            //notesDescription.IsDeleted = false;
            //notesDescription.IsPublished = "Published";

            return PostAsync<NotesDescriptionMainModel1, NotesDescriptionModel>("Notes/CreateNotes", notesDescription).Result;
        }

        public ReturnModel UpdateNotesDescription(int DescriptionId, string Title, string DescriptionText, string IsPublished,string LoginUserID)
        {
            NotesDescriptionModel ndm = new NotesDescriptionModel();
            ndm.Id = DescriptionId;
            ndm.Title = Title;
            ndm.Description = DescriptionText;
            ndm.IsPublished = IsPublished;
            ndm.ModifiedBy = LoginUserID;

            return PostAsync<ReturnModel, NotesDescriptionModel>("Notes/UpdateNotesDescription", ndm).Result;
        }

        public NotesDescriptionModel PublishingNotes(int Id)
        {
            string result = string.Format("Notes/PublishingNotes?Id={0}", Id);
            return PostAsync<NotesDescriptionModel>(result).Result;
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
