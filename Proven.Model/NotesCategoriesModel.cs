using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proven.Model
{
    [Serializable]
    public class NotesCategoriesModel
    {
        public string Id { get; set; }
        public string NoteCategory { get; set; }
        public string Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string IsDeleted { get; set; }
        public List<NotesDescriptionModel> NotesCategoriesList { get; set; }
    }

    public class NotesCategoriesMainModel
    {
        public bool Status { get; set; }
        public int statusCode { get; set; }
        public List<NotesCategoriesModel> ResultData { get; set; }
        public string Message { get; set; }
        public object ResourceType { get; set; }
        public object MetaData { get; set; }
    }
}
