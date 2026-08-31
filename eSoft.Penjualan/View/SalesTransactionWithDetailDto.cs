using System.Collections.Generic;
using eSoft.Penjualan.Model;

namespace eSoft.Penjualan.View
{
    public class SalesTransactionWithDetailDto
    {
        public OeTransH Header { get; set; }
        public List<OeTrans> Details { get; set; } = new();
    }
}
