using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSoft.Company.View
{
    public class CoSetupView
    {
        [Key]
        public int CoSetupId { get; set; }
        [StringLength(3)]
        public string CoKode { get; set; }
        public string CoName { get; set; }
        public string Account1 { get; set; }
        public string Account2 { get; set; }
        public string Account3 { get; set; }
        public string Account4 { get; set; }
        public string Account5 { get; set; }
        public string Account6 { get; set; }

    }
}
