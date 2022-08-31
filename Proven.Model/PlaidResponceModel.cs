using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proven.Model
{
    public class PlaidResponceModel
    {
        public int TotalNotInBanks { get; set; }

        public int TotalNotInBooks { get; set; }

        public int TotalInsertedRecords { get; set; }

        public int TotalUpdatedRecords { get; set; }

        public bool Status { get; set; }

        public string Error { get; set; }

        public string ErrorDesctiption { get; set; }

        public string ErrorType { get; set; }
    }
}
