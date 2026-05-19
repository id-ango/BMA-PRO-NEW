using System;

namespace eSoft.Piutang.View
{
    /// <summary>
    /// Realisasi penerimaan / collection per bulan dari ArTransD.
    /// </summary>
    public class ArMonthlyCollectionView
    {
        public int Tahun { get; set; }
        public int Bulan { get; set; }
        public string LabelBulan => $"{new DateTime(Tahun, Bulan, 1):MMM yy}";
        public decimal TotalBayar { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalPenerimaan => TotalBayar + TotalDiscount;
        public int JumlahFaktur { get; set; }
        public int JumlahCustomer { get; set; }
    }
}
