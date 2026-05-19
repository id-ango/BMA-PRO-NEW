using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSoft.Piutang.View
{
    public class ArDistView
    {
        public int ArDistId { get; set; }
        [Required(ErrorMessage = "Distribution Code harus diisi")]
        public string DistCode { get; set; }
        [StringLength(100, ErrorMessage = "Keterangan terlalu panjang (100 character limit).")]
        public string Description { get; set; }
        public string Dist1 { get; set; }
    }
}
