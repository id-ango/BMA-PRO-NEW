using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSoft.Order.View
{
    public class PoItemQtyByLocationView
    {
        public string ItemCode { get; set; }
       
        public string NamaItem { get; set; }
        public string Satuan { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [DisplayFormat(DataFormatString = "#,###.##")]
        public decimal Qty { get; set; }
        public decimal QtyAwal { get; set; }
        public List<PoLocationQtyView> Locations { get; set; }
    }
}
