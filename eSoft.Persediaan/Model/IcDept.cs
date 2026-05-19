using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSoft.Persediaan.Model
{
    public class IcDept
    {
        [Key]
        public int IcDeptId { get; set; }
        public string DeptCode { get; set; }
        public string NamaDept { get; set; }
    }
}
