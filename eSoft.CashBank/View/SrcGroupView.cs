using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSoft.CashBank.View
{
    public class SrcGroupView
    {
        public int CbGrpId { get; set; }
        [StringLength(3)]
        public string Grp { get; set; }
        public string NamaGrp { get; set; }
    }
}
