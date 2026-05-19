using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSoft.Hutang.View
{
    public class ApAcctView
    {
        public int ApAcctId { get; set; }
        [Required(ErrorMessage = "AkunSet harus diisi")]
        public string AcctSet { get; set; }
        [StringLength(100, ErrorMessage = "Keterangan terlalu panjang (100 character limit).")]
        public string Description { get; set; }
        public string Acct1 { get; set; }
        public string Acct2 { get; set; }
        public string Acct3 { get; set; }
        public string Acct4 { get; set; }
        public string Acct5 { get; set; }
        public string Acct6 { get; set; }
    }
}
