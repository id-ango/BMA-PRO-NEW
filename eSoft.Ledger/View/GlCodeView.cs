using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSoft.Ledger.View
{
    public class GlCodeView
    {
        [Key]
        public int GlCodeId { get; set; }
        [Required]
        [MaxLength(5)]
        public string KodeGl { get; set; }
        public string NamaGl { get; set; }
    }
}
