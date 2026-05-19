using System;
using System.Collections.Generic;

namespace eSoft.Piutang.View
{
    public class ArForecastPiutangView
    {
        public int Bulan { get; set; }
        public int Tahun { get; set; }
        public string NamaBulan { get; set; }
        public decimal TotalTagihan { get; set; }
        public int JumlahDokumen { get; set; }
        public List<ArPiutangForecastDetail> Details { get; set; } = new();
    }

    public class ArPiutangForecastDetail
    {
        public string Dokumen { get; set; }
        public DateTime Tanggal { get; set; }
        public DateTime DueDate { get; set; }
        public string Customer { get; set; }
        public string NamaCust { get; set; }
        public decimal Jumlah { get; set; }
        public decimal Bayar { get; set; }
        public decimal Sisa { get; set; }
        public string Keterangan { get; set; }
        public string Salesman { get; set; }
    }
}