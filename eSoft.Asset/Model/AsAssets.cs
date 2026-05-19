using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace eSoft.Asset.Model
{
    public class AsAssets
    {
       [Key]
        public int AsAssetsId { get; set; }
        public string BarcodeAssets { get; set; }
        public string AsItemCode{ get; set; }
        public string Merek { get; set; }
        public string NamaBarang { get; set; }
        public string NoMesin { get; set; }
        public string NoRangka { get; set; }
        public string NoPol { get; set; }
        public int Qty { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Nilai { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal PPn { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Asuransi { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Bunga { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Administrasi { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Provisi { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Termin { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Penyusutan { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal SisaNilai { get; set; }
        public DateTime TglBeli { get; set; }
        public DateTime JatuhTempo { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal NilaiTerjual { get; set; }
        public DateTime TglJual { get; set; }
      
        public string Acctset { get; set; }
        public string DistCode { get; set; }

    }
}
