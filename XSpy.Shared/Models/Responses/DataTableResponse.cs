using System.Collections.Generic;

namespace XSpy.Database.Models.Responses
{
    public class DataTableResponse<TEntity>
    {
        public long RecordsFiltered { get; set; }
        public long RecordsTotal { get; set; }
        public long Draw { get; set; }
        public IEnumerable<TEntity> Data { get; set; }

    }
}