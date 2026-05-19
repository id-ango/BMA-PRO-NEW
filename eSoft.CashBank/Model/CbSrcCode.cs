using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSoft.CashBank.Model
{
    
   
    public class CbSrcCode
    {
        public int CbSrcCodeId { get; set; }
        public string SrcCode { get; set; }
        public string NamaSrc { get; set; }
        public string GlAcct { get; set; }
        [StringLength(3)]
        public string Grp { get; set; }
    }

   
}
