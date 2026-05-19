using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eSoft.Financial.Model;
using Microsoft.EntityFrameworkCore;



namespace eSoft.Financial.Data
{
    public class DbContextFinancial : DbContext
    {
        public DbContextFinancial(DbContextOptions<DbContextFinancial> options) : base(options)
        {
        }

        public DbSet<FcAccount> FcAccounts { get; set; }
     
        public DbSet<FcTransH> FcTransHs { get; set; }
        public DbSet<FcTransD> FcTransDs { get; set; }
        public DbSet<FcCom> FcComs { get; set; }
        public DbSet<FcGlTransH> FcGlTransHs { get; set; }
        public DbSet<FcGlTransD> FcGlTransDs { get; set; }
        public DbSet<FcPrintGl> FcPrintGls { get; set; }
        public DbSet<FcPrintTP> FcPrintTPs { get; set; }

    }
}
