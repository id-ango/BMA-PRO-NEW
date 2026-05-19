using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSoft.Financial.Model
{
    public class FcCom
    {

        [Key]
        public int FcComId { get; set; }
        public string FcComKode { get; set; }
        public string FcNamaPerusahaan { get; set; }
        public string FcAlamat { get; set; }
        public string GlAcct1 { get; set; }
        public string GlAcct2 { get; set; }
        public string GlAcct3 { get; set; }
        public string GlAcct4 { get; set; }
        public string GlAcct5 { get; set; }
        public string GlAcct6 { get; set; }
        public string GlDept1 { get; set; }
        public string GlDept2 { get; set; }
        public string GlDept3 { get; set; }
        public string GlDept4 { get; set; }
        public string GlDept5 { get; set; }
        public string GlDept6 { get; set; }
        public int FcFiscalYear { get; set; }
    }
}
