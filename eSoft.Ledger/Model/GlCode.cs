using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSoft.Ledger.Model
{
    public class GlCode
    {
        [Key]
        public int GlCodeId { get; set; }
        public string KodeGl { get; set; }
        public string NamaGl { get; set; }
    }
}
