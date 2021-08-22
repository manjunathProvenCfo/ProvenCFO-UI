using Newtonsoft.Json;
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
            //response = client.GetAsync("Notes/GetAllNotesCategories?Status=" + Status + "&AgencyID=" + AgencyID).Result;
            //if (response.IsSuccessStatusCode)
            //{
            //    var _content = response.Content.ReadAsStringAsync().Result;
            //    return JsonConvert.DeserializeObject<NotesCategoriesMainModel>(_content);
            //}
            //else
            //{
            //    string msg = response.ReasonPhrase;
            //    throw new Exception(msg);
            //}
        }


        public NotesDescriptionMainModel GetAllNotesDescription(string Status)
        {
            return GetAsync<NotesDescriptionMainModel>("Notes/GetAllNotesDescription?Status=" + Status).Result;
            //response = client.GetAsync("Notes/GetAllNotesDescription?Status=" + Status).Result;
            //if (response.IsSuccessStatusCode)
            //{
            //    var _content = response.Content.ReadAsStringAsync().Result;
            //    return JsonConvert.DeserializeObject<NotesDescriptionMainModel>(_content);
            //}
            //else
            //{
            //    string msg = response.ReasonPhrase;
            //    throw new Exception(msg);

            //}
        }


        public NotesDescriptionMainModel CreateNotes(NotesDescriptionModel notesDescription, string LoginUserID)
        {
           // var form = new Dictionary<string, dynamic>
           //{
           //     {"Title", notesDescription.Title},
           //     {"NoteCatId", Convert.ToString(notesDescription.NoteCatId)},
           //     {"AgencyId", Convert.ToString(notesDescription.AgencyId)},
           //     {"Description", notesDescription.Description},
           //     {"Status", notesDescription.Status},
           //     {"CreatedBy", LoginUserID},
           //     {"IsDeleted", "false"},

           //};

            NotesDescriptionModel ndm = new NotesDescriptionModel();

            ndm.Title = notesDescription.Title;
            ndm.NoteCatId = notesDescription.NoteCatId;
            ndm.AgencyId = notesDescription.AgencyId;
            ndm.Description = notesDescription.Description;
            ndm.Status = notesDescription.Status;
            ndm.CreatedBy = LoginUserID;
            ndm.IsDeleted = false;

            //content = new StringContent(JsonConvert.SerializeObject(ndm), Encoding.UTF8, "application/json");
            return PostAsync<NotesDescriptionMainModel, NotesDescriptionModel>("Notes/CreateNotes", ndm).Result;
            //response = client.PostAsync("Notes/CreateNotes", content).Result;

            //if (response.IsSuccessStatusCode)
            //{
            //    var _content = response.Content.ReadAsStringAsync().Result;
            //    return JsonConvert.DeserializeObject<NotesDescriptionMainModel>(_content);
            //}
            //else
            //{
            //    string msg = response.ReasonPhrase;
            //    throw new Exception(msg);

            //}
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
