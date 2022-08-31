using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proven.Model
{
    public class GridResponse<T> where T : class
    {
        public IEnumerable<T> data { get; set; }
        public int Total { get; set; }

    }
    public class GridRequest<T> where T : class
    {
        public int PAGE_NUMBER { get; set; }
        public int Type { get; set; }
        public int ROWS_PER_PAGE { get; set; }
        public string SORT_BY { get; set; }
        public string @SORT_DIRECTION { get; set; }
        public DateTime START_DATE { get; set; }
        public DateTime END_DATE { get; set; }
        public T param { get; set; }
        public Func<IQueryable<T>, IOrderedQueryable<T>> orderBy { get; set; }

    }
}