using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace eSoft.Piutang.View
{
    public class ResultLunasView
    {
        public string NoLpb { get; set; }
        public DateTime Tanggal { get; set; }
        public string Customer { get; set; }
        public string NamaCust { get; set; }
        public decimal Jumlah { get; set; }
        public string Status { get; set; }
        public DateTime TanggalLunas { get; set; }
        public string KdBank { get; set; }
        public int OeTransHId { get; set; }
        public int SelisihTanggal { get; set; }
        public bool IsSelected { get; set; }
    }

}