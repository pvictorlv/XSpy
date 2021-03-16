using System.Collections.Generic;
using System.Linq;

namespace XSpy.Database.Models.Tables
{
    public class DataTableRequest
    {
        public int Draw { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
        public DataTableSearch Search { get; set; }

        public IEnumerable<ColumnDataTable> Columns { get; set; }
        public IEnumerable<OrderDataTable> Order { get; set; }

        public ColumnOrderDataTable GetOrderedColumn()
        {
            if (Order == null || Columns == null)
            {
                return null;
            }

            var order = Order.First();
            var column = Columns.ElementAt(order.Column);
            return new ColumnOrderDataTable()
            {
                Column = column,
                Order = order
            };
        }
        
        public List<ColumnOrderDataTable> GetOrderedColumns()
        {
            if (Order == null || Columns == null)
            {
                return null;
            }

            var ret = new List<ColumnOrderDataTable>();
            foreach (var orderDataTable in Order)
            {
                var column = Columns.ElementAt(orderDataTable.Column);
                ret.Add(new ColumnOrderDataTable()
                {
                    Column = column,
                    Order = orderDataTable
                });
            }

            return ret;
        }

        public ColumnOrderDataTable GetLastOrderedColumn()
        {
            if (Order == null || Columns == null || Columns.Count() <= 1)
            {
                return null;
            }

            var order = Order.Last();
            var column = Columns.ElementAt(order.Column);
            return new ColumnOrderDataTable()
            {
                Column = column,
                Order = order
            };
        }
    }

    public class DataTableSearch
    { 
        public string Value { get; set; }
        public bool Regex { get; set; }
    }

    public class ColumnDataTable
    {
        public string Data { get; set; }
        public string Name { get; set; }
    }

    public class OrderDataTable
    {
        public int Column { get; set; }
        public string Dir { get; set; }
    }

    public class ColumnOrderDataTable
    {
        public ColumnDataTable Column { get; set; }
        public OrderDataTable Order { get; set; }
    }

    public class DataTableRequest<TSearchObject> : DataTableRequest
    {
        public TSearchObject Filter { get; set; }
        public TSearchObject Clear { get; set; }
    }
}