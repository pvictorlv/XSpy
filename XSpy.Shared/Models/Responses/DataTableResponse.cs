using System.Collections.Generic;

namespace CFCEad.Shared.Models.Response
{
    public class DataTableResponse<TEntity>
    {
        public long RecordsFiltered { get; set; }
        public long RecordsTotal { get; set; }
        public long Draw { get; set; }
        public IEnumerable<TEntity> Data { get; set; }

    }
}