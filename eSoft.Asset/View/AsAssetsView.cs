using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSoft.Asset.View
{
    public class AsAssetsView
    {
        private decimal penyusutan;
        public int AsAssetsId { get; set; }

        [Required]
        [MinLength(5)]
        [StringLength(5, ErrorMessage = "BarCode terlalu panjang (5 character limit).")]
        public string KodeBarcodeAssets { get; set; }
        public string BarcodeAssets { get; set; }
        public string AsItemCode { get; set; }
        public string Merek { get; set; }
        public string NamaBarang { get; set; }
        public string NoMesin { get; set; }
        public string NoRangka { get; set; }
        public string NoPol { get; set; }
        public int Qty { get; set; }
        public decimal Nilai { get; set; }
        public decimal PPn { get; set; }
        public decimal Asuransi { get; set; }
        public decimal Bunga { get; set; }
        public decimal Administrasi { get; set; }
        public decimal Provisi { get; set; }
        public decimal Termin { get; set; }
        public decimal Penyusutan
        {
            get
            {
                if (Termin != 0)
                {
                    return Nilai / Termin;
                }
                else
                {
                    return penyusutan;
                }
            }
            set
            {
                penyusutan = value;
            }
        }

        public decimal SisaNilai { get; set; }
        public DateTime TglBeli { get; set; }
        public DateTime JatuhTempo { get; set; }
        public decimal NilaiTerjual { get; set; }
        public DateTime TglJual { get; set; }

        public string Acctset { get; set; }
        public string DistCode { get; set; }
    }
}
