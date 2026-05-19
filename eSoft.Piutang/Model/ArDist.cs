using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSoft.Piutang.Model
{
    public class ArDist
    {
        public int ArDistId { get; set; }
        public string DistCode { get; set; }
        public string Description { get; set; }
        public string Dist1 { get; set; }
    }
}
