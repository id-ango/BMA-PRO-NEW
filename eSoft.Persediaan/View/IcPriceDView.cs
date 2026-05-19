using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSoft.Persediaan.View
{
    public class IcPriceDView
    {
        private decimal HrgPokok;
        private decimal jumlah;

        [Key]
        public int IcPriceDId { get; set; }
        [StringLength(2)]
        public string Kode { get; set; }
        public string NoLpb { get; set; }

        public string ItemCode { get; set; }
        public string NamaItem { get; set; }
        public string Satuan { get; set; }

        public string CurrencyCode { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Kurs { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal HargaBeli { get; set; }
        public decimal Qty { get; set; } = 1;

        [Column(TypeName = "decimal(18,2)")]
        public decimal HargaPokok
        {
            get
            {


                if (Kurs != 0)
                {
                    return (HargaBeli * Kurs) * Qty;
                }
                else
                {
                    return HrgPokok;
                }


            }
            set
            {
                HrgPokok = value;
            }
        }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Biaya { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Hpp { get; set; }
        public int IcPriceHId { get; set; }
        public IcPriceHView IcPriceH { get; set; }
    }
}