using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSoft.Persediaan.Model
{
    public class IcTransD
    {
        [Key]
        public int IcTransDId { get; set; }
        public string Kode { get; set; }
        public string NoOrder { get; set; }
        public string NoFaktur { get; set; }
        public string NoSj { get; set; }
        public DateTime Tanggal { get; set; }
        public string Lokasi { get; set; }
        public string Lokasi2 { get; set; }
        public string ItemCode { get; set; }
        public string NamaItem { get; set; }
        public string Satuan { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Harga { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal QtyBo { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal QtyShp { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Jumlah { get; set; }
        public string AcctSet { get; set; }
        public int IcPoTransD { get; set; }
        public int IcTransHId { get; set; }
        public IcTransH IcTransH { get; set; }
    }
}
