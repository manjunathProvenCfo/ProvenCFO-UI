using System;
using System.Collections.Generic;
using System.Text;

namespace QuickBooksSharp.Entities
{
   

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class ColDatum
    {
        public string id { get; set; }
        public string value { get; set; }
    }

    

    public class IncomeHeader
    {
        public string Customer { get; set; }
        public string ReportName { get; set; }
        public List<Option> Option { get; set; }
        public string DateMacro { get; set; }
        public string ReportBasis { get; set; }
        public string StartPeriod { get; set; }
        public string Currency { get; set; }
        public string EndPeriod { get; set; }
        public DateTime Time { get; set; }
    }

    public class Option
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class CutomerIncome
    {
        public IncomeHeader Header { get; set; }
        public Rows Rows { get; set; }
        public Columns Columns { get; set; }
    }

  

    public class IncomeSummary
    {
        public List<ColDatum> ColData { get; set; }
    }


}
