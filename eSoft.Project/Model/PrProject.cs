using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace eSoft.Project.Model
{
    public class PrProject
    {
        public int PrProjectID { get; set; }
        public string KodeProject { get; set; }
        public string NamaProject { get; set; }
        public string Keterangan { get; set; }
        public DateTime TglStart { get; set; }
        public DateTime TglFinish { get; set; }
        public decimal TotalProject { get; set; }
        public decimal Terima { get; set; }
        public decimal Bayar { get; set; }
        public decimal Saldo { get; set; }
        public bool Finish { get; set; }
    }
}
