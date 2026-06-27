using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSoft.Persediaan.View
{
    public class IcPriceHView
    {
        private decimal harga;
        private decimal ppnmsk;
        private decimal pph;
        private decimal expedisi;
        private decimal totalcost;
        private decimal keuntungan;
        private decimal HargabefPPn;
        private decimal gprofit;
        private decimal markup;


        [Key]
        public int IcPriceHId { get; set; }
        [StringLength(2)]
        public string Kode { get; set; }
        public string NoLpb { get; set; }
        public string NoPrj { get; set; }

        public decimal CNY { get; set; } = 2700;
        public decimal USD { get; set; } = 18000;
        public decimal Harga
        {
            get
            {
                if (harga == 0)
                {
                    return IcPriceDs.Sum(p => p.HargaPokok);
                }
                else
                {
                    return harga;
                }
            }
            set
            {
                harga = value;
            }
        }
        [Column(TypeName = "decimal(18,2)")]
        public decimal NilaiPPnM { get; set; } = 11;

        [Column(TypeName = "decimal(18,2)")]
        public decimal NilaiPPnK { get; set; } = 11;
        [Column(TypeName = "decimal(18,2)")]
        public decimal NilaiPPh { get; set; } = 2.50M;

        [Column(TypeName = "decimal(18,2)")]
        public decimal NilaiExpd { get; set; } = 3;

        [Column(TypeName = "decimal(18,2)")]
        public decimal PPnMasukan
        {

            get
            {
                return Harga * (NilaiPPnM != 0 ? NilaiPPnM / 100 : 0);
            }
        }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PPnKeluaran
        {

            get
            {
                return (NilaiPPnK != 0 ? 1 + (NilaiPPnK / 100) : 111 / 100);
            }

        }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PPh
        {
            get
            {
                return Harga * (NilaiPPh != 0 ? NilaiPPh / 100 : 0);
            }
        }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Expedisi
        {
            get
            {
                return Harga * (NilaiExpd != 0 ? NilaiExpd / 100 : 0);
            }
        }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TambahanBiaya
        {
            get
            {
                return Math.Round(PPnMasukan + PPh + Expedisi);
            }
        }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalCost
        {
            get
            {
                if (totalcost == 0)
                {
                    return Harga + TambahanBiaya;
                }
                else
                {
                    return totalcost;
                }

            }
            set
            {
                totalcost = value;
            }
        }

        public decimal HargaJual { get; set; }
        public decimal HargabefPPN
        {
            get
            {
                if (PPnKeluaran != 0)
                    return (HargaJual / PPnKeluaran);
                else
                    return 0;
            }
            set
            {
                HargabefPPn = value;
            }

        }


        public decimal Keuntungan
        {
            get
            {
                if (TotalCost != 0)
                {
                    keuntungan = HargabefPPN - TotalCost;

                    return keuntungan;
                }
                else
                {
                    return keuntungan;
                }
            }

            set
            {
                keuntungan = value;
            }

        }
        public decimal MarkUp
        {
            get
            {
                if (TotalCost != 0)
                    return (Keuntungan / TotalCost) * 100;
                else
                    return 0;
            }
            set
            {
                markup = value;
            }
        }
        public decimal GProfit
        {
            get
            {
                if (HargabefPPN != 0)
                    return (Keuntungan / HargabefPPN) * 100;
                else
                    return 0;
            }
            set
            {
                gprofit = value;
            }
        }


        public string Keterangan { get; set; }

        public List<IcPriceDView> IcPriceDs { get; set; }

    }
}