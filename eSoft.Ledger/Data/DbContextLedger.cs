using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eSoft.Ledger.Model;
using Microsoft.EntityFrameworkCore;


namespace eSoft.Ledger.Data

{
    public class DbContextLedger : DbContext
    {
        public DbContextLedger(DbContextOptions<DbContextLedger> options) : base(options)
        {
        }

        public DbSet<GlAccount> GlAccounts { get; set; }
        public DbSet<GlCode> GlCodes { get; set; }
        public DbSet<GlTransH> GlTransHs { get; set; }
        public DbSet<GlTransD> GlTransDs { get; set; }
    }
}
