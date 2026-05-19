using System;
using System.Collections.Generic;

namespace eSoft.Order.View
{
    public class SalesOrderStockMatrixView
    {
        public List<SalesOrderItemHeader> ItemHeaders { get; set; } = new();
        public List<SalesOrderMatrixRow> Rows { get; set; } = new();
    }

    public class SalesOrderItemHeader
    {
        public string ItemCode { get; set; }
        public string NamaItem { get; set; }
        public string Satuan { get; set; }
        public decimal QtyStock { get; set; }
    }

    public class SalesOrderMatrixRow
    {
        public int PoTransHId { get; set; }
        public string NoLpb { get; set; }
        public string NamaCustomer { get; set; }
        public DateTime Tanggal { get; set; }
        public string Keterangan { get; set; }
        public string NoPrj { get; set; }
        public bool IsComplete { get; set; }
        public List<SalesOrderMatrixCell> Cells { get; set; } = new();
    }

    public class SalesOrderMatrixCell
    {
        public string ItemCode { get; set; }
        public decimal QtyOrder { get; set; }
        /// <summary>Sisa stock sebelum SO ini mengambil (rolling FIFO)</summary>
        public decimal QtyStockSisa { get; set; }
        /// <summary>Sisa stock setelah SO ini terpenuhi (bisa negatif)</summary>
        public decimal QtyStockSetelah => QtyStockSisa - QtyOrder;
        public bool HasStock => QtyStockSisa >= QtyOrder && QtyOrder > 0;
        public bool IsOrdered => QtyOrder > 0;
    }
}
