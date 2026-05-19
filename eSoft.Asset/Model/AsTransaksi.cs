using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSoft.Asset.Model
{
    public class AsTransaksi
    {
        [Key]
        public int AsTransaksiId { get; set; }
        public string Kode { get; set; }
        public string BarcodeAssets { get; set; }
        public DateTime Tanggal { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Nilai { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Saldo { get; set; }
        public string Cek { get; set; }

    }
}
