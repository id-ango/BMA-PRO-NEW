using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSoft.TestRun.View
{
    public class TsScheduleView
    {

        [Key]
        public int TsScheduleId { get; set; }
        public int TsOrder { get; set; }
        public string Dokumen { get; set; }
        public string Customer { get; set; }
        public string NamaCustomer { get; set; }
        public string Daftar { get; set; }
        public string Keterangan { get; set; }
        public string Daerah { get; set; }
        public DateTime TglKirim { get; set; }
        public DateTime TglTest { get; set; }
        public string HasilTest { get; set; }
        public string PInvoice { get; set; }
        public bool Tested { get; set; }
    }
}
