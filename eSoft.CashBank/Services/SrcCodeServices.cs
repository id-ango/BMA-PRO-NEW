using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eSoft.CashBank.Model;



namespace eSoft.CashBank.Services
{
    public class SrcCodeServices
    {

        private List<CbSrcCode> Banks;
        public string Uid { get; set; }


        public SrcCodeServices()
        {
            Banks = new List<CbSrcCode>
            {
                new CbSrcCode {CbSrcCodeId = 1,SrcCode="T1", NamaSrc="B1",GlAcct="20001"},
             new CbSrcCode {CbSrcCodeId = 2,SrcCode="G1", NamaSrc="B2",GlAcct="20201"},
             new CbSrcCode {CbSrcCodeId = 3,SrcCode="E1", NamaSrc="B3",GlAcct="20401"},

            };

            Uid = Guid.NewGuid().ToString();
          
        }
        public IEnumerable<CbSrcCode> GetBanks()
        {
            return Banks;
        }
    }
}