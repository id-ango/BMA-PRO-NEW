using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSoft.Asset.Model
{
    public class AsDivisi
    {
       [Key]
        public int AsDivId { get; set; }
        public string Divisi { get; set; }
        public string NamaDiv { get; set; }
    }
}
