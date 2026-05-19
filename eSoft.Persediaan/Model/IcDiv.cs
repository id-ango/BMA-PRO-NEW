using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSoft.Persediaan.Model
{
    public class IcDiv
    {
        [Key]
        public int IcDivId { get; set; }
        public string Divisi { get; set; }
        public string NamaDiv { get; set; }
    }
}
