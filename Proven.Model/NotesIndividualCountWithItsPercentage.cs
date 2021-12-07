using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proven.Model
{
    [Serializable]
    public class NotesIndividualCountWithItsPercentage
    {
        public string NoteCategoryName { get; set; }
        public int NoteCategoryCount { get; set; }
        public double CategoriesPercentage { get; set; }
        public int TotalNotes { get; set; }
        public string AgencyId { get; set; }
    }

    public class NotesIndividualCountWithItsPercentageMainModel
    {
        public bool Status { get; set; }
        public int statusCode { get; set; }
        public List<NotesIndividualCountWithItsPercentage> ResultData { get; set; }
        public string Message { get; set; }
        public object ResourceType { get; set; }
        public object MetaData { get; set; }

    }
}
