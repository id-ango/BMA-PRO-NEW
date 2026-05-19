using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSoft.Financial.Model
{
    public class FcPrintTP
    {
        [Key]
        public int FcPrintTPId { get; set; }
        public string KodeCetak { get; set; }
        public string NamaCetak { get; set; }
        public string JnsReport { get; set; }
        public string FcComKode { get; set; }
    }
}
