﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proven.Model
{
    public class NotesDescriptionModel
    {
        public int? Id { get; set; }
        public string NoteCatId { get; set; }
        public string AgencyId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedByUser { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedByUser { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsDeleted { get; set; }
    }
    public class NotesDescriptionMainModel
    {
        public bool Status { get; set; }
        public int statusCode { get; set; }
        public List<NotesDescriptionModel> ResultData { get; set; }
        public string Message { get; set; }
        public object ResourceType { get; set; }
        public object MetaData { get; set; }
    }
}
