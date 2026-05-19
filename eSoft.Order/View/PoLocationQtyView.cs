using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace eSoft.Order.View
{
    public class PoLocationQtyView
    {
        public string Lokasi { get; set; }
        public string NamaLokasi { get; set; }
        public Decimal Qty { get; set; }
    }
}
