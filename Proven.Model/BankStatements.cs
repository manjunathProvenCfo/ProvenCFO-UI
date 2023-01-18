using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proven.Model
{

    
    public class BankStatements
    {
        public string Id { get; set; }
        public string Status { get; set; }
        public string ProviderName { get; set; }
        public DateTime DateTimeUTC { get; set; }
        public List<Report> Reports { get; set; }
    }
    public class Attribute
    {
        public string Value { get; set; }
        public string Id { get; set; }
    }

    public class Cell
    {
        public string Value { get; set; }
        public List<Attribute> Attributes { get; set; }
    }

    public class Report
    {
        public string ReportID { get; set; }
        public string ReportName { get; set; }
        public string ReportType { get; set; }
        public List<string> ReportTitles { get; set; }
        public string ReportDate { get; set; }
        public DateTime UpdatedDateUTC { get; set; }
        public List<object> Fields { get; set; }
        public List<Row> Rows { get; set; }
    }



    public class Row
    {
        public string RowType { get; set; }
        public List<Cell> Cells { get; set; }
        public string Title { get; set; }
        public List<Row> Rows { get; set; }
    }


}
