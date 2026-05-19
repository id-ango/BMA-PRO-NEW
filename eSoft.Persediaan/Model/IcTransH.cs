using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSoft.Persediaan.Model
{
    public class IcTransH
    {
        [Key]
        public int IcTransHId { get; set; }
        public string Kode { get; set; }
        public string NoOrder { get; set; }
        public string NoFaktur { get; set; }
        public string NoSj { get; set; }
        public DateTime Tanggal { get; set; }
        public string Lokasi { get; set; }
        public string Lokasi2 { get; set; }
        public string Keterangan { get; set; }
        public string AcctSet { get; set; }
        public bool Pajak { get; set; }
        public List<IcTransD> IcTransDs { get; set; }
    }
}
