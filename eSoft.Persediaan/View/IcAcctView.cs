using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSoft.Persediaan.View
{
    public class IcAcctView
    {
        [Key]
        public int IcAcctId { get; set; }
        [Required]
        public string AcctSet { get; set; }
        public string Description { get; set; }
        public string Acct1 { get; set; }
        public string Acct2 { get; set; }
        public string Acct3 { get; set; }
        public string Acct4 { get; set; }
        public string Acct5 { get; set; }
        public string Acct6 { get; set; }
    }
}
