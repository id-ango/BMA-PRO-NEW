using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace eSoft.Persediaan.Model
{
    public class IcLokasi
    {
        [Key]
        public int IcLokasiId { get; set; }
        public string Lokasi { get; set; }
        public string NamaLokasi { get; set; }
        public string Alamat { get; set; }
        public string Kota { get; set; }
        public string Telpon { get; set; }
    }
}
