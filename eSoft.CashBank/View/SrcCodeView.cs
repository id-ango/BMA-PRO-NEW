using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSoft.CashBank.Model
{
    
   
    public class SrcCodeView
    {

        public int SrcCodeId { get; set; }
        [Required]
        [MinLength(2)]
        [StringLength(3, ErrorMessage = "Source Code terlalu panjang (3 character limit).")]
        public string SrcCode { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Nama terlalu panjang (100 character limit).")]
        public string NamaSrc { get; set; }
        [StringLength(7)]
        public string GlAcct { get; set; }
        [StringLength(3)]
        public string Grp { get; set; }
    }

   
}
