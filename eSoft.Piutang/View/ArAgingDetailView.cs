using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSoft.Piutang.View
{
    public class ArAgingDetailView
    {
        public string Kode { get; set; }
        public string Customer { get; set; }
        public string NamaCust { get; set; }
        public string Dokumen { get; set; }
        public DateTime Tanggal { get; set; }
      //  public DateTime DueDate { get; set; }
        public string Salesman { get; set; }
        public string Keterangan { get; set; }
        public string Remarks { get; set; }
        public decimal Total { get; set; }
        public decimal Sisa { get; set; }
        public bool Cicilan { get; set; }
        public List<CicilanDetail> CicilanList { get; set; } = new();
    }

    public class CicilanDetail
    {
        public DateTime Tanggal { get; set; }
        public decimal Jumlah { get; set; }
        public decimal Bayar { get; set; }
    }



}
