using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSoft.Persediaan.View
{
    public class IcDivView
    {
        [Key]
        public int IcDivId { get; set; }
        [Required]
        public string Divisi { get; set; }
        [Required]
        public string NamaDiv { get; set; }
    }
}
